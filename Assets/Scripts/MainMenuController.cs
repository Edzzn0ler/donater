using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void Play() => GameManager.Instance.StartGame();
    public void OpenSettings() => GameManager.Instance.OpenSettings();
    public void Exit() => Application.Quit();
}
