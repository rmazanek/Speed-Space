using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] float health = 100;
    [SerializeField] GameObject explosionVfxPrefab;
    [SerializeField] float explosionDuration = 0.3f;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] float explosionSoundVolume = 0.05f;
    [SerializeField] int enemyScoreValue = 100;
    [SerializeField] int damageReactionColorFrames = 20;
    [SerializeField] Color damageReactionColor;
    [SerializeField] AudioClip damageReactionSound;
    [SerializeField] float damageReactionSoundVolume = 0.05f;
    
    [Header("Projectile")]
    [SerializeField] bool shootingHandledElsewhere = false;
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject enemyProjectile;
    [SerializeField] float enemyProjectileSpeed = 10f;

    //[SerializeField] float enemyProjectileXMomentumFractionConserved = 0f;

    //Cached refs
    GameSession gameSession;
    LootDropHandler lootDropHandler;
    LevelManager levelManager;
    List<LevelContainer> levelList;
    LevelContainer currentLevel;
    Color originalColor;
    bool isDestroyed = false;
    DamageReaction damageReaction;
    
    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        gameSession = FindObjectOfType<GameSession>();
        levelManager = FindObjectOfType<LevelManager>();
        levelList = levelManager.GetLevelList();
        currentLevel = levelList[gameSession.GetLevelToStart()-1];
        health = health * currentLevel.EnemyHealthCumulativeMultiplier;
        maxTimeBetweenShots = Mathf.Max(maxTimeBetweenShots * currentLevel.EnemyMaxTimeBetweenShotsCumulativeMultiplier, minTimeBetweenShots);
        originalColor = gameObject.GetComponent<SpriteRenderer>().color;

        lootDropHandler = gameObject.GetComponent<LootDropHandler>();
        damageReaction = gameObject.GetComponent<DamageReaction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!shootingHandledElsewhere)
        {
            CountDownAndShoot();
        }
    }

    private void AdjustProjectile(GameObject projectile)
    {
        projectile.GetComponent<DamageDealer>().MultiplyDamage(currentLevel.EnemyDamageCumulativeMultiplier);
    }
    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, AdjustedMaxTimeBetweenShots());
        }
    }
    private float AdjustedMaxTimeBetweenShots()
    {
        levelManager = FindObjectOfType<LevelManager>();
        levelList = levelManager.GetLevelList();
        currentLevel = levelList[gameSession.GetLevelToStart()-1];
        float adjustedMaxTime = Mathf.Max(minTimeBetweenShots, maxTimeBetweenShots * currentLevel.EnemyMaxTimeBetweenShotsCumulativeMultiplier);
        Debug.Log("Adjusted Max Time Between Shots: " + adjustedMaxTime);
        return adjustedMaxTime;
    }
    private void Fire()
    {
        //float parentVelocityX = GetComponent<Rigidbody2D>().velocity.x;
        GameObject projectile = Instantiate(enemyProjectile, transform.position, Quaternion.identity) as GameObject;
        AdjustProjectile(projectile);
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(0,-enemyProjectileSpeed);
        //projectile.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity + new Vector2(0,-enemyProjectileSpeed);        
        // parentVelocityX * enemyProjectileXMomentumFractionConserved
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
        if(damageReaction != null)
        {
            damageReaction.ProcessHit(damageDealer);
        }
        else
        {
            StartCoroutine(DamageReaction());
            AudioSource.PlayClipAtPoint(damageReactionSound, Camera.main.transform.position, damageReactionSoundVolume);
        }
        
        if ((health <= 0) && !isDestroyed)
        {
          isDestroyed = true;
          DestroyEnemy();
          gameSession.AddToScore(enemyScoreValue);
        }
    }
    private void BossDutiesCheck()
    {
        var bossDuties = gameObject.GetComponentInChildren<BossDuties>();
        if(bossDuties != null)
        {
            gameSession = FindObjectOfType<GameSession>();
            UnpauseSpawns();
            levelManager = FindObjectOfType<LevelManager>();
            levelList = levelManager.GetLevelList();
            currentLevel = levelList[gameSession.GetLevelToStart()-1];
            bossDuties.BossDefeated();
            ModifyEnemies();
            currentLevel.LoopCount();
            currentLevel.SetBossDefeated();
        }
    }
    private void DestroyEnemy()
    {
        GameObject explosionVfx = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
        
        BossDutiesCheck();

        Destroy(gameObject);
        Destroy(explosionVfx, explosionDuration);
        
        if(lootDropHandler != null)
        {
            lootDropHandler.DropItem();
        }
    }
    private void ModifyEnemies()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        List<LevelContainer> levelList = levelManager.GetLevelList();
        LevelContainer currentLevel = levelList[gameSession.GetLevelToStart()-1];
        currentLevel.InvokeRandomModifier();
    }

    IEnumerator DamageReaction()
    {
        gameObject.GetComponent<SpriteRenderer>().color = damageReactionColor;
        yield return new WaitForSeconds(damageReactionColorFrames * Time.deltaTime);
        gameObject.GetComponent<SpriteRenderer>().color = originalColor;
    }
    private void UnpauseSpawns()
    {
        if(FindObjectsOfType<BossDuties>().Length <=1)
        {
            gameSession.AllSpawnsPaused = false;
        }
    }
}
