using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapMover : MonoBehaviour
{
  List<LevelIcon> levelIcons;
  GameSession gameSession;
  PlayerControls controls;
  int lastLevelPlayed = 1;
  int newIconIndex;
  [SerializeField] float maxDistanceDelta = 10f;
  float moveX;
  void Start()
  {
    levelIcons = GetLevelIcons();
    gameSession = FindObjectOfType<GameSession>();
    lastLevelPlayed = gameSession.GetLevelToStart();
    SetNewIconIndex(lastLevelPlayed - 1);
    LevelIcon currentLevelIcon = levelIcons[newIconIndex];
    gameObject.transform.position = currentLevelIcon.transform.position;
    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -2f);
  }
  public void NavigateMap(float moveX)
  {
    if (moveX > 0)
    {
      SetNewIconIndex(++newIconIndex);
      MoveToNextIcon();
    }
    else if (moveX < 0)
    {
      SetNewIconIndex(--newIconIndex);
      MoveToNextIcon();
    }
  }
  private void MoveToNextIcon()
  {
    StartCoroutine(MoveToNextIconCoroutine());
  }
  IEnumerator MoveToNextIconCoroutine()
  {
    while (gameObject.transform.position != levelIcons[newIconIndex].transform.position)
    {
      gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, levelIcons[newIconIndex].transform.position, maxDistanceDelta * Time.deltaTime);
    }
    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -2f);
    yield return null;
  }
  public void SetNewIconIndex(int value)
  {
    gameSession = FindObjectOfType<GameSession>();
    newIconIndex = Mathf.Clamp(value, 0, gameSession.GetUnlockedLevelCount() - 1);
  }
  private List<LevelIcon> GetLevelIcons()
  {
    List<LevelIcon> levelIconsUnsorted = new List<LevelIcon>();

    foreach (LevelIcon levelIcon in FindObjectsOfType<LevelIcon>())
    {
      levelIconsUnsorted.Add(levelIcon);
    }

    List<LevelIcon> levelIconsSorted = new List<LevelIcon>();
    levelIconsSorted = levelIconsUnsorted.OrderBy(o => o.LevelIconIndex).ToList();
    return levelIconsSorted;
  }
}
