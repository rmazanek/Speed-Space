using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class LetterItem : MonoBehaviour
{
  [SerializeField] TextMeshPro textMeshPro;
  [SerializeField] string collectibleString = "space";
  [SerializeField] SpriteRenderer backgroundSpriteRenderer;
  [SerializeField] Sprite alreadyCollectedSprite;
  [SerializeField] float alreadyCollectedAlpha = 83f;
  [SerializeField] AudioClip alreadyCollectedPowerUpSound;
  [SerializeField] AudioClip powerUpSound;
  [SerializeField] float powerUpSoundVolume = 0.05f;
  GameSession gameSession;
  public int LetterIndex;
  Player player;
  LevelContainer currentLevel;
  void Start()
  {
    gameSession = FindObjectOfType<GameSession>();
    LetterIndex = GetLetterIndex();
    collectibleString = collectibleString.ToLower();
    currentLevel = gameSession.CurrentLevel;
    if (gameSession.CurrentLevel.LetterCollected[LetterIndex])
    {
      CollectedStatusAdjustments();
    }
  }
  private void CollectedStatusAdjustments()
  {
    backgroundSpriteRenderer.sprite = alreadyCollectedSprite;
    backgroundSpriteRenderer.color = new Color(backgroundSpriteRenderer.color.r, backgroundSpriteRenderer.color.g, backgroundSpriteRenderer.color.b, alreadyCollectedAlpha);
    powerUpSound = alreadyCollectedPowerUpSound;
  }
  private int GetLetterIndex()
  {
    for (int i = 0; i < collectibleString.Length; i++)
    {
      if (textMeshPro.text.ToLower()[0] == collectibleString[i])
      {
        return i;
      }
    }
    return 0;
  }
  private void OnTriggerEnter2D(Collider2D other)
  {
    player = other.gameObject.GetComponent<Player>();
    if (player != null)
    {
      currentLevel.AwardLetter(LetterIndex);
      AudioSource.PlayClipAtPoint(powerUpSound, Camera.main.transform.position, powerUpSoundVolume);
      if (AllLettersCollected()) gameSession.FinalLevelEnabled = true;
      Destroy(gameObject);
    }
  }
  private bool AllLettersCollected()
  {
    List<LevelContainer> levels;
    levels = FindObjectsOfType<LevelContainer>().ToList();

    for (int i = 0; i < levels.Count; i++)
    {
      for (int j = 0; j < levels[i].LetterCollected.Length; j++)
      {
        if (!levels[i].LetterCollected[j]) return false;
      }
    }
    return true;
  }
}
