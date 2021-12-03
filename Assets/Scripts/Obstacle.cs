using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
[Header("Obstacle")]
    [SerializeField] float health = 100;
    [SerializeField] GameObject explosionVfxPrefab;
    [SerializeField] float explosionDuration = 0.3f;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] float explosionSoundVolume = 0.05f;
    [SerializeField] int obstacleScoreValue = 100;

    //Cached refs
    GameSession gameSession;
    AsteroidSpawner asteroidSpawner;
    
    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        asteroidSpawner = gameObject.GetComponent<AsteroidSpawner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();

        if (health <= 0)
        {
          DestroyEnemy();
          gameSession.AddToScore(obstacleScoreValue);
        }
    }

    private void DestroyEnemy()
    {
        GameObject explosionVfx = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
        if(asteroidSpawner != null)
        {
            asteroidSpawner.SpawnAsteroids();
        }
        Destroy(gameObject);
        Destroy(explosionVfx, explosionDuration);
    }
}
