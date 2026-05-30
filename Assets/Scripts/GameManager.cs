using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Paused }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private PlayerLook playerLook;

    public GameState State { get; private set; }

    private GameState stateBeforeSettings;
    private bool settingsOpen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnterMenu();
    }

    private void Update()
    {
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

        if (settingsOpen) { CloseSettings(); return; }
        if (State == GameState.Playing) PauseGame();
        else if (State == GameState.Paused) ResumeGame();
    }

    // --- Переходы состояний ---

    public void StartGame()
    {
        CloseAllMenus();
        SetPlaying();
    }

    public void PauseGame()
    {
        CloseAllMenus();
        pauseMenu.SetActive(true);
        playerLook.enabled = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        State = GameState.Paused;
    }

    public void ResumeGame()
    {
        CloseAllMenus();
        SetPlaying();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- Settings ---

    public void OpenSettings()
    {
        stateBeforeSettings = State;
        settingsOpen = true;
        settingsMenu.SetActive(true);

        // PauseMenu остаётся фоном, но его кнопки не должны работать
        if (State == GameState.Paused)
        {
            var raycaster = pauseMenu.GetComponent<GraphicRaycaster>();
            if (raycaster != null) raycaster.enabled = false;
        }
    }

    public void CloseSettings()
    {
        settingsOpen = false;
        settingsMenu.SetActive(false);

        // Восстанавливаем кнопки PauseMenu
        if (stateBeforeSettings == GameState.Paused)
        {
            var raycaster = pauseMenu.GetComponent<GraphicRaycaster>();
            if (raycaster != null) raycaster.enabled = true;
        }
    }

    // --- Утилиты ---

    public void CloseAllMenus()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);

        // Сбрасываем raycaster PauseMenu на случай если был отключён
        var raycaster = pauseMenu.GetComponent<GraphicRaycaster>();
        if (raycaster != null) raycaster.enabled = true;

        settingsOpen = false;
    }

    private void EnterMenu()
    {
        CloseAllMenus();
        mainMenu.SetActive(true);
        playerLook.enabled = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        State = GameState.Menu;
    }

    private void SetPlaying()
    {
        playerLook.enabled = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        State = GameState.Playing;
    }
}
