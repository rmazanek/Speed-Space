using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GameSession : MonoBehaviour
{
  [Header("Modifier Graphics")]
  [SerializeField] public Sprite FirePeriodReductionSprite;
  [SerializeField] public Sprite DamageMultiplierSprite;
  [SerializeField] public Sprite ProjectileSpeedMultiplierSprite;
  [SerializeField] public Sprite MaxHealthSprite;
  [SerializeField] public Sprite MaxShieldHealthSprite;
  [SerializeField] public Sprite ShieldRegenSprite;
  [SerializeField] public Sprite ShieldCooldownSprite;
  [SerializeField] public Sprite WorkerSprite;
  [Header("Extract Graphics")]
  [SerializeField] GameObject extractCanvasPrefab;
  private int gameScore = 0;
  [SerializeField] public float DropSpeed { get; set; } = 4f;
  ScoreDisplay gameScoreDisplay;
  PlayerBindings playerBindings;
  PlayerControls controls;
  ParentPositionMover parentPositionMover;
  EnemySpawner enemySpawner;
  SceneLoader sceneLoader;
  LevelManager levelManager;
  List<LevelContainer> levelList;
  Wallet wallet;
  int randomIndex;
  int numberOfPlayerShips;
  [SerializeField] public static int MaxPlayerShips = 5;
  string gameScoreDisplayText;
  public bool CycleMainShip { get; set; }
  public bool HealthUpdateNeeded { get; set; } = false;
  public KeyCode rotateShipsKeyCode { get; set; }
  public float PlusLuckModifier { get; set; }
  public bool ExtractAvailable { get; set; } = false;
  public bool GameStarted { get; set; }
  public bool AllSpawnsPaused = false;
  int levelCount;
  int levelToStart;
  bool initializeLevelNeeded;
  public bool HidePlayers { get; set; }
  public static float GameTimeScale;
  public float SlowTimeScale { get { return slowTimeScale; } private set { slowTimeScale = value; } }
  [SerializeField] float slowTimeScale = 0.25f;
  public LevelContainer CurrentLevel;
  WalletDisplay walletDisplay;
  public bool FinalLevelEnabled = true;
  public bool FinalLevelRevealed = false;

  private void Awake()
  {
    levelToStart = 1;

    if (FindObjectsOfType(GetType()).Length > 1)
    {
      gameObject.SetActive(false);
      Destroy(gameObject);
    }
    else
    {
      DontDestroyOnLoad(gameObject);
    }
  }
  // Start is called before the first frame update
  void Start()
  {
    gameScore = 0;
    GameTimeScale = Time.timeScale;
    InitializeLevel();
  }
  private void Update()
  {
    if (initializeLevelNeeded)
    {
      InitializeLevelActivities();
    }
  }
  public void InitializeLevel()
  {
    initializeLevelNeeded = true;
  }
  private void OnEnable()
  {
    playerBindings = FindObjectOfType<PlayerBindings>();
    playerBindings.SetGameSession(this);
    playerBindings.Players = GameObject.Find("Players");
    playerBindings.Players.SetActive(true);
  }
  public void InitializeLevelActivities()
  {
    initializeLevelNeeded = false;
    sceneLoader = FindObjectOfType<SceneLoader>();
    GameStarted = true;
    randomIndex = 0;
    gameScoreDisplay = FindObjectOfType<ScoreDisplay>();
    playerBindings.HealthDisplayPublic = FindObjectOfType<HealthDisplay>();
    walletDisplay = FindObjectOfType<WalletDisplay>();
    enemySpawner = FindObjectOfType<EnemySpawner>();
    levelManager = FindObjectOfType<LevelManager>();
    controls = playerBindings.GetPlayerControls();
    levelList = levelManager.GetLevelList();
    levelManager.UpdateLevelCharacteristics();
    CurrentLevel = levelList[levelToStart - 1];
    ExtractAvailable = CurrentLevel.ExtractAvailable();
    InstantiateExtract();
    playerBindings.PlayerShips = playerBindings.GetPlayerShips();
    CycleMainShip = false;
    playerBindings.SetNewMainShip();
    HealthUpdateNeeded = true;
    rotateShipsKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rotateKey", "LeftShift"));
    InitializeShakeCamera(playerBindings.PlayerShips);
    AllSpawnsPaused = false;
  }
  private void InitializeShakeCamera(List<Player> players)
  {
    foreach (Player player in players)
    {
      player.InitializeShakeCamera();
    }
  }
  public void ChangeTimeScaleBackToOneAfterSeconds(float timeInSeconds)
  {
    StartCoroutine(ChangeTimeScaleBackToOneAfterSecondsCoroutine(timeInSeconds));
  }
  IEnumerator ChangeTimeScaleBackToOneAfterSecondsCoroutine(float timeInSeconds)
  {
    yield return new WaitForSecondsRealtime(timeInSeconds);
    Time.timeScale = 1f;
  }
  public int GetLevelToStart()
  {
    return levelToStart;
  }
  public void SetLevelToStart(int levelNumber)
  {
    levelToStart = levelNumber;
  }
  public int GetLevelCount()
  {
    return levelCount;
  }
  public int GetUnlockedLevelCount()
  {
    return levelList.Where(a => a.ExtractAvailable()).Count() + 1;
  }
  public void CountLevels()
  {
    levelCount++;
  }
  public void InstantiateExtract()
  {
    Vector3 bottomRight = new Vector3(0.88f, 0.08f, 10f);

    if (ExtractAvailable & GameObject.FindGameObjectWithTag("ExtractNotification") == null)
    {
      GameObject extractCanvas = Instantiate(extractCanvasPrefab, Camera.main.ViewportToWorldPoint(bottomRight), Quaternion.identity);
      Vector3 extractCanvasPosition = extractCanvas.transform.position;
      extractCanvasPosition = new Vector3(extractCanvasPosition.x, extractCanvasPosition.y, 0f);
    }
  }
  public void AddToScore(int score)
  {
    gameScore += score;
    gameScoreDisplay = FindObjectOfType<ScoreDisplay>();
    gameScoreDisplay.UpdateScore();
  }
  public int GetScore() { return gameScore; }
  public int GetRandomIndex() { return randomIndex; }
  public void ResetGame()
  {
    levelManager = FindObjectOfType<LevelManager>();
    PauseMenu.GamePaused = false;
    Destroy(playerBindings.ParentPositionMover.gameObject);
    Destroy(levelManager.gameObject);
    Destroy(gameObject);
  }
  public void AddToRandomIndex()
  {
    randomIndex++;
  }
  public void ResetRandomIndex()
  {
    randomIndex = 0;
  }
  public void IncrementMaxPlayerShips()
  {
    MaxPlayerShips += 1;
  }
}