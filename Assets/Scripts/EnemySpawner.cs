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
    private void Awake()
    {
        InstantiateRandomPaths();
        gameSession = FindObjectOfType<GameSession>();
        playerBindings = FindObjectOfType<PlayerBindings>();
        levelManager = FindObjectOfType<LevelManager>();
        levelList = levelManager.GetLevelList();
    }
    public void InstantiateRandomPaths()
    {
        for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
        {
            var instantiatedPath = waveConfigs[waveIndex].InstantiatePathPrefab(waveIndex);
            if(instantiatedPath != null)
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
    private IEnumerator SpawnAllWaves()
    {   
        for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
        {
            var currentWave = waveConfigs[waveIndex];
            yield return SpawnAllEnemiesInWave(currentWave);
            
            if(currentWave.PathIsRandom())
            {
                gameSession.AddToRandomIndex();
            }
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        int enemiesToSpawn;
        if(waveConfig.GetEnemyPrefab().tag == "Shop")
        {
            enemiesToSpawn = 1;
        }
        else
        {
            enemiesToSpawn = GetAdjustedNumberOfEnemies(waveConfig);
        }

        for(int enemyCount = 0; enemyCount < enemiesToSpawn; enemyCount++)
        {
            var newEnemy = Instantiate(
                            waveConfig.GetEnemyPrefab(),
                            waveConfig.GetWaypoints()[0].transform.position,
                            Quaternion.identity);

            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
            
            yield return new WaitForSeconds(GetAdjustedTimeBetweenSpawns(waveConfig));
        }
        
        gameSession = FindObjectOfType<GameSession>();
        //If we want to wait until the current enemies are destroyed
        yield return new WaitUntil(()=>!gameSession.AllSpawnsPaused);
    }

    float GetAdjustedTimeBetweenSpawns(WaveConfig waveConfig)
    {
        float adjustedTimeBetweenSpawns;
        currentLevel = levelList[gameSession.GetLevelToStart()-1];
        
        adjustedTimeBetweenSpawns = waveConfig.GetTimeBetweenSpawns() * currentLevel.EnemyTimeBetweenSpawnsCumulativeMultiplier;

        return adjustedTimeBetweenSpawns;
    }
    int GetAdjustedNumberOfEnemies(WaveConfig waveConfig)
    {
        int adjustedNumberOfEnemies;
        currentLevel = levelList[gameSession.GetLevelToStart()-1];
        
        adjustedNumberOfEnemies = Mathf.FloorToInt(waveConfig.GetNumberOfEnemies() * currentLevel.EnemyNumberCumulativeMultiplier);

        return adjustedNumberOfEnemies;
    }
}
