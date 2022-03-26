using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
  [SerializeField] int currentHealthIncrease = 50;
  [SerializeField] int maxHealthIncrease = 0;
  [SerializeField] AudioClip powerUpSound;
  [SerializeField] float powerUpSoundVolume = 0.05f;
  Player player;
  [SerializeField] string itemName;
  [SerializeField] string tooltipText = "Tooltip text.";
  TooltipDisplay tooltipDisplay;
  Sprite sprite;
  private void Start()
  {
    tooltipDisplay = GameObject.FindObjectOfType<TooltipDisplay>();
    sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    PopulateTooltipText();
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
    player = other.gameObject.GetComponent<Player>();
    if (player != null)
    {
      player.AddToMaxHealth(maxHealthIncrease);
      player.AddToHealth(currentHealthIncrease);
      AudioSource.PlayClipAtPoint(powerUpSound, Camera.main.transform.position, powerUpSoundVolume);
      UpdateTooltipText();
      Destroy(gameObject);
    }
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
    PopulateTooltipText();
    return tooltipText;
  }
  public override string GetItemName()
  {
    itemName = itemName.Replace("{maxHealth}", maxHealthIncrease.ToString());
    itemName = itemName.Replace("{heal}", currentHealthIncrease.ToString());
    return itemName;
  }
  public override Sprite GetSprite()
  {
    return sprite;
  }
  public override void PopulateTooltipText()
  {
    itemName = itemName.Replace("{maxHealth}", maxHealthIncrease.ToString());
    itemName = itemName.Replace("{heal}", currentHealthIncrease.ToString());
    tooltipText = tooltipText.Replace("{maxHealth}", maxHealthIncrease.ToString());
    tooltipText = tooltipText.Replace("{heal}", currentHealthIncrease.ToString());
  }
}
