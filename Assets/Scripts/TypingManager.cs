using UnityEngine;

public class TypingManager : MonoBehaviour
{
    public event System.Action<int, float, int> OnTypingFinished;

    [SerializeField] private DonationQueue      donationQueue;
    [SerializeField] private TypingStats        stats;
    [SerializeField] private TypingUI           typingUI;
    [SerializeField] private HandTypingAnimator handAnimator;
    [SerializeField] private TypingSound        typingSound;
    [SerializeField] private AnvilPunishment    anvilPunishment;
    [SerializeField] private GameObject         typingPanel;
    [SerializeField] private KeyboardOilEffect  oilEffect;

    [Range(0f, 100f)]
    [SerializeField] private float accuracyThreshold = 50f;

    private string currentText;
    private bool[] typed;
    private bool[] correct;
    private int    cursor;
    private bool   isActive;

    private void Start()
    {
        LoadText();
    }

    private void Update()
    {
        if (isActive && stats.IsStarted)
            typingUI.UpdateStats(stats.WPM, stats.Accuracy, stats.Errors, stats.ElapsedSeconds);
    }

    private void OnGUI()
    {
        if (!isActive) return;
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;

        var e = Event.current;
        if (e.type != EventType.KeyDown) return;

        if (e.keyCode == KeyCode.Backspace)
        {
            HandleBackspace(); e.Use();
        }
        else if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
        {
            HandleEnter(); e.Use();
        }
        else if (e.character != '\0' && !char.IsControl(e.character))
        {
            HandleCharacter(e.character); e.Use();
        }
    }

    private void HandleCharacter(char c)
    {
        if (cursor >= currentText.Length) return;
        if (!stats.IsStarted) stats.Begin();

        typed[cursor]   = true;
        correct[cursor] = (c == currentText[cursor]);

        if (correct[cursor]) stats.RecordCorrect();
        else                 stats.RecordError();

        cursor++;
        handAnimator?.TriggerKeyPress();
        oilEffect?.TriggerSplash();
        typingSound?.PlayClick();
        typingUI.RenderText(currentText, typed, correct, cursor);

        if (cursor >= currentText.Length)
            HandleEnter();
    }

    private void HandleBackspace()
    {
        if (cursor <= 0) return;
        cursor--;
        typed[cursor]   = false;
        correct[cursor] = false;
        handAnimator?.TriggerKeyPress();
        oilEffect?.TriggerSplash();
        typingSound?.PlayClick();
        typingUI.RenderText(currentText, typed, correct, cursor);
    }

    private void HandleEnter()
    {
        if (cursor < currentText.Length) return;
        isActive = false;

        if (stats.Accuracy <= accuracyThreshold)
        {
            if (anvilPunishment != null)
            {
                if (typingPanel != null) typingPanel.SetActive(false);
                anvilPunishment.Trigger(() =>
                {
                    donationQueue.Reset();
                    LoadText();
                    if (typingPanel != null) typingPanel.SetActive(true);
                });
            }
            else
            {
                donationQueue.Reset();
                LoadText();
            }
            return;
        }

        handAnimator?.TriggerEnter();
        OnTypingFinished?.Invoke(stats.WPM, stats.Accuracy, stats.Errors);
    }

    public void LoadNextText()
    {
        donationQueue.Advance();
        LoadText();
    }

    private void LoadText()
    {
        currentText = donationQueue.Current.donationText;
        typed       = new bool[currentText.Length];
        correct     = new bool[currentText.Length];
        cursor      = 0;
        stats.Reset();
        typingUI.RenderText(currentText, typed, correct, cursor);
        typingUI.UpdateStats(0, 100f, 0, 0f);
        isActive = true;
    }
}
