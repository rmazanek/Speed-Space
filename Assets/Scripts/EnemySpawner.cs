using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
  // Wave Configs
  [SerializeField] List<WaveConfig> waveConfigs;
  [SerializeField] int startingWave = 0;
  [SerializeField] bool looping = false;
  public List<GameObject> RandomPaths;
  GameSession gameSession;
  LevelManager levelManager;
  List<LevelContainer> levelList;
  LevelContainer currentLevel;
  PlayerBindings playerBindings;
  //List<GameObject> EnemiesPausingSpawn;
  private void Awake()
  {
    InstantiateRandomPaths();
    gameSession = FindObjectOfType<GameSession>();
    playerBindings = FindObjectOfType<PlayerBindings>();
    levelManager = FindObjectOfType<LevelManager>();
    levelList = levelManager.GetLevelList();
    //EnemiesPausingSpawn = new List<GameObject>();
  }
  public void InstantiateRandomPaths()
  {
    for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
    {
      var instantiatedPath = waveConfigs[waveIndex].InstantiatePathPrefab(waveIndex);
      if (instantiatedPath != null)
      {
        RandomPaths.Add(instantiatedPath);
        //Debug.Log("Instantiated random path named: " + instantiatedPath.name);
      }
    }
  }
  // Start is called before the first frame update
  IEnumerator Start()
  {
    do
    {
      yield return StartCoroutine(SpawnAllWaves());
      gameSession.ResetRandomIndex();
    }
    while (looping);
  }
  //private void Update()
  //{
  //    if(EnemiesPausingSpawn.Where(o => o != null).ToList().Count == 0)
  //    {
  //        gameSession.AllSpawnsPaused = false;
  //    }
  //}

  private IEnumerator SpawnAllWaves()
  {
    for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
    {
      var currentWave = waveConfigs[waveIndex];
      yield return SpawnAllEnemiesInWave(currentWave);

      if (currentWave.PathIsRandom())
      {
        gameSession.AddToRandomIndex();
      }
    }
  }

  private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
  {
    int enemiesToSpawn = NumberOfEnemiesToSpawn(waveConfig);

    for (int enemyCount = 0; enemyCount < enemiesToSpawn; enemyCount++)
    {
      //var newEnemy = Instantiate(
      //                waveConfig.GetEnemyPrefab(),
      //                waveConfig.GetWaypoints()[0].transform.position,
      //                Quaternion.identity);

      var newEnemy = Instantiate(
                      waveConfig.GetEnemyPrefab(),
                      waveConfig.GetWaypoints()[0].transform.position,
                      waveConfig.GetEnemyPrefab().transform.rotation);

      //Debug.Log(newEnemy.transform.position);
      SetWaveConfig(newEnemy, waveConfig);
      newEnemy.transform.position += newEnemy.GetComponent<EnemyPathing>().WaypointOffset();
      //Debug.Log(newEnemy.transform.position);
      if (waveConfig.EnemyWavePausesOtherSpawns())
      {
        newEnemy.AddComponent<EnemyWavePauser>();
        gameSession.AllSpawnsPaused = true;
      }

      yield return new WaitForSeconds(GetAdjustedTimeBetweenSpawns(waveConfig));
    }

    gameSession = FindObjectOfType<GameSession>();
    //If we want to wait until the current enemies are destroyed
    yield return new WaitUntil(() => !gameSession.AllSpawnsPaused);
  }
  private int NumberOfEnemiesToSpawn(WaveConfig waveConfig)
  {
    if (!waveConfig.AdjustableNumber)
    {
      return 1;
    }
    else
    {
      return GetAdjustedNumberOfEnemies(waveConfig);
    }
  }
  float GetAdjustedTimeBetweenSpawns(WaveConfig waveConfig)
  {
    float adjustedTimeBetweenSpawns;
    currentLevel = levelList[gameSession.GetLevelToStart() - 1];

    adjustedTimeBetweenSpawns = waveConfig.GetTimeBetweenSpawns() * currentLevel.EnemyTimeBetweenSpawnsCumulativeMultiplier;

    return adjustedTimeBetweenSpawns;
  }
  int GetAdjustedNumberOfEnemies(WaveConfig waveConfig)
  {
    int adjustedNumberOfEnemies;
    currentLevel = levelList[gameSession.GetLevelToStart() - 1];

    adjustedNumberOfEnemies = Mathf.FloorToInt(waveConfig.GetNumberOfEnemies() * currentLevel.EnemyNumberCumulativeMultiplier);

    return adjustedNumberOfEnemies;
  }
  private void SetWaveConfig(GameObject objectSpawned, WaveConfig waveConfig)
  {
    if (waveConfig.ChildrenRequirePathingInstructions())
    {
      List<EnemyPathing> enemyPaths = objectSpawned.GetComponentsInChildren<EnemyPathing>().ToList();
      for (int i = 0; i < enemyPaths.Count; i++)
      {
        enemyPaths[i].SetWaveConfig(waveConfig);
        enemyPaths[i].PartOfArrangement = true;
        enemyPaths[i].ArrangementIndex = i;
      }
    }
    else
    {
      objectSpawned.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
    }
  }
}
