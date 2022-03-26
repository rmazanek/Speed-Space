using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] Sprite shieldSprite;
    [SerializeField] float maxHealth = 500f;
    [SerializeField] float regenPerSecond = 50f;
    [SerializeField] float brokenCooldown = 10f;
    [SerializeField] Color fullHealthColor;
    [SerializeField] GameObject explosionVfxPrefab;
    [SerializeField] AudioClip fullHealthSound;
    [SerializeField] float fullHealthSoundVolume = 0.5f;
    [SerializeField] AudioClip shieldActivateSound;
    [SerializeField] float shieldActivateSoundVolume = 0.5f;
    [SerializeField] AudioClip shieldDamageSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] float explosionSoundVolume = 0.5f;
    [SerializeField] float explosionDuration = 0.2f;
    [SerializeField] Collider2D shieldCollider;
    float health = 0;
    Color shieldColor;
    Color baseColor;
    Color transparent;
    Coroutine regenerating;
    Coroutine repairShield;
    float currentAlpha;
    float startingHealth;
    float startingRegenRate;
    private void Start()
    {
        startingHealth = maxHealth;
        startingRegenRate = regenPerSecond;
        
        //if(shieldCollider != null)
        //{
        //    shieldCollider.enabled = false;
        //    Activate();
        //}
    }
    public float GetStartingHealth() { return startingHealth; }
    public float GetStartingRegenRate() { return startingRegenRate; }
    public float GetMaxHealth() { return maxHealth; }
    public float GetRegenRate() { return regenPerSecond; }
    public float GetBrokenCooldown() { return brokenCooldown; }
    public float GetCurrentHealth() { return health; }
    public Sprite GetSprite() { return shieldSprite; }
    public void SetRegenRate(float newRegenPerSecond) { regenPerSecond = newRegenPerSecond; }
    public void SetBrokenCooldown(float newBrokenCooldown) { brokenCooldown = newBrokenCooldown; }
    float GetParentAlpha()
    {
        return gameObject.transform.parent.GetComponent<SpriteRenderer>().color.a;
    }
    public void AddToHealth(int healAmount)
    {
        health += Mathf.Min(healAmount,maxHealth-health);
        EnableCollider2D();
        gameObject.GetComponent<SpriteRenderer>().color = baseColor;
    }
    public void EnableCollider2D()
    {
        if(gameObject.transform.parent.GetComponent<Collider2D>().enabled && health > 0)
        {
            shieldCollider.enabled = true;
        }
    }
    public void DisableCollider2D()
    {
        shieldCollider.enabled = false;
    }
    public void SubtractFromHealth(int damageAmount)
    {
        health = Mathf.Clamp(health-damageAmount, 0, maxHealth);
    }
    public void AddToMaxHealth(int additionalHealth)
    {
        maxHealth += additionalHealth;
    }
    public void AddToRegenerationRate(int additionalHealthPerSecond)
    {
        regenPerSecond += additionalHealthPerSecond;
    }
    public void ApplyFactorToBrokenCooldown(float factor)
    {
        brokenCooldown *= factor;
    }
    private void OnEnable()
    {
        Activate();
    }
    public void Activate()
    {   
        shieldColor = new Color (255f, 255f, 255f, 1f);

        baseColor = new Color (shieldColor.r, shieldColor.g, shieldColor.b, 1f);
        transparent = new Color (baseColor.r, baseColor.g, baseColor.b, 0f);
        gameObject.GetComponent<SpriteRenderer>().color = fullHealthColor;

        health = maxHealth;

        shieldCollider.enabled = true;
        //regenerating = StartCoroutine(Regenerate());
    }
    IEnumerator Regenerate()
    {
        AudioSource.PlayClipAtPoint(shieldActivateSound, Camera.main.transform.position, shieldActivateSoundVolume);
        while(health < maxHealth)
        {
            yield return new WaitForSeconds(1f);
            AddToHealth((int)regenPerSecond);
        }
        AudioSource.PlayClipAtPoint(fullHealthSound, Camera.main.transform.position, fullHealthSoundVolume);
        ChangeToColorKeepAlpha(fullHealthColor);
    }
    IEnumerator RepairShield()
    {
        StopCoroutine(regenerating);
        yield return new WaitForSeconds(brokenCooldown);
        ChangeToColorKeepAlpha(shieldColor);
        regenerating = StartCoroutine(Regenerate());
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }
    public void ReceiveLaserDamage(float damage)
    {
        if(!PauseMenu.GamePaused)
        {
            SubtractFromHealth((int)damage);
            Camera.main.GetComponent<AudioSource>().PlayOneShot(shieldDamageSound, 0.5f);
            BrokenCheck();
        }
    }
    private void ProcessHit(DamageDealer damageDealer)
    {
        SubtractFromHealth(damageDealer.GetDamage());
        damageDealer.Hit();
        Camera.main.GetComponent<AudioSource>().PlayOneShot(shieldDamageSound, 1f);
        ChangeToColorKeepAlpha(shieldColor);

        if(regenerating == null)
        {
            regenerating = StartCoroutine(Regenerate());
        }
        BrokenCheck();
    }
    private void BrokenCheck()
    {
        if (health <= 0)
        {
            BreakShield();
            repairShield = StartCoroutine(RepairShield());
        }
    }
    private void BreakShield()
    {
        gameObject.GetComponent<SpriteRenderer>().color = transparent;
        shieldCollider.enabled = false;
        
        GameObject explosionVfx = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
        Destroy(explosionVfx, explosionDuration);
    }
    private void ChangeToColorKeepAlpha(Color color)
    {
        Color newColor = new Color (color.r, color.g, color.b, GetParentAlpha());
        gameObject.GetComponent<SpriteRenderer>().color = newColor;
    }
}
