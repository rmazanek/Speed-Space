using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapManager : MonoBehaviour
{
  GameSession gameSession;
  SceneLoader sceneLoader;
  public int LevelIndex { get; set; }
  public int LevelIconCount { get; set; }
  public bool OnMapIcon = false;
  PlayerControls controls;
  void Start()
  {
    gameSession = FindObjectOfType<GameSession>();
    gameSession.GameStarted = false;
    sceneLoader = FindObjectOfType<SceneLoader>();
    AssignLevelIconIndex();
  }
  private List<LevelIcon> GetLevelIcons()
  {
    List<LevelIcon> levelIconsUnsorted = new List<LevelIcon>();

    foreach (LevelIcon levelIcon in FindObjectsOfType<LevelIcon>())
    {
      levelIconsUnsorted.Add(levelIcon);
    }

    List<LevelIcon> levelIconsSorted = new List<LevelIcon>();
    levelIconsSorted = levelIconsUnsorted.OrderBy(o => o.transform.position.x).ToList();
    return levelIconsSorted;
  }
  private void AssignLevelIconIndex()
  {
    int index = 0;
    List<LevelIcon> levelIconList = GetLevelIcons();
    foreach (LevelIcon level in levelIconList)
    {
      level.LevelIconIndex = index + 1;
      index++;
    }
  }
  public void StartSelectedLevel()
  {
    gameSession.SetLevelToStart(LevelIndex);
    sceneLoader.LoadLevelCanvasFromMap();
  }
}
