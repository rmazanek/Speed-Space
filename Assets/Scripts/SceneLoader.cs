using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Start Menu")]
    [SerializeField] float startMenuTransitionPeriod = 0.5f;

    [Header("Game Over")]
    [SerializeField] AudioClip gameOverSoundClip;
    [SerializeField] float gameOverSoundVolume = 0.075f;
    [SerializeField] float gameOverTransitionPeriod = 6f;
    [Header("New Game")]
    [SerializeField] float newGameTransitionPeriod = 0.5f;
    [Header("Generic Next Scene")]
    [SerializeField] float nextSceneTransitionPeriod = 0.5f;
    [Header("Level Success")]
    [SerializeField] AudioClip levelSuccessSoundClip;
    [SerializeField] float levelSuccessSoundVolume = 0.075f;
    [SerializeField] float levelSuccessTransitionPeriod = 5.5f;
    [SerializeField] AudioClip warpSoundClip;
    [SerializeField] float warpSoundVolume = 0.075f;
    AudioSource backgroundMusicSource;
    GameSession gameSession;
    PlayerControls playerControls;
    PlayerInput playerInput;
    PlayerBindings playerBindings;
    MenuBindings menuBindings;
    MapBindings mapBindings;
    WormholeScaler wormholeScaler;
    ParentPositionMover parentPositionMover;
    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        playerInput = FindObjectOfType<PlayerInput>();
        playerBindings = FindObjectOfType<PlayerBindings>();
        menuBindings = FindObjectOfType<MenuBindings>();
        mapBindings = FindObjectOfType<MapBindings>();
    }
    public void SetPlayerControls(PlayerControls controls)
    {
        playerControls = controls;
    }
    public void LoadNextScene()
    {
        StartCoroutine(LoadNextSceneCoroutine());
    }
    IEnumerator LoadNextSceneCoroutine()
    {
        yield return new WaitForSeconds(nextSceneTransitionPeriod);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SwitchControlsUIToGameplay();
    }
    public void LoadLevelEndScene()
    {
        StartCoroutine(LoadLevelEndSceneCoroutine());
    }
    IEnumerator LoadLevelEndSceneCoroutine()
    {
        backgroundMusicSource = GameObject.FindGameObjectWithTag("Background Music").GetComponent<AudioSource>();
        backgroundMusicSource.volume = 0f;

        AudioSource.PlayClipAtPoint(levelSuccessSoundClip, Camera.main.transform.position, levelSuccessSoundVolume);
        wormholeScaler = FindObjectOfType<WormholeScaler>();
        wormholeScaler.WormholeTransition();
        yield return new WaitForSeconds(levelSuccessTransitionPeriod);
        AudioSource.PlayClipAtPoint(warpSoundClip, Camera.main.transform.position, warpSoundVolume);
        yield return new WaitForSeconds(nextSceneTransitionPeriod);
        SceneManager.LoadScene("Level Recap");
        playerInput.SwitchCurrentActionMap("Map");
        SwitchControlsGameplayToMap();
        mapBindings = FindObjectOfType<MapBindings>();
        mapBindings.InventoryDisableNeeded();
    }
    public void LoadStartMenu()
    {
        StartCoroutine(LoadStartMenuCoroutine());
    }
    IEnumerator LoadStartMenuCoroutine()
    {
        yield return new WaitForSeconds(startMenuTransitionPeriod);
        LoadStartMenuImmediate();
    }
    public void LoadStartMenuImmediate()
    {
        StartCoroutine(LoadStartMenuImmediateCoroutine());
    }
    IEnumerator LoadStartMenuImmediateCoroutine()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.02f);
        gameSession.ResetGame();
        SceneManager.LoadScene("Start Menu");
        SwitchControlsGameplayToUI();  
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadNewGame()
    {
        StartCoroutine(LoadNewGameCoroutine());
    }
    IEnumerator LoadNewGameCoroutine()
    {
        yield return new WaitForSeconds(newGameTransitionPeriod);
        gameSession.ResetGame();
        SceneManager.LoadScene("Level 1");
        SwitchControlsUIToGameplay();
    }
    public void LoadLevelCanvasFromMap()
    {
        StartCoroutine(LoadLevelCanvasFromMapCoroutine());
    }
    IEnumerator LoadLevelCanvasFromMapCoroutine()
    {
        yield return new WaitForSeconds(newGameTransitionPeriod);
        parentPositionMover = FindObjectOfType<ParentPositionMover>(true);
        //Debug.Log(parentPositionMover.name + " with " + parentPositionMover.transform.childCount + " children found (even from inactive) and set to active.");
        parentPositionMover.gameObject.SetActive(true);
        SceneManager.LoadScene("Level 1");
        SwitchControlsMapToGameplay();
        gameSession = FindObjectOfType<GameSession>();
        gameSession.InitializeLevel();
    }
    public void LoadGameOver()
    {
        StartCoroutine(LoadGameOverCoroutine());
    }
    IEnumerator LoadGameOverCoroutine()
    {
        backgroundMusicSource = GameObject.FindGameObjectWithTag("Background Music").GetComponent<AudioSource>();
        backgroundMusicSource.volume = 0f;

        AudioSource.PlayClipAtPoint(gameOverSoundClip, Camera.main.transform.position, gameOverSoundVolume);
        yield return new WaitForSeconds(gameOverTransitionPeriod);
        SceneManager.LoadScene("Game Over");
        playerBindings.DisableShipControls();
        playerInput.SwitchCurrentActionMap("UI");
        menuBindings.EnableUIControls();
    }
    public void SwitchControlsUIToGameplay()
    {
        menuBindings.DisableUIControls();
        playerInput.SwitchCurrentActionMap("Gameplay");
        playerBindings.InitializeShipControls();
    }
    public void SwitchControlsMapToGameplay()
    {
        playerInput.actions.FindActionMap("Map").Disable();
        mapBindings.DisableMapControls();
        playerInput.SwitchCurrentActionMap("Gameplay");
        playerBindings.InitializeShipControls();
    }
    public void SwitchControlsGameplayToUI()
    {
        playerBindings.DisableShipControls();
        playerInput.SwitchCurrentActionMap("UI");
        menuBindings.EnableUIControls();
    }
    public void SwitchControlsGameplayToMap()
    {
        playerBindings.DisableShipControls();
        playerInput.SwitchCurrentActionMap("Map");
        mapBindings.EnableMapControls();
    }
    public void SwitchControlsMapToUI()
    {
        playerInput.actions.FindActionMap("Map").Disable();
        mapBindings.DisableMapControls();
        playerInput.SwitchCurrentActionMap("UI");
        menuBindings.EnableUIControls();
    }
    public void SwitchControlsUIToMap()
    {
        playerInput.actions.FindActionMap("UI").Disable();
        mapBindings.EnableMapControls();
        playerInput.SwitchCurrentActionMap("Map");
        mapBindings.EnableMapControls();
    }
}
