using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringPeriodReductionItem : Item
{
  [SerializeField] float firingPeriodReduction = 0.50f;
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
      player.ReducedFiringPeriod(firingPeriodReduction);
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
    itemName = itemName.Replace("{dec}", (1-firingPeriodReduction).ToString("0.0%"));
    return itemName;
  }
  public override Sprite GetSprite()
  {
    return sprite;
  }
  public override void PopulateTooltipText()
  {
    itemName = itemName.Replace("{dec}", (1-firingPeriodReduction).ToString("0.0%"));
    tooltipText = tooltipText.Replace("{dec}", (1-firingPeriodReduction).ToString("0.0%"));
  }
}
