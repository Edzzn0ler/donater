using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider                  sensitivitySlider;
    [SerializeField] private Slider                  volumeSlider;
    [SerializeField] private UnityEngine.UI.Toggle   clickSoundToggle;

    private void Awake()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1f);
    }

    private void Start()
    {
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
        volumeSlider.value      = PlayerPrefs.GetFloat("Volume", 1f);
        if (clickSoundToggle != null)
            clickSoundToggle.isOn = PlayerPrefs.GetInt("ClickSound", 1) == 1;

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        if (clickSoundToggle != null)
            clickSoundToggle.onValueChanged.AddListener(OnClickSoundChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
    }

    private void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
        AudioListener.volume = value;
    }

    private void OnClickSoundChanged(bool value)
    {
        PlayerPrefs.SetInt("ClickSound", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainScene");
    }
}
