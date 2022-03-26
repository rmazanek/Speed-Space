using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDuties : MonoBehaviour
{
  GameSession gameSession;
  [SerializeField] AudioClip playerVictorySound;
  [SerializeField] float playerVictorySoundVolume = 0.075f;
  [SerializeField] bool pauseOtherSpawns = true;
  LevelManager levelManager;
  List<LevelContainer> levelList;
  LevelContainer currentLevel;
  List<Player> playerShips;
  PlayerBindings playerBindings;
  [SerializeField] float timeBetweenEnemiesDestroyed = 0.2f;
  [SerializeField] int bossExplosions = 6;
  [SerializeField] Enemy enemyComponent;
  bool dutiesIncomplete = true;
  // Start is called before the first frame update
  void Start()
  {
    gameSession = FindObjectOfType<GameSession>();
    gameSession.AllSpawnsPaused = pauseOtherSpawns;
    levelManager = FindObjectOfType<LevelManager>();
    levelList = levelManager.GetLevelList();
    currentLevel = levelList[gameSession.GetLevelToStart() - 1];
    playerBindings = FindObjectOfType<PlayerBindings>();
  }
  private void PlayBossVictoryAudio()
  {
    AudioSource.PlayClipAtPoint(playerVictorySound, Camera.main.transform.position, playerVictorySoundVolume);
  }
  private void PlayersCompletedRound()
  {
    playerShips = playerBindings.GetPlayerShips();

    foreach (Player p in playerShips)
    {
      p.AddToRounds();
    }
  }
  public void BossDutiesCheck()
  {
    //gameSession = FindObjectOfType<GameSession>();
    //levelManager = FindObjectOfType<LevelManager>();
    //levelList = levelManager.GetLevelList();
    //currentLevel = levelList[gameSession.GetLevelToStart() - 1];
    currentLevel.LoopCount();
    currentLevel.SetBossDefeated();
    FinalBossDuties();
    StartCoroutine(ExplodeBoss(enemyComponent.ExplosionDuration));
  }
  private void FinalBossDuties()
  {
    if (FindObjectsOfType<BossDuties>().Length <= 1 && dutiesIncomplete)
    {
      dutiesIncomplete = false;
      SlowTimeUntilPaused();
      DestroyRemainingEnemiesWithoutReward();
      BossDefeated();
      ModifyEnemies();
      gameSession.AllSpawnsPaused = false;
      //gameSession.PlayWinSong();
    }
  }
  void SlowTimeUntilPaused()
  {
    while (Time.timeScale > gameSession.SlowTimeScale)
    {
      Time.timeScale *= 0.90f;
    }
  }
  void DestroyRemainingEnemiesWithoutReward()
  {
    Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
    gameSession.ChangeTimeScaleBackToOneAfterSeconds(enemies.Length * timeBetweenEnemiesDestroyed);

    for (int i = enemies.Length - 1; i >= 0; i--)
    {
      if (enemies[i] != this)
      {
        enemies[i].DestroyAfterTimeWithoutReward(i * timeBetweenEnemiesDestroyed);
      }
    }
  }
  public void BossDefeated()
  {
    PlayBossVictoryAudio();
    PlayersCompletedRound();
  }
  private void ModifyEnemies()
  {
    LevelManager levelManager = FindObjectOfType<LevelManager>();
    List<LevelContainer> levelList = levelManager.GetLevelList();
    LevelContainer currentLevel = levelList[gameSession.GetLevelToStart() - 1];
    currentLevel.InvokeRandomModifier();
  }
  IEnumerator ExplodeBoss(float duration)
  {
    for (int i = 0; i < bossExplosions; i++)
    {
      StartCoroutine(ExplodeBossCoroutine(i, duration));
    }
    yield return new WaitForSecondsRealtime((bossExplosions - 2) * duration / 2);
    Destroy(gameObject);
  }
  IEnumerator ExplodeBossCoroutine(int timeMultiple, float duration)
  {
    yield return new WaitForSecondsRealtime(timeMultiple * duration / 2);
    enemyComponent.PlayExplosion(GetRandomPositionOnBoss());
    Debug.Log("Boss explosion!");
  }
  Vector3 GetRandomPositionOnBoss()
  {
    Vector3 bossExtents = gameObject.GetComponent<SpriteRenderer>().bounds.extents;
    Vector3 randomPosition = transform.position + (Vector3)Random.insideUnitCircle * 3f;
    float clampedX = Mathf.Clamp(randomPosition.x, -bossExtents.x, bossExtents.x);
    float clampedY = Mathf.Clamp(randomPosition.y, -bossExtents.y, bossExtents.y);
    return new Vector3(clampedX, clampedY, transform.position.z);
  }
}
