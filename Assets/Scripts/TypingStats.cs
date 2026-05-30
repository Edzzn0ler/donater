using UnityEngine;

public class TypingStats : MonoBehaviour
{
    private float startTime;
    private int correctCount;
    private int errorCount;
    private int totalTyped;

    public bool IsStarted { get; private set; }
    public int Errors => errorCount;
    public float ElapsedSeconds => IsStarted ? Time.time - startTime : 0f;

    public int WPM
    {
        get
        {
            if (!IsStarted) return 0;
            float minutes = ElapsedSeconds / 60f;
            if (minutes < 0.001f) return 0;
            return Mathf.RoundToInt((correctCount / 5f) / minutes);
        }
    }

    public float Accuracy => totalTyped > 0 ? (correctCount / (float)totalTyped) * 100f : 100f;

    public void Reset()
    {
        startTime = 0f;
        correctCount = 0;
        errorCount = 0;
        totalTyped = 0;
        IsStarted = false;
    }

    public void Begin()
    {
        startTime = Time.time;
        IsStarted = true;
    }

    public void RecordCorrect() { correctCount++; totalTyped++; }
    public void RecordError()   { errorCount++;   totalTyped++; }
}
