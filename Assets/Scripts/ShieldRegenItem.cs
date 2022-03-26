using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRegenItem : Item
{
  [SerializeField] int regenRateIncrease = 10;
  [SerializeField] AudioClip powerUpSound;
  [SerializeField] float powerUpSoundVolume = 0.05f;
  Shield shield;
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
    shield = other.gameObject.GetComponent<Shield>();
    if (shield != null)
    {
      shield.AddToRegenerationRate(regenRateIncrease);
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
    itemName = itemName.Replace("{regenRateIncrease}", regenRateIncrease.ToString());
    return itemName;
  }
  public override Sprite GetSprite()
  {
    return sprite;
  }
  public override void PopulateTooltipText()
  {
    itemName = itemName.Replace("{regenRateIncrease}", regenRateIncrease.ToString());
    tooltipText = tooltipText.Replace("{regenRateIncrease}", regenRateIncrease.ToString());
  }
}
