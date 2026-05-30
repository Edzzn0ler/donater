using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class AnvilPunishment : MonoBehaviour
{
    [SerializeField] private GameObject  anvilPrefab;
    [SerializeField] private AudioClip   impactSound;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float lookUpDuration = 0.8f;
    [SerializeField] private float fallHeight      = 8f;
    [SerializeField] private float fallDuration    = 0.5f;
    [SerializeField] private float fadeDuration    = 0.35f;
    [SerializeField] private float holdDuration    = 1.5f;

    private Image fadePanel;

    private static readonly FieldInfo RotationXField =
        typeof(PlayerLook).GetField("rotationX", BindingFlags.NonPublic | BindingFlags.Instance);

    private void Awake()
    {
        var canvasGO = new GameObject("_AnvilFadeOverlay");
        DontDestroyOnLoad(canvasGO);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var panelGO = new GameObject("FadePanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        var rt = panelGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        fadePanel = panelGO.AddComponent<Image>();
        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        fadePanel.raycastTarget = false;
        panelGO.SetActive(false);
    }

    public void Trigger(System.Action onComplete)
    {
        StartCoroutine(Sequence(onComplete));
    }

    private IEnumerator Sequence(System.Action onComplete)
    {
        var cam        = Camera.main;
        var playerLook = FindFirstObjectByType<PlayerLook>();
        var camT       = cam != null ? cam.transform : null;

        if (playerLook != null) playerLook.enabled = false;

        if (camT != null)
        {
            Quaternion startRot  = camT.localRotation;
            Quaternion targetRot = Quaternion.Euler(-80f, 0f, 0f);
            float t = 0f;
            while (t < lookUpDuration)
            {
                t += Time.deltaTime;
                camT.localRotation = Quaternion.Slerp(startRot, targetRot, t / lookUpDuration);
                yield return null;
            }
            camT.localRotation = targetRot;
        }

        GameObject anvil = null;
        if (anvilPrefab != null && camT != null)
        {
            Vector3 hitPos   = camT.position;
            Vector3 spawnPos = hitPos + Vector3.up * fallHeight;
            anvil = Instantiate(anvilPrefab, spawnPos, Quaternion.identity);

            float t = 0f;
            while (t < fallDuration)
            {
                t += Time.deltaTime;
                float eased = (t / fallDuration) * (t / fallDuration);
                if (anvil != null)
                    anvil.transform.position = Vector3.Lerp(spawnPos, hitPos, eased);
                yield return null;
            }
            if (anvil != null) anvil.transform.position = hitPos;
        }

        if (audioSource != null && impactSound != null)
            audioSource.PlayOneShot(impactSound);

        yield return FadeTo(1f);
        yield return new WaitForSecondsRealtime(holdDuration);

        if (anvil != null) Destroy(anvil);

        if (camT != null) camT.localRotation = Quaternion.identity;
        if (playerLook != null)
        {
            RotationXField?.SetValue(playerLook, 0f);
            playerLook.enabled = true;
        }

        onComplete?.Invoke();

        yield return FadeTo(0f);
        if (fadePanel != null) fadePanel.gameObject.SetActive(false);
    }

    private IEnumerator FadeTo(float target)
    {
        if (fadePanel == null) yield break;
        fadePanel.gameObject.SetActive(true);

        float start = fadePanel.color.a;
        float t     = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            Color c = fadePanel.color;
            c.a = Mathf.Lerp(start, target, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        Color final = fadePanel.color;
        final.a = target;
        fadePanel.color = final;
    }
}
