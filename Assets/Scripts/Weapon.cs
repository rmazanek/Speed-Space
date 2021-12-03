using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public GameObject ProjectilePrefab;
    [SerializeField] GameObject firingPoint;
    [SerializeField] float useCooldown = 0.4f;
    [SerializeField] float yProjectileVelocity = 7f;
    WeaponMod weaponMod;
    private void Start()
    {
        weaponMod = gameObject.GetComponent<WeaponMod>();
    }

    public IEnumerator FireContinuously()
    {
        while(true)
        {
            GameObject projectile = Instantiate(ProjectilePrefab, firingPoint.transform.position, Quaternion.identity) as GameObject;
            projectile.GetComponent<Rigidbody2D>().velocity = new Vector3(0, yProjectileVelocity * weaponMod.ProjectileSpeedMultiplier, 0);
            ApplyDamageBonus(projectile);
            yield return new WaitForSeconds(useCooldown * weaponMod.FirePeriodReduction);
        }
    }
    private void ApplyDamageBonus(GameObject projectile)
    {
        DamageDealer[] damageDealers = projectile.GetComponentsInChildren<DamageDealer>();
        for(int i=0; i < damageDealers.Length; i++)
        {
            damageDealers[i].ApplyDamageBonus(weaponMod.DamageMultiplier);
        }
    }
    public void UpdateWeaponMod()
    {
        weaponMod = gameObject.GetComponent<WeaponMod>();
    }
    public float GetUnmodifiedUseCooldown()
    {
        return useCooldown;
    }
    public float GetModifiedUseCooldown()
    {
        UpdateWeaponMod();
        return useCooldown * weaponMod.FirePeriodReduction;
    }
    public float GetUnmodifiedProjectileSpeed()
    {
        return yProjectileVelocity;
    }
    public float GetModifiedProjectileSpeed()
    {
        UpdateWeaponMod();
        return yProjectileVelocity * weaponMod.ProjectileSpeedMultiplier;
    }
    public void SetBaseUseCooldown(float newUseCooldown)
    {
        useCooldown = newUseCooldown;
    }
    public void SetBaseProjectileSpeed(float newProjectileSpeed)
    {
        yProjectileVelocity = newProjectileSpeed;
    }
}
