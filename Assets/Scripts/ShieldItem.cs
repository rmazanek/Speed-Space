using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : Item
{
  [SerializeField] float maxHealth = 500f;
  [SerializeField] float regenPerSecond = 50f;
  [SerializeField] float brokenCooldown = 10f;
  [SerializeField] AudioClip powerUpSound;
  [SerializeField] float powerUpSoundVolume = 0.05f;
  Player player;
  [SerializeField] string itemName;
  [SerializeField, Tooltip("Dynamic tags: {shieldHealth}, {regenRate}, {cooldown}"), TextArea] 
  string tooltipText = "Tooltip text.";
  TooltipDisplay tooltipDisplay;
  Sprite sprite;
  PlayerBindings playerBindings;
  private void Start()
  {
    tooltipDisplay = GameObject.FindObjectOfType<TooltipDisplay>();
    sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    playerBindings = FindObjectOfType<PlayerBindings>();
    player = playerBindings.GetMainPlayer();
    PopulateTooltipText();
  }
  private void Update()
  {
    if (PlayerIsDetected())
    {
      UpdateTooltipText();
    }
  }
  public float GetMaxHealth() { return maxHealth; }
  public float GetRegenRate() { return regenPerSecond; }
  public float GetBrokenCooldown() { return brokenCooldown; }
  private void OnTriggerEnter2D(Collider2D other)
  {
    player = other.gameObject.GetComponent<Player>();
    if (player != null)
    {
      player.SetNewShield(this);
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
    itemName = itemName.Replace("{shieldHealth}", (GetMaxHealth()).ToString("F0"));
    itemName = itemName.Replace("{regenRate}", GetRegenRate().ToString("F1"));
    itemName = itemName.Replace("{cooldown}", (GetBrokenCooldown()).ToString("F1"));
    return itemName;
  }
  public override Sprite GetSprite()
  {
    return sprite;
  }
  public override void PopulateTooltipText()
  {
    playerBindings = FindObjectOfType<PlayerBindings>();
    player = playerBindings.GetMainPlayer();

    itemName = itemName.Replace("{shieldHealth}", (GetMaxHealth()).ToString("F0"));
    itemName = itemName.Replace("{regenRate}", GetRegenRate().ToString("F1"));
    itemName = itemName.Replace("{cooldown}", (GetBrokenCooldown()).ToString("F1"));
    tooltipText = tooltipText.Replace("{shieldHealth}", (GetMaxHealth()).ToString("F0"));
    tooltipText = tooltipText.Replace("{regenRate}", GetRegenRate().ToString("F1"));
    tooltipText = tooltipText.Replace("{cooldown}", (GetBrokenCooldown()).ToString("F1"));
  }
  private int GetNumberOfProjectiles(GameObject projectileFired)
  {
    return projectileFired.GetComponentsInChildren<DamageDealer>().Length;
  }
}
