using UnityEngine;
using UnityEngine.InputSystem;

public class HandFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothness    = 0.1f;
    [SerializeField] private float rotationSmoothness    = 8f;
    [SerializeField] private float rotationLagMultiplier = 2f;

    [Header("Offset (camera-local, auto-filled from scene position)")]
    [SerializeField] private Vector3 followOffset;

    [Header("Sway")]
    [SerializeField] private float swayAmount = 0.3f;
    [SerializeField] private float swaySmooth = 4f;

    [Header("Typing Return")]
    [Tooltip("Скорость перехода к позиции клавиатуры и обратно")]
    [SerializeField] private float typingBlendSpeed = 5f;
    [Tooltip("Секунд без нажатий до возврата в режим следования за камерой")]
    [SerializeField] private float typingTimeout    = 1.5f;

    private Vector3    posVelocity;
    private Quaternion currentRot;
    private Vector2    currentSway;

    private float      typingBlend;
    private float      lastKeyTime = -10f;

    private Vector3    keyboardLocalPos;
    private Quaternion keyboardRestRot;

    private bool IsTyping => Time.time - lastKeyTime < typingTimeout;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (playerTransform == null)
            playerTransform = cameraTransform.parent;

        CaptureCurrentPosition();

        keyboardLocalPos = playerTransform.InverseTransformPoint(transform.position);
        keyboardRestRot  = transform.rotation;

        currentRot = cameraTransform.rotation;
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb.anyKey.wasPressedThisFrame)
            lastKeyTime = Time.time;
    }

    private void LateUpdate()
    {
        float targetBlend = IsTyping ? 1f : 0f;
        typingBlend = Mathf.Lerp(typingBlend, targetBlend, Time.deltaTime * typingBlendSpeed);

        Vector3 cameraTarget   = TargetPosition();
        Vector3 keyboardTarget = playerTransform.TransformPoint(keyboardLocalPos);
        Vector3 finalTarget    = Vector3.Lerp(cameraTarget, keyboardTarget, typingBlend);

        transform.position = Vector3.SmoothDamp(
            transform.position, finalTarget, ref posVelocity, positionSmoothness);

        float rotSpeed = rotationSmoothness / rotationLagMultiplier;

        Quaternion targetRot = Quaternion.Slerp(
            cameraTransform.rotation, keyboardRestRot, typingBlend);
        currentRot = Quaternion.Slerp(currentRot, targetRot, Time.deltaTime * rotSpeed);

        Vector2 mouseDelta = Mouse.current != null
            ? Mouse.current.delta.ReadValue()
            : Vector2.zero;

        Vector2 targetSway = new Vector2(mouseDelta.y, -mouseDelta.x)
                             * swayAmount * (1f - typingBlend);
        currentSway = Vector2.Lerp(currentSway, targetSway, Time.deltaTime * swaySmooth);

        Quaternion swayRot = Quaternion.Euler(currentSway.x, 0f, currentSway.y);
        transform.rotation = currentRot * swayRot;
    }

    private void OnGUI()
    {
        var e = Event.current;
        if (e.type != EventType.KeyDown) return;
        if ((e.character != '\0' && !char.IsControl(e.character))
            || e.keyCode == KeyCode.Backspace
            || e.keyCode == KeyCode.Return
            || e.keyCode == KeyCode.KeypadEnter)
        {
            lastKeyTime = Time.time;
        }
    }

    private Vector3 TargetPosition() =>
        cameraTransform.position + cameraTransform.TransformDirection(followOffset);

    [ContextMenu("Capture Current Position as Offset")]
    private void CaptureCurrentPosition()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main != null ? Camera.main.transform : null;
        if (cameraTransform == null) return;

        followOffset = cameraTransform.InverseTransformDirection(
            transform.position - cameraTransform.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;
        Gizmos.color = Color.cyan;
        Vector3 target = TargetPosition();
        Gizmos.DrawSphere(target, 0.03f);
        Gizmos.DrawLine(cameraTransform.position, target);
    }
}