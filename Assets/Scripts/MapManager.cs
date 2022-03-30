using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapManager : MonoBehaviour
{
  [Header("Final Level")]
  [SerializeField] GameObject finalLevel;
  [SerializeField] AudioClip finalLevelRevealAnticipationMusic;
  [SerializeField] float finalLevelRevealAnticipationMusicVolume = 0.075f;
  [SerializeField] float finalLevelRevealAnticipationDuration = 3f;
  [SerializeField] AudioClip finalLevelRevealSound;
  [SerializeField] float finalLevelRevealSoundVolume = 0.075f;
  [SerializeField] GameObject finalLevelRevealVfx;
  [SerializeField] float finalLevelRevealVfxDuration = 1f;
  [SerializeField] AudioSource backgroundMusicSource;
  GameSession gameSession;
  SceneLoader sceneLoader;
  public int LevelIndex { get; set; }
  public int LevelIconCount { get; set; }
  public bool OnMapIcon = false;
  PlayerControls controls;
  float startingMusicVolume;
  void Start()
  {
    gameSession = FindObjectOfType<GameSession>();
    gameSession.GameStarted = false;
    sceneLoader = FindObjectOfType<SceneLoader>();
    AssignLevelIconIndex();
    startingMusicVolume = backgroundMusicSource.volume;
    CheckFinalLevelRequirements();
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
  IEnumerator UnlockFinalLevel()
  {
    gameSession.FinalLevelRevealed = true;
    backgroundMusicSource.Pause();
    backgroundMusicSource.volume = 0f;
    AudioSource.PlayClipAtPoint(finalLevelRevealAnticipationMusic, Camera.main.transform.position, finalLevelRevealAnticipationMusicVolume);
    yield return new WaitForSeconds(finalLevelRevealAnticipationDuration);

    finalLevel.SetActive(true);
    finalLevel.GetComponentInChildren<LevelIcon>().LevelIconIndex = FindObjectsOfType<LevelIcon>().Length;
    AudioSource.PlayClipAtPoint(finalLevelRevealSound, Camera.main.transform.position, finalLevelRevealSoundVolume);
    GameObject revealVfx = Instantiate(finalLevelRevealVfx, finalLevel.transform.position, Quaternion.identity);
    yield return new WaitForSeconds(finalLevelRevealVfxDuration);

    backgroundMusicSource.UnPause();
    RampUpBackgroundVolume();
  }
  public void CheckFinalLevelRequirements()
  {
    if (gameSession.FinalLevelRevealed)
    {
      finalLevel.SetActive(true);
      finalLevel.GetComponentInChildren<LevelIcon>().LevelIconIndex = FindObjectsOfType<LevelIcon>().Length;
    }
    if (gameSession.FinalLevelEnabled && !gameSession.FinalLevelRevealed)
    {
      StartCoroutine(UnlockFinalLevel());
    }
  }
  IEnumerator RampUpBackgroundVolume()
  {
    while (backgroundMusicSource.volume < startingMusicVolume)
    {
      backgroundMusicSource.volume += 0.001f;
      yield return new WaitForEndOfFrame();
    }
  }
}
