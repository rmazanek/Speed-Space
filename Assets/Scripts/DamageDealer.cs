using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damage = 100;

    [Header("Explosion Vfx")]
    [SerializeField] GameObject explosionVfxPrefab;
    [SerializeField] float explosionDuration = 0.3f;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] float explosionSoundVolume = 0.05f;
    [SerializeField] bool destroySelfOnHit = true;

    public int GetDamage()
    {
        //Debug.Log(gameObject.name + " did " + damage + " damage.");
        return damage;
    }
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    public void ApplyDamageBonus(float multiplier)
    {
        damage = Mathf.FloorToInt(damage * multiplier);
    }
    public void Hit()
    {
        PlayHitEffects();
        if(destroySelfOnHit)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayHitEffects();
    }
    public void PlayHitEffects()
    {
        if(explosionVfxPrefab != null)
        {
            GameObject explosionVfx = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
            Destroy(explosionVfx, explosionDuration);
        }
        if(explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
        }
    }
    public void MultiplyDamage(float multiplier)
    {
        damage = Mathf.FloorToInt(damage * multiplier);
    }
}
