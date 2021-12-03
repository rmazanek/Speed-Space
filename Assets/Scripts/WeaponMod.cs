using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMod : MonoBehaviour
{
    public float FirePeriodReduction = 1f;
    public float DamageMultiplier = 1f;
    public float ProjectileSpeedMultiplier = 1f;
    Weapon weapon;
    public List<ShipModifier> WeaponModifiers;
    GameSession gameSession;
    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        weapon = gameObject.GetComponent<Weapon>();
        WeaponModifiers = GetShipModifiers();
    }

  public void UpdateWeaponModIcons()
  {
    WeaponModifiers = GetShipModifiers();
  }
  private List<ShipModifier> GetShipModifiers()
  {
    List<ShipModifier> shipModifiers = new List<ShipModifier>();

    shipModifiers.Add(WeaponModifierDatum("Fire Period Reduction", gameSession.FirePeriodReductionSprite, "x", FirePeriodReduction));
    shipModifiers.Add(WeaponModifierDatum("Damage Multiplier", gameSession.DamageMultiplierSprite, "x", DamageMultiplier));
    shipModifiers.Add(WeaponModifierDatum("Projectile Speed Multiplier", gameSession.ProjectileSpeedMultiplierSprite, "x", ProjectileSpeedMultiplier));

    return shipModifiers;
  }

  private ShipModifier WeaponModifierDatum(string name, Sprite sprite, string modifierPrefix, float modifier, string rounding="F2")
  {
    if(modifier != 1f)
    {
        ShipModifier shipModifier = new ShipModifier();
        shipModifier.Name = name;
        shipModifier.Sprite = sprite;
        shipModifier.Modifier = modifierPrefix + modifier.ToString(rounding);
        return shipModifier;
    }
    else
    {
        return null;
    }
  }

  public void MultiplyFiringPeriod(float factor)
    {
        FirePeriodReduction *= factor;
        weapon.UpdateWeaponMod();
        UpdateWeaponModIcons();
    }
    public void MultiplyDamage(float factor)
    {
        DamageMultiplier *= factor;
        weapon.UpdateWeaponMod();
        UpdateWeaponModIcons();
    }
    public void MultiplyProjectileSpeed(float factor)
    {
        ProjectileSpeedMultiplier *= factor;
        weapon.UpdateWeaponMod();
        UpdateWeaponModIcons();
    }
    public void ResetWeaponModFactors()
    {
        ResetFirePeriodReduction();
        ResetDamageMultiplier();
        ResetProjectileSpeedMultiplier();
    }
    public void ResetFirePeriodReduction()
    {
        FirePeriodReduction = 1f;
        weapon.UpdateWeaponMod();
        UpdateWeaponModIcons();
    }
    public void ResetDamageMultiplier()
    {
        DamageMultiplier = 1f;
        weapon.UpdateWeaponMod();
        UpdateWeaponModIcons();
    }
    public void ResetProjectileSpeedMultiplier()
    {
        ProjectileSpeedMultiplier = 1f;
        weapon.UpdateWeaponMod();
        UpdateWeaponModIcons();
    }
}
