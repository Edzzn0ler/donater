using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [Header("Typing")]
    [SerializeField] private TypingManager typingManager;
    [SerializeField] private GameObject    typingPanel;

    [Header("Donation Queue")]
    [SerializeField] private DonationQueue donationQueue;

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage    videoDisplay;
    [SerializeField] private GameObject  videoPanel;

    [Header("Results")]
    [SerializeField] private GameObject               resultsPanel;
    [SerializeField] private TMPro.TextMeshProUGUI    wpmText;
    [SerializeField] private TMPro.TextMeshProUGUI    accuracyText;
    [SerializeField] private TMPro.TextMeshProUGUI    errorsText;

    [Header("Tab Bar")]
    [SerializeField] private UnityEngine.UI.Image tabBarImage;
    [SerializeField] private Sprite               tabDonationAlerts;
    [SerializeField] private Sprite               tabTwitch;

    [Header("Skip Button")]
    [SerializeField] private UnityEngine.UI.Button skipButton;
    [SerializeField] private float                 doubleClickWindow = 0.35f;

    private RenderTexture renderTexture;
    private bool isPlayingVideo;
    private bool skipRequested;
    private float lastSkipClickTime = -1f;

    private void Update()
    {
        if (!isPlayingVideo || videoPlayer == null) return;
        bool shouldPause = GameManager.Instance != null && GameManager.Instance.State == GameState.Paused;
        if (shouldPause && videoPlayer.isPlaying) videoPlayer.Pause();
        else if (!shouldPause && videoPlayer.isPaused) videoPlayer.Play();
    }

    private void Start()
    {
        if (videoPlayer != null && videoDisplay != null)
        {
            renderTexture = new RenderTexture(1280, 720, 0);
            videoPlayer.targetTexture = renderTexture;
            videoDisplay.texture = renderTexture;
        }
        if (videoPanel != null) videoPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
        SetTab(false);
        if (skipButton != null) skipButton.onClick.AddListener(OnSkipClicked);
        typingManager.OnTypingFinished += OnTypingFinished;
    }

    public void OnSkipClicked()
    {
        if (!isPlayingVideo) return;
        float now = Time.unscaledTime;
        if (now - lastSkipClickTime <= doubleClickWindow)
            skipRequested = true;
        lastSkipClickTime = now;
    }

    private void OnTypingFinished(int wpm, float accuracy, int errors)
    {
        StartCoroutine(Sequence(wpm, accuracy, errors));
    }

    private IEnumerator Sequence(int wpm, float accuracy, int errors)
    {
        if (typingPanel != null) typingPanel.SetActive(false);
        var clip = donationQueue != null ? donationQueue.Current.videoClip : null;
        bool hasVideo = videoPlayer != null && clip != null;

        if (hasVideo)
        {
            skipRequested = false;
            SetTab(true);
            if (videoPanel != null) videoPanel.SetActive(true);
            if (skipButton != null) skipButton.gameObject.SetActive(true);
            videoPlayer.clip = clip;
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);

            bool videoEnded = false;
            VideoPlayer.EventHandler onEnd = null;
            onEnd = _ => { videoEnded = true; videoPlayer.loopPointReached -= onEnd; };
            videoPlayer.loopPointReached += onEnd;

            videoPlayer.Play();
            isPlayingVideo = true;
            yield return new WaitUntil(() => videoEnded || skipRequested);
            isPlayingVideo = false;
            if (skipRequested) videoPlayer.loopPointReached -= onEnd;
            videoPlayer.Stop();
            if (skipButton != null) skipButton.gameObject.SetActive(false);
            if (videoPanel != null) videoPanel.SetActive(false);
            SetTab(false);
        }

        if (wpmText != null) wpmText.text = "WPM: " + wpm;
        if (accuracyText != null) accuracyText.text = "Accuracy: " + accuracy.ToString("F1") + "%";
        if (errorsText != null) errorsText.text = "Errors: " + errors;
        if (resultsPanel != null) resultsPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (resultsPanel != null) resultsPanel.SetActive(false);
        if (typingPanel != null) typingPanel.SetActive(true);
        typingManager.LoadNextText();
    }

    private void SetTab(bool twitch)
    {
        if (tabBarImage == null) return;
        tabBarImage.sprite = twitch ? tabTwitch : tabDonationAlerts;
    }

    private void OnDestroy()
    {
        if (typingManager != null) typingManager.OnTypingFinished -= OnTypingFinished;
        if (skipButton != null) skipButton.onClick.RemoveListener(OnSkipClicked);
        if (renderTexture != null) renderTexture.Release();
    }
}
