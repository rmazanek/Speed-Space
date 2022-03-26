using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class NewShipItem : Item
{
  [SerializeField] AudioClip powerUpSound;
  [SerializeField] float powerUpSoundVolume = 0.05f;
  [SerializeField] Player[] playerPrefabs;
  GameSession gameSession;
  PlayerBindings playerBindings;
  Player collidingPlayer;
  [SerializeField] string itemName;
  [SerializeField] string tooltipText = "Tooltip text.";
  [SerializeField] string maxShipsWarning = "Maximum number of ships reached!";
  [SerializeField] Sprite maxShipsSprite;
  [SerializeField] RuntimeAnimatorController coinAnimator;
  TooltipDisplay tooltipDisplay;
  Sprite sprite;
  delegate void CollectionEvent();
  CollectionEvent collectionEvent;
  [SerializeField] int coinIncrease = 5;
  [SerializeField] AudioClip coinUpSound;
  [SerializeField] float coinUpSoundVolume = 0.05f;
  Wallet wallet;
  private void Start()
  {
    gameSession = FindObjectOfType<GameSession>();
    playerBindings = FindObjectOfType<PlayerBindings>();
    tooltipDisplay = GameObject.FindObjectOfType<TooltipDisplay>();
    sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    ConvertIfMaxShipsReached();
  }
  private void Update()
  {
    if (PlayerIsDetected())
    {
      UpdateTooltipText();
    }
  }
  private void OnTriggerEnter2D(Collider2D other)
  {
    collidingPlayer = other.gameObject.GetComponent<Player>();

    if (collidingPlayer != null)
    {
      ConvertIfMaxShipsReached();
      collectionEvent();
      UpdateTooltipText();
      Destroy(gameObject);
    }
  }
  private void InstantiateNewPlayer()
  {
    int index = (int)UnityEngine.Random.Range(0, playerPrefabs.Length);
    Player player = Instantiate(playerPrefabs[index], transform.position, Quaternion.Euler(0f, 0f, -180f));
    AudioSource.PlayClipAtPoint(powerUpSound, Camera.main.transform.position, powerUpSoundVolume);
    //player.Fire(true, true);
    player.transform.parent = GameObject.Find("Players").transform;
    //playerBindings.SetNewMainShip();
    gameSession.HealthUpdateNeeded = true;
  }
  private void GetCoins()
  {
    wallet = FindObjectOfType<Wallet>();
    if(wallet != null)
    {
        wallet.AddToWallet(coinIncrease);
        AudioSource.PlayClipAtPoint(coinUpSound, Camera.main.transform.position, coinUpSoundVolume);
        Destroy(gameObject);
    }
  }
  private void ConvertIfMaxShipsReached()
  {
    if(FindObjectsOfType<Player>().Length < GameSession.MaxPlayerShips)
    {
        collectionEvent = InstantiateNewPlayer;
    }
    else
    {
        ConvertItemToCoins();
    }
  }
  private void ConvertItemToCoins()
  {
    tooltipText = maxShipsWarning;
    sprite = maxShipsSprite;
    gameObject.GetComponent<SpriteRenderer>().sprite = maxShipsSprite;
    gameObject.GetComponent<Animator>().runtimeAnimatorController = coinAnimator;
    if(gameObject.GetComponent<Spinner>() != null)
    {
        Destroy(gameObject.GetComponent<Spinner>());
    }
    collectionEvent = GetCoins;
  }
  public void UpdateTooltipText()
  {
    tooltipDisplay.DisplayChange(itemName, tooltipText, sprite);
  }
  private bool PlayerIsDetected()
  {
    RaycastHit2D[] hit = Physics2D.RaycastAll(gameObject.transform.position, Vector2.down);
    for (int index = 0; index < hit.Length; index++)
    {
      Player player = hit[index].collider.GetComponent<Player>();
      if (player != null)
      {
        return true;
      }
      else
      {
        continue;
      }
    }
    return false;
  }
  public override string GetTooltipText()
  {
    return tooltipText;
  }
  public override string GetItemName()
  {
    return itemName;
  }
  public override Sprite GetSprite()
  {
    return sprite;
  }
}
