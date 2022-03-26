using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceCounter : MonoBehaviour
{
    [SerializeField] float percentOfTimeSpentAsExperience = 0.1f;
    float currentExperience = 0f;
    int currentExperienceLevel = 0;
    GameSession gameSession;
    PlayerManager playerManager;
    float timeActive = 0f;
    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        playerManager = gameObject.GetComponent<PlayerManager>();
    }
    private void Update()
    {
        ActiveStatusExperienceCounter();
    }
    private PlayerManager GetPlayerManager()
    {
        if(gameObject.GetComponent<PlayerManager>() == null)
        {
            return gameObject.GetComponent<PlayerManager>();
        }
        else
        {
            return gameObject.transform.parent.GetComponent<PlayerManager>();
        }
    }
    public float GetExperience()
    {
        return currentExperience;
    }
    public int GetExperienceLevel()
    {
        return currentExperienceLevel;
    }
    private void ActiveStatusExperienceCounter()
    {
        if(playerManager.GetMainShipStatus() && gameSession.GameStarted && !PauseMenu.GamePaused)
        {
            timeActive += Time.deltaTime;
            currentExperience += percentOfTimeSpentAsExperience * Time.deltaTime;
        }
    }
    public float GetActiveTimeInSeconds()
    {
        return timeActive;
    }
    public string GetActiveTimeInMinutesAndSeconds()
    {
        int minutes = Mathf.FloorToInt(timeActive/60f);
        float seconds = timeActive % 60f;

        return minutes + ":" + seconds.ToString("F2");
    }
}
