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
    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        gameSession.AllSpawnsPaused = pauseOtherSpawns;
        levelManager = FindObjectOfType<LevelManager>();
        levelList = levelManager.GetLevelList();
        currentLevel = levelList[gameSession.GetLevelToStart()-1];
        playerBindings = FindObjectOfType<PlayerBindings>();
    }
    public void BossDefeated()
    {
        PlayBossVictoryAudio();
        PlayersCompletedRound();
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
}
