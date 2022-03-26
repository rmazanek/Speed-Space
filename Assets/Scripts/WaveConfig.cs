using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Wave Config")]
public class WaveConfig : ScriptableObject
{
  [SerializeField] GameObject enemyPrefab;
  [SerializeField] GameObject pathPrefab;
  [SerializeField] bool randomPath = false;
  [SerializeField] float timeBetweenSpawns = 0.5f;
  [SerializeField] float spawnRandomFactor = 0.3f;
  [SerializeField] int numberOfEnemies = 5;
  [SerializeField] float moveSpeed = 2f;
  public bool AdjustableNumber = true;
  GameObject path;
  GameSession gameSession;
  EnemySpawner enemySpawner;
  LevelManager levelManager;
  LevelContainer currentLevel;
  List<LevelContainer> levelList;
  [SerializeField] bool enemyWavePausesOtherSpawns = false;
  [SerializeField] bool childrenRequirePathingInstructions = false;
  private void Start()
  {
    gameSession = FindObjectOfType<GameSession>();
    levelManager = FindObjectOfType<LevelManager>();
    levelList = levelManager.GetLevelList();
    currentLevel = levelList[gameSession.GetLevelToStart() - 1];
    if (enemyPrefab.tag != "Shop")
    {
      moveSpeed = moveSpeed * currentLevel.EnemyMoveSpeedCumulativeMultiplier;
      timeBetweenSpawns = Mathf.Max(timeBetweenSpawns * currentLevel.EnemyTimeBetweenSpawnsCumulativeMultiplier, currentLevel.MinTimeBetweenSpawns);
      numberOfEnemies = Mathf.RoundToInt(numberOfEnemies * currentLevel.EnemyNumberCumulativeMultiplier);
    }
  }
  public bool EnemyWavePausesOtherSpawns()
  {
    return enemyWavePausesOtherSpawns;
  }
  public bool PathIsRandom()
  {
    return randomPath;
  }
  public GameObject GetEnemyPrefab() { return enemyPrefab; }
  public List<Transform> GetWaypoints()
  {
    enemySpawner = FindObjectOfType<EnemySpawner>();
    gameSession = FindObjectOfType<GameSession>();
    var waypoints = new List<Transform>();
    var randomPaths = new List<GameObject>();
    randomPaths = enemySpawner.RandomPaths;
    //Debug.Log("there are random paths found: " + randomPaths.Count);

    if (randomPath)
    {
      //Debug.Log("we're going random for this path");
      enemySpawner = FindObjectOfType<EnemySpawner>();
      gameSession = FindObjectOfType<GameSession>();
      //Debug.Log("We're trying to find random path with random index " + gameSession.GetRandomIndex() + " and there are " + enemySpawner.RandomPaths.Count + " enemy random paths");
      //Debug.Log("The path has " + enemySpawner.RandomPaths[gameSession.GetRandomIndex()].GetComponent<PathConfig>().Waypoints.Count + " nodes");
      waypoints = enemySpawner.RandomPaths[gameSession.GetRandomIndex()].GetComponent<PathConfig>().Waypoints;
    }
    else
    {
      foreach (Transform p in pathPrefab.transform)
      {
        waypoints.Add(p);
      }
    }
    //Debug.Log(waypoints[0].name);
    //Debug.Log(waypoints[1].name);
    return waypoints;
  }
  public GameObject InstantiatePathPrefab(int number)
  {
    if (randomPath)
    {
      var position = new Vector3(0, 0, 0);
      GameObject instantiatedPath = Instantiate(pathPrefab, position, Quaternion.identity) as GameObject;
      instantiatedPath.name = "Path (Random) " + number;
      return instantiatedPath;
    }
    else
    {
      return null;
    }
  }
  public float GetTimeBetweenSpawns() { return timeBetweenSpawns; }
  public float GetSpawnRandomFactor() { return spawnRandomFactor; }
  public int GetNumberOfEnemies() { return numberOfEnemies; }
  public float GetMoveSpeed() { return moveSpeed; }
  public bool ChildrenRequirePathingInstructions() { return childrenRequirePathingInstructions; }
}
