using UnityEngine;

public class TypingSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioSource audioSource;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.7f;

    public void PlayClick()
    {
        if (clips == null || clips.Length == 0 || audioSource == null) return;
        if (PlayerPrefs.GetInt("ClickSound", 1) == 0) return;
        var clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip, volume);
    }
}
