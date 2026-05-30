using UnityEngine;

public class HandTypingAnimator : MonoBehaviour
{
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private Vector3 leftBasePos;
    private Vector3 rightBasePos;

    private float leftPressTimer;
    private float rightPressTimer;
    private bool nextLeft = true;

    private const float PressDepth   = 0.019f;
    private const float PressDuration = 0.10f;
    private const float IdleAmplitude = 0.008f;
    private const float IdleSpeed     = 1.4f;

    private void Start()
    {
        leftBasePos  = leftHand.localPosition;
        rightBasePos = rightHand.localPosition;
    }

    private void Update()
    {
        leftPressTimer  = Mathf.Max(0, leftPressTimer  - Time.deltaTime);
        rightPressTimer = Mathf.Max(0, rightPressTimer - Time.deltaTime);

        float t = Time.time * IdleSpeed;
        float leftIdle  = Mathf.Sin(t)        * IdleAmplitude;
        float rightIdle = Mathf.Sin(t + 1.6f) * IdleAmplitude;

        float leftPress  = leftPressTimer  > 0
            ? Mathf.Sin(leftPressTimer  / PressDuration * Mathf.PI) * PressDepth : 0;
        float rightPress = rightPressTimer > 0
            ? Mathf.Sin(rightPressTimer / PressDuration * Mathf.PI) * PressDepth : 0;

        leftHand.localPosition  = leftBasePos  + Vector3.up * (leftIdle  - leftPress);
        rightHand.localPosition = rightBasePos + Vector3.up * (rightIdle - rightPress);
    }

    public void TriggerKeyPress()
    {
        if (nextLeft) leftPressTimer  = PressDuration;
        else          rightPressTimer = PressDuration;
        nextLeft = !nextLeft;
    }

    public void TriggerEnter()
    {
        leftPressTimer  = PressDuration * 1.5f;
        rightPressTimer = PressDuration * 1.5f;
    }
}
