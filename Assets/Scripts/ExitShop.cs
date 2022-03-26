using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExitShop : MonoBehaviour
{
    [Header("Exit GameObject")]
    [SerializeField] GameObject shopToClose;
    [SerializeField] int hitsToBreak = 10;
    [Header("Spawn In Fanfare")]
    [SerializeField] AudioClip spawnInSound;
    [SerializeField] float spawnInSoundVolume = 0.1f;
    [Header("Death")]
    [SerializeField] GameObject explosionVfxPrefab;
    [SerializeField] float explosionDuration = 0.3f;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] float explosionSoundVolume = 0.05f;
    [Header("Damage Reaction")]
    [SerializeField] int damageReactionColorFrames = 20;
    [SerializeField] Color damageReactionColor;
    [SerializeField] AudioClip damageReactionSound;
    [SerializeField] float damageReactionSoundVolume = 0.05f;  
    //Cached refs
    GameSession gameSession;
    Color originalColor;
    int remainingHits;
    private void Start()
    {
        remainingHits = hitsToBreak;
        originalColor = gameObject.GetComponent<TextMeshPro>().faceColor;
        gameSession = FindObjectOfType<GameSession>();
        gameSession.AllSpawnsPaused = true;
        AudioSource.PlayClipAtPoint(spawnInSound, Camera.main.transform.position, spawnInSoundVolume);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }
    private void ProcessHit(DamageDealer damageDealer)
    {
        remainingHits--;
        
        damageDealer.Hit();
        StartCoroutine(DamageReaction());
        AudioSource.PlayClipAtPoint(damageReactionSound, Camera.main.transform.position, damageReactionSoundVolume);

        if (remainingHits <= 0)
        {
          DestroyEnemy();
        }
    }
    private void DestroyEnemy()
    {
        GameObject explosionVfx = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
        gameSession = FindObjectOfType<GameSession>();
        gameSession.AllSpawnsPaused = false;
        Destroy(shopToClose);
        //Destroy(gameObject);
        Destroy(explosionVfx, explosionDuration);
    }
    IEnumerator DamageReaction()
    {
        gameObject.GetComponent<TextMeshPro>().faceColor = damageReactionColor;
        yield return new WaitForSeconds(damageReactionColorFrames * Time.deltaTime);
        gameObject.GetComponent<TextMeshPro>().faceColor = originalColor;
    }
}
