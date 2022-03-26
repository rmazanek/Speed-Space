using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
  [SerializeField] AudioClip powerUpSound;
  [SerializeField] float powerUpSoundVolume = 0.05f;
  Player player;
  [SerializeField] string itemName;
  [SerializeField, Tooltip("Dynamic tags: {numProjectiles}, {damage}, {useCooldown}, {projectileType}, {projectileSpeed}"), TextArea] 
  string tooltipText = "Tooltip text.";
  Weapon weapon;
  DamageDealer damageDealer;
  TooltipDisplay tooltipDisplay;
  Sprite sprite;
  WeaponMod activePlayerWeaponMod;
  PlayerBindings playerBindings;
  int numberOfProjectiles = 1;
  private void Start()
  {
    tooltipDisplay = GameObject.FindObjectOfType<TooltipDisplay>();
    sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    weapon = gameObject.GetComponent<Weapon>();
    damageDealer = GetFirstDamageDealerInPrefab();
    numberOfProjectiles = GetNumberOfProjectiles(weapon.ProjectilePrefab);
    playerBindings = FindObjectOfType<PlayerBindings>();
    player = playerBindings.GetMainPlayer();
    activePlayerWeaponMod = player.GetComponent<WeaponMod>();
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
      player.SetNewWeapon(weapon);
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
        activePlayerWeaponMod = player.GetComponent<WeaponMod>();
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
    return itemName;
  }
  public override Sprite GetSprite()
  {
    return sprite;
  }
  public override void PopulateTooltipText()
  {
    if(weapon == null)
    {
      Debug.Log("Weapon was null");
      weapon = gameObject.GetComponent<Weapon>();
      damageDealer = GetFirstDamageDealerInPrefab();
      numberOfProjectiles = GetNumberOfProjectiles(weapon.ProjectilePrefab);
    }
    playerBindings = FindObjectOfType<PlayerBindings>();
    player = playerBindings.GetMainPlayer();
    activePlayerWeaponMod = player.GetComponent<WeaponMod>();
    //Debug.Log(weapon.name);
    //Debug.Log(damageDealer.GetDamage());
    //Debug.Log(player.name);
    //Debug.Log(activePlayerWeaponMod.name);
    //Debug.Log(activePlayerWeaponMod.DamageMultiplier);
    //itemName = itemName.Replace("{damage}", (damageDealer.GetDamage() * activePlayerWeaponMod.DamageMultiplier).ToString("F0"));
    //itemName = itemName.Replace("{numProjectiles}", numberOfProjectiles.ToString() + " projectiles");
    //itemName = itemName.Replace("{projectileType}", weapon.ProjectilePrefab.name);
    //itemName = itemName.Replace("{useCooldown}", (weapon.GetUnmodifiedUseCooldown() * activePlayerWeaponMod.FirePeriodReduction).ToString("F3"));
    //itemName = itemName.Replace("{projectileSpeed}", (weapon.GetUnmodifiedProjectileSpeed() * activePlayerWeaponMod.ProjectileSpeedMultiplier).ToString("F0"));
    tooltipText = tooltipText.Replace("{damage}", (damageDealer.GetDamage() * activePlayerWeaponMod.DamageMultiplier).ToString("F0"));
    tooltipText = tooltipText.Replace("{numProjectiles}", numberOfProjectiles.ToString() + " projectiles");
    tooltipText = tooltipText.Replace("{projectileType}", weapon.ProjectilePrefab.name);
    tooltipText = tooltipText.Replace("{useCooldown}", (weapon.GetUnmodifiedUseCooldown() * activePlayerWeaponMod.FirePeriodReduction).ToString("F3"));
    tooltipText = tooltipText.Replace("{projectileSpeed}", (weapon.GetUnmodifiedProjectileSpeed() * activePlayerWeaponMod.ProjectileSpeedMultiplier).ToString("F0"));
  }
  private int GetNumberOfProjectiles(GameObject projectileFired)
  {
    return projectileFired.GetComponentsInChildren<DamageDealer>().Length;
  }
  private DamageDealer GetFirstDamageDealerInPrefab()
  {
    return weapon.ProjectilePrefab.GetComponentsInChildren<DamageDealer>()[0];
  }
}