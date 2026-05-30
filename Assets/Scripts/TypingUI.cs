using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI wpmText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI charCounter;

    private const string ColUntyped = "#888888";
    private const string ColCorrect = "#222222";
    private const string ColError   = "#FF4444";

    private float baseY;
    private bool  baseYReady;

    private RectTransform cursorRect;
    private float  blinkTimer;
    private bool   cursorVisible = true;
    private int    cachedCursor  = -1;

    private void Awake()
    {
        var go = new GameObject("_TextCursor");
        go.transform.SetParent(targetText.transform, false);

        cursorRect           = go.AddComponent<RectTransform>();
        cursorRect.anchorMin = new Vector2(0.5f, 0.5f);
        cursorRect.anchorMax = new Vector2(0.5f, 0.5f);
        cursorRect.pivot     = new Vector2(0f, 0f);
        cursorRect.sizeDelta = new Vector2(2f, 24f);

        var img   = go.AddComponent<Image>();
        img.color = new Color(1f, 0.55f, 0f, 1f);

        go.SetActive(false);
    }

    private void Update()
    {
        if (cachedCursor < 0 || cursorRect == null) return;
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= 0.53f)
        {
            blinkTimer    = 0f;
            cursorVisible = !cursorVisible;
            cursorRect.gameObject.SetActive(cursorVisible);
        }
    }

    public void RenderText(string text, bool[] typed, bool[] correct, int cursor)
    {
        cachedCursor  = cursor;
        blinkTimer    = 0f;
        cursorVisible = true;

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            string col;
            if      (!typed[i])   col = ColUntyped;
            else if (correct[i])  col = ColCorrect;
            else                  col = ColError;

            sb.Append("<color=").Append(col).Append(">")
              .Append(text[i])
              .Append("</color>");
        }
        targetText.text = sb.ToString();

        if (charCounter != null)
            charCounter.text = cursor + " / " + text.Length;

        ScrollToCursor(cursor);
        PlaceCursor(cursor);
    }

    private void PlaceCursor(int cursor)
    {
        if (cursorRect == null || targetText == null) return;

        targetText.ForceMeshUpdate();
        var info = targetText.textInfo;

        if (info.characterCount == 0)
        {
            cursorRect.gameObject.SetActive(false);
            return;
        }

        float x, bottom, top;

        if (cursor < info.characterCount)
        {
            var ci = info.characterInfo[cursor];
            x      = ci.bottomLeft.x;
            bottom = ci.descender;
            top    = ci.ascender;
        }
        else
        {
            var ci = info.characterInfo[info.characterCount - 1];
            x      = ci.bottomRight.x;
            bottom = ci.descender;
            top    = ci.ascender;
        }

        cursorRect.localPosition = new Vector3(x, bottom, 0f);
        cursorRect.sizeDelta     = new Vector2(2f, top - bottom);
        cursorRect.gameObject.SetActive(true);
    }

    private void ScrollToCursor(int cursorPos)
    {
        if (targetText == null) return;

        var rt = targetText.rectTransform;

        if (!baseYReady)
        {
            baseY      = rt.anchoredPosition.y;
            baseYReady = true;
        }

        targetText.ForceMeshUpdate();
        var info = targetText.textInfo;

        float preferred = targetText.preferredHeight + 4f;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, preferred);

        if (info.lineCount == 0 || info.characterCount == 0)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, baseY);
            return;
        }

        int charIdx    = Mathf.Clamp(cursorPos, 0, info.characterCount - 1);
        int cursorLine = info.characterInfo[charIdx].lineNumber;

        float lineH = info.lineInfo[0].lineHeight;
        if (lineH < 1f) lineH = targetText.fontSize * 1.25f;

        var   containerRT = (RectTransform)rt.parent;
        float containerH  = containerRT.rect.height;
        int   maxVisible  = Mathf.Max(1, Mathf.FloorToInt(containerH / lineH));

        int firstVisible = Mathf.Max(0, cursorLine - maxVisible + 1);

        float scroll = firstVisible * lineH;
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, baseY + scroll);
    }

    public void UpdateStats(int wpm, float accuracy, int errors, float elapsed)
    {
        wpmText.text      = wpm.ToString();
        accuracyText.text = accuracy.ToString("F0") + "%";
        errorText.text    = errors.ToString();
        int s = Mathf.FloorToInt(elapsed);
        timerText.text    = (s / 60).ToString("D2") + ":" + (s % 60).ToString("D2");
    }
}