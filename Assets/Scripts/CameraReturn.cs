using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;

public class CameraReturn : MonoBehaviour
{
    [SerializeField] private PlayerLook    playerLook;
    [SerializeField] private Transform     cameraTransform;
    [SerializeField] private Transform     playerBody;

    [Header("Позиция покоя (захватывается при старте)")]
    [SerializeField] private float restPitch = -10f;
    [SerializeField] private float restYaw   =   0f;

    [Header("Параметры")]
    [SerializeField] private float returnSpeed   = 4f;
    [SerializeField] private float typingTimeout = 1.5f;

    private float lastKeyTime = -10f;
    private bool  IsTyping    => Time.time - lastKeyTime < typingTimeout;

    private static readonly FieldInfo RotXField =
        typeof(PlayerLook).GetField("rotationX",
            BindingFlags.NonPublic | BindingFlags.Instance);

    private void Start()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        if (playerBody      == null) playerBody      = cameraTransform.parent;
        if (playerLook      == null) playerLook      = playerBody.GetComponent<PlayerLook>();

        CaptureCurrentRotationAsRest();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb.anyKey.wasPressedThisFrame)
            lastKeyTime = Time.time;
    }

    private void LateUpdate()
    {
        if (!IsTyping) return;

        float pitch = cameraTransform.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        float newPitch = Mathf.LerpAngle(pitch, restPitch, Time.deltaTime * returnSpeed);
        cameraTransform.localRotation = Quaternion.Euler(newPitch, 0f, 0f);

        RotXField?.SetValue(playerLook, newPitch);

        float yaw    = playerBody.eulerAngles.y;
        float newYaw = Mathf.LerpAngle(yaw, restYaw, Time.deltaTime * returnSpeed);
        playerBody.rotation = Quaternion.Euler(0f, newYaw, 0f);
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

    [ContextMenu("Capture Current Rotation as Rest")]
    private void CaptureCurrentRotationAsRest()
    {
        if (cameraTransform == null) return;

        restPitch = cameraTransform.localEulerAngles.x;
        if (restPitch > 180f) restPitch -= 360f;

        restYaw = playerBody != null ? playerBody.eulerAngles.y : 0f;
    }
}