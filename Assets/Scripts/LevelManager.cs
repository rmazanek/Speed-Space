using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    List<LevelContainer> levels;
    GameSession gameSession;
    int levelToStart;
    // Start is called before the first frame update
    private void Awake()
    {
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
    void Start()
    {
        //UpdateLevelCharacteristics();
    }
    public void UpdateLevelCharacteristics()
    {
        gameSession = FindObjectOfType<GameSession>();
        levelToStart = gameSession.GetLevelToStart();
        levels = GetLevelList();
        //Debug.Log("levelManager knows levelToStart is " + levelToStart);
        ActivateLevel(levelToStart);
    }
    public List<LevelContainer> GetLevelList()
    {
        List<LevelContainer> levelList = new List<LevelContainer>();

        foreach(LevelContainer levelContainer in gameObject.transform.GetComponentsInChildren<LevelContainer>())
        {
            levelList.Add(levelContainer);
        }
        
        List<LevelContainer> sortedLevelList = new List<LevelContainer>();
        sortedLevelList = levelList.OrderBy(o=>o.LevelIndex).ToList();

        return levelList;
    }
    private void ActivateLevel(int levelToStart)
    {
        //Debug.Log("GameSession wants level " + levelToStart);
        //Debug.Log(levels.Count + " levels available");
        for(int index = 0; index < levels.Count; index++)
        {
            //Debug.Log("Checking level " + index + " with name: " + levels[index].gameObject.name + " and LevelIndex " + levels[index].LevelIndex);
            if(levels[index].LevelIndex == levelToStart)
            {
                levels[index].GetComponent<LevelContainer>().StartLevel();
                //Debug.Log(levels[index].name + " started!");
            }
        }
    }
}
