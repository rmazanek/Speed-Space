using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class LevelContainer : MonoBehaviour
{
  [Header("Level Characteristics")]
  [SerializeField] int levelIndex = 1;
  [SerializeField] AudioClip backgroundMusic;
  [SerializeField] Material backgroundMaterial;
  [SerializeField] GameObject enemySpawner;
  [SerializeField] bool bossDefeated = false;
  public bool[] LetterCollected = new bool[5];
  [Header("Enemy Modifiers")]
  [SerializeField] float enemyMoveSpeedModifier = 1.25f;
  [SerializeField] float maxMoveSpeedModifier = 5f;
  [SerializeField] float enemyHealthModifier = 1.25f;
  [SerializeField] float enemyMaxTimeBetweenShotsModifier = 0.5f;
  [SerializeField] float enemyDamageModifier = 1.5f;
  [SerializeField] float enemyTimeBetweenSpawnsModifier = 0.80f, minEnemyTimeBetweenSpawnsModifier = 0.10f;
  [SerializeField] float enemyNumberModifier = 1.1f, maxEnemyNumberModifier = 5f;
  [Tooltip("Enter a weight for each modifier above.")]
  [SerializeField] private int[] modifierWeights = new int[6] { 100, 100, 100, 50, 100, 100 };
  [SerializeField] GameObject asteroidFieldPrefab;
  [SerializeField] float secondsBeforeAsteroidSpawn = 1f;
  [Header("Minimums for Modifiers Above")]
  [SerializeField] float minTimeBetweenSpawns = 0.2f;
  public float MinTimeBetweenSpawns { get => minTimeBetweenSpawns; }
  GameObject backgroundMusicGameObject;
  GameObject backgroundMaterialGameObject;
  GameSession gameSession;
  GameObject waveModifierDisplay;
  LetterCollectiblesDisplay letterCollectiblesDisplay;
  private int loopsCompleted = 0;
  public float EnemyMoveSpeedCumulativeMultiplier { get; set; } = 1f; //Applied in EnemyPathing
  public float EnemyHealthCumulativeMultiplier { get; set; } = 1f; //Applied in Enemy
  public float EnemyMaxTimeBetweenShotsCumulativeMultiplier { get; set; } = 1f; //Applied in Enemy
  public float EnemyDamageCumulativeMultiplier { get; set; } = 1f; //Applied in Enemy
  public float EnemyTimeBetweenSpawnsCumulativeMultiplier { get; set; } = 1f; //Applied in EnemySpawner
  public float EnemyNumberCumulativeMultiplier { get; set; } = 1f; //Applied in EnemySpawner
  bool extractAvailable;
  public int LevelIndex { get => levelIndex; }
  private bool levelCharacteristicsNeedUpdate;
  private void Start()
  {
    gameSession = FindObjectOfType<GameSession>();
    gameSession.CountLevels();
  }
  private void Update()
  {
    if (levelCharacteristicsNeedUpdate)
    {
      levelCharacteristicsNeedUpdate = false;
      backgroundMusicGameObject = GameObject.Find("Background Music");
      backgroundMaterialGameObject = GameObject.Find("Background_Stars");
      ChangeBackground();
      ChangeMusic();
      InstantiateEnemySpawner();
      GameObject.FindObjectOfType<ParentPositionMover>().ShipPositionsNeedUpdate = true;
      ColorCollectibleDisplay();
    }
  }
  public void StartLevel()
  {
    levelCharacteristicsNeedUpdate = true;
  }
  private void ChangeBackground()
  {
    //Debug.Log(backgroundMaterialGameObject.GetComponent<MeshRenderer>().material);
    //Debug.Log(backgroundMaterial);
    backgroundMaterialGameObject.GetComponent<MeshRenderer>().material = backgroundMaterial;
    backgroundMaterialGameObject.GetComponent<BackgroundScroller>().RestartBackgroundScroll();
  }
  private void ChangeMusic()
  {
    backgroundMusicGameObject.GetComponent<AudioSource>().clip = backgroundMusic;
    backgroundMusicGameObject.GetComponent<AudioSource>().Play();
  }
  private void InstantiateEnemySpawner()
  {
    Instantiate(enemySpawner, transform.position, Quaternion.identity);
  }
  public bool ExtractAvailable()
  {
    return bossDefeated;
  }
  public void SetBossDefeated()
  {
    bossDefeated = true;
    gameSession = FindObjectOfType<GameSession>();
    //Debug.Log("This is where it can't find an instance, but the name is " + gameSession.name);
    gameSession.ExtractAvailable = true;
    gameSession.InstantiateExtract();
  }
  private void ColorCollectibleDisplay()
  {
    letterCollectiblesDisplay = FindObjectOfType<LetterCollectiblesDisplay>();
    for (int i = 0; i < LetterCollected.Length; i++)
    {
      if (LetterCollected[i])
      {
        letterCollectiblesDisplay.ColorInSprite(i);
      }
    }
  }
  private int GetTotalWeight()
  {
    int totalWeight = 0;

    foreach (int w in modifierWeights)
    {
      totalWeight += w;
    }

    return totalWeight;
  }
  private List<Action> GetListOfFunctions()
  {
    List<Action> modifierFunctions = new List<Action>();
    modifierFunctions.Add(EnemyMoveSpeedModifier);
    modifierFunctions.Add(EnemyHealthModifier);
    modifierFunctions.Add(EnemyMaxTimeBetweenShotsModifier);
    modifierFunctions.Add(EnemyDamageModifier);
    modifierFunctions.Add(EnemyTimeBetweenSpawnsModifier);
    modifierFunctions.Add(EnemyNumberModifier);


    //modifierFunctions.Add("EnemyMoveSpeedModifier");
    //modifierFunctions.Add("EnemyHealthModifier");
    //modifierFunctions.Add("EnemyMaxTimeBetweenShotsModifier");
    //modifierFunctions.Add("EnemyDamageModifier");
    //modifierFunctions.Add("EnemyTimeBetweenSpawnsModifier");
    //modifierFunctions.Add("EnemyNumberModifier");

    return modifierFunctions;
  }
  private Action GetRandomModifierMethod(int rand)
  {
    for (int index = 0; index < modifierWeights.Length; index++)
    {
      if (rand <= modifierWeights[index])
      {
        return GetListOfFunctions()[index];
      }
      else
      {
        rand -= modifierWeights[index];
        continue;
      }
    }

    return null;
  }
  public void InvokeRandomModifier()
  {
    int rand = UnityEngine.Random.Range(0, GetTotalWeight());
    GetRandomModifierMethod(rand)();
    //Invoke(GetRandomModifierMethodName(rand), secondsBeforeModifierApplied);
  }
  private void EnemyMoveSpeedModifier()
  {
    DisplayMessage("Enemy move speed increased!");
    EnemyMoveSpeedCumulativeMultiplier = Mathf.Min(maxMoveSpeedModifier, EnemyMoveSpeedCumulativeMultiplier * enemyMoveSpeedModifier);
  }
  private void EnemyHealthModifier()
  {
    DisplayMessage("Enemy health increased!");
    EnemyHealthCumulativeMultiplier = EnemyHealthCumulativeMultiplier * enemyHealthModifier;
  }
  private void EnemyMaxTimeBetweenShotsModifier()
  {
    DisplayMessage("Enemies shoot faster!");
    EnemyMaxTimeBetweenShotsCumulativeMultiplier = EnemyMaxTimeBetweenShotsCumulativeMultiplier * enemyMaxTimeBetweenShotsModifier;
  }
  private void EnemyDamageModifier()
  {
    DisplayMessage("Enemy damage increased!");
    EnemyDamageCumulativeMultiplier = EnemyDamageCumulativeMultiplier * enemyDamageModifier;
  }
  private void EnemyTimeBetweenSpawnsModifier()
  {
    DisplayMessage("Enemies spawn faster!");
    EnemyTimeBetweenSpawnsCumulativeMultiplier = Mathf.Max(minEnemyTimeBetweenSpawnsModifier, EnemyTimeBetweenSpawnsCumulativeMultiplier * enemyTimeBetweenSpawnsModifier);
  }
  private void EnemyNumberModifier()
  {
    DisplayMessage("Enemy count increased!");
    EnemyNumberCumulativeMultiplier = Mathf.Min(maxEnemyNumberModifier, EnemyNumberCumulativeMultiplier * enemyNumberModifier);
  }
  public void SpawnAsteroidField()
  {
    StartCoroutine(SpawnAsteroidFieldCoroutine());
  }
  IEnumerator SpawnAsteroidFieldCoroutine()
  {
    yield return new WaitForSeconds(secondsBeforeAsteroidSpawn);
    DisplayMessage("Asteroid field incoming!");
  }
  public void LoopCount()
  {
    loopsCompleted++;
  }
  public int GetLoopsCompleted()
  {
    return loopsCompleted;
  }
  private void DisplayMessage(string message)
  {
    waveModifierDisplay = GameObject.FindGameObjectWithTag("Modifier Message");
    waveModifierDisplay.GetComponent<TextMeshProUGUI>().text = message;
    waveModifierDisplay.GetComponent<Animator>().Play("Base Layer.OpacityInOut");
  }
  public void AwardLetter(int index)
  {
    LetterCollected[index] = true;
    letterCollectiblesDisplay = FindObjectOfType<LetterCollectiblesDisplay>();
    letterCollectiblesDisplay.ColorInSprite(index);
  }
}
