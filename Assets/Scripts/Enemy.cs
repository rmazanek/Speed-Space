using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  [Header("Enemy")]
  [SerializeField] float health = 100;
  [SerializeField] GameObject explosionVfxPrefab;
  [SerializeField] public float ExplosionDuration = 0.3f;
  [SerializeField] AudioClip explosionSound;
  [SerializeField] float explosionSoundVolume = 0.05f;
  [SerializeField] int enemyScoreValue = 100;
  [SerializeField] int damageReactionColorFrames = 20;
  [SerializeField] Color damageReactionColor;
  [SerializeField] AudioClip damageReactionSound;
  [SerializeField] float damageReactionSoundVolume = 0.05f;
  public bool CanBeSlowed = true;
  [SerializeField] float minimumTemporarySpeedFactor = 0.15f;
  [SerializeField] float maximumTemporarySpeedFactor = 1f;
  [Header("Projectile")]
  [SerializeField] bool shootingHandledHere = true;
  [SerializeField] float shotCounter;
  [SerializeField] float minTimeBetweenShots = 0.2f;
  [SerializeField] float maxTimeBetweenShots = 3f;
  [SerializeField] GameObject enemyProjectile;
  [SerializeField] bool aimedShot = false;
  [SerializeField] float enemyProjectileSpeed = 10f;
  [Header("Rapid Fire Options")]
  [SerializeField] bool isRapidFire = false;
  [SerializeField] int numberOfRapidFireProjectiles = 40;
  [SerializeField] float rapidFireDelay = 0.2f;

  //Cached refs
  GameSession gameSession;
  LootDropHandler lootDropHandler;
  LevelManager levelManager;
  List<LevelContainer> levelList;
  LevelContainer currentLevel;
  Color originalColor;
  bool isDestroyed = false;
  DamageReaction damageReaction;
  float maxHealth;
  DamageFriendOnDeath damageFriendOnDeath;
  Vector2 initialVelocity;
  Rigidbody2D rigidbodyOnObject;
  BossDuties bossDuties;

  // Start is called before the first frame update
  void Start()
  {
    maxHealth = health;
    shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    gameSession = FindObjectOfType<GameSession>();
    levelManager = FindObjectOfType<LevelManager>();
    levelList = levelManager.GetLevelList();
    currentLevel = levelList[gameSession.GetLevelToStart() - 1];
    health = health * currentLevel.EnemyHealthCumulativeMultiplier;
    maxTimeBetweenShots = Mathf.Max(maxTimeBetweenShots * currentLevel.EnemyMaxTimeBetweenShotsCumulativeMultiplier, minTimeBetweenShots);
    originalColor = gameObject.GetComponent<SpriteRenderer>().color;
    lootDropHandler = gameObject.GetComponent<LootDropHandler>();
    damageReaction = gameObject.GetComponent<DamageReaction>();
    damageFriendOnDeath = gameObject.GetComponent<DamageFriendOnDeath>();
    rigidbodyOnObject = gameObject.GetComponent<Rigidbody2D>();
    initialVelocity = rigidbodyOnObject.velocity;
    bossDuties = gameObject.GetComponentInChildren<BossDuties>();
  }

  // Update is called once per frame
  void Update()
  {
    if (shootingHandledHere)
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
    currentLevel = levelList[gameSession.GetLevelToStart() - 1];
    float adjustedMaxTime = Mathf.Max(minTimeBetweenShots, maxTimeBetweenShots * currentLevel.EnemyMaxTimeBetweenShotsCumulativeMultiplier);
    Debug.Log("Adjusted Max Time Between Shots: " + adjustedMaxTime);
    return adjustedMaxTime;
  }
  private void Fire()
  {
    if (isRapidFire)
    {
      FireRapid();
    }
    else
    {
      FireSingle();
    }
  }
  private void FireSingle()
  {
    Vector2 projectileVelocity;
    Quaternion projectileRotation;
    if (aimedShot)
    {
      projectileVelocity = -transform.up * enemyProjectileSpeed;
      projectileRotation = transform.rotation;
    }
    else
    {
      projectileVelocity = new Vector2(0, -enemyProjectileSpeed);
      projectileRotation = Quaternion.identity;
    }
    GameObject projectile = Instantiate(enemyProjectile, transform.position, projectileRotation) as GameObject;
    AdjustProjectile(projectile);
    projectile.GetComponent<Rigidbody2D>().velocity = projectileVelocity;
  }
  private void FireRapid()
  {
    StartCoroutine(FireRapidCoroutine());
  }
  IEnumerator FireRapidCoroutine()
  {
    int puffs = 0;
    while (puffs < numberOfRapidFireProjectiles)
    {
      FireSingle();
      puffs++;
      yield return new WaitForSeconds(rapidFireDelay);
    }
  }
  private void OnTriggerEnter2D(Collider2D other)
  {
    DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
    if (!damageDealer) { return; }
    ProcessHit(damageDealer);
  }
  public void ReceiveDamage(int damage)
  {
    GameObject dummyAttacker = new GameObject();
    dummyAttacker.transform.position = gameObject.transform.position;
    dummyAttacker.transform.rotation = gameObject.transform.rotation;
    DamageDealer damageDealer = dummyAttacker.AddComponent<DamageDealer>();
    //damageDealer.SetDestroySelfOnHit(true);
    damageDealer.SetDamage(damage);
    ProcessHit(damageDealer);
  }
  private void ProcessHit(DamageDealer damageDealer)
  {
    health -= damageDealer.GetDamage();
    damageDealer.Hit();
    if (damageReaction != null)
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
      damageFriendOnDeath?.DoDamage();
      DestroyEnemy();
      gameSession.AddToScore(enemyScoreValue);
    }
  }
  private void DestroyEnemy()
  {
    DestroyWithoutReward();
    DropLoot();
  }
  private void DropLoot()
  {
    if (lootDropHandler != null)
    {
      lootDropHandler.DropItem();
    }
  }
  private void UnpauseCheck()
  {
    if (OneRemainingWavePauser() & !ShopExists())
    {
      gameSession = FindObjectOfType<GameSession>();
      if (gameSession != null)
      {
        gameSession.AllSpawnsPaused = false;
      }
    }
  }
  private bool ShopExists()
  {
    EnemyPathing potentialShop = FindObjectOfType<EnemyPathing>();
    return (potentialShop?.IsShop() ?? false);
  }
  private bool OneRemainingWavePauser()
  {
    return FindObjectsOfType<EnemyWavePauser>().Length <= 1 && (FindObjectOfType<BossDuties>() == null);
  }
  public void DestroyWithoutReward()
  {
    UnpauseCheck();
    PlayExplosion(transform.position);

    if (bossDuties != null)
    {
      bossDuties.BossDutiesCheck();
    }
    else
    {
      Destroy(gameObject);
    }
  }
  public void PlayExplosion(Vector3 position)
  {
    GameObject explosionVfx = Instantiate(explosionVfxPrefab, position, Quaternion.identity);
    AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
    Destroy(explosionVfx, ExplosionDuration);
  }
  IEnumerator DamageReaction()
  {
    gameObject.GetComponent<SpriteRenderer>().color = damageReactionColor;
    yield return new WaitForSeconds(damageReactionColorFrames * Time.deltaTime);
    gameObject.GetComponent<SpriteRenderer>().color = originalColor;
  }
  public float GetHealth()
  {
    return health;
  }
  public float GetMaxHealth()
  {
    return maxHealth;
  }
  public void SetBaseProjectileSpeed(float newSpeed)
  {
    enemyProjectileSpeed = newSpeed;
  }
  public void SetBaseMinMaxTimeBetweenShots(float newTimeBetweenShots)
  {
    minTimeBetweenShots = newTimeBetweenShots;
    maxTimeBetweenShots = newTimeBetweenShots;
  }
  public void SetBaseMinMaxTimeBetweenShots(float newMin, float newMax)
  {
    minTimeBetweenShots = newMin;
    maxTimeBetweenShots = newMax;
  }
  IEnumerator MultiplySpeedByFactorTemporarilyCoroutine(float factor, float timeInSeconds)
  {
    Vector2 currentDirection = rigidbodyOnObject.velocity.normalized;
    rigidbodyOnObject.velocity = currentDirection * rigidbodyOnObject.velocity.magnitude * Mathf.Clamp(rigidbodyOnObject.velocity.magnitude * factor / initialVelocity.magnitude, minimumTemporarySpeedFactor, maximumTemporarySpeedFactor);
    yield return new WaitForSeconds(timeInSeconds);
    currentDirection = rigidbodyOnObject.velocity.normalized;
    rigidbodyOnObject.velocity = currentDirection * rigidbodyOnObject.velocity.magnitude / factor;
  }
  public void MultiplySpeedByFactorTemporarily(float factor, float timeInSeconds)
  {
    if (bossDuties == null && CanBeSlowed)
    {
      StartCoroutine(MultiplySpeedByFactorTemporarilyCoroutine(factor, timeInSeconds));
    }
  }
  IEnumerator DestroyAfterTimeWithoutRewardCoroutine(float timeInSeconds)
  {
    Time.timeScale = gameSession.SlowTimeScale;
    yield return new WaitForSecondsRealtime(timeInSeconds);
    DestroyWithoutReward();
  }
  public void DestroyAfterTimeWithoutReward(float timeInSeconds)
  {
    StartCoroutine(DestroyAfterTimeWithoutRewardCoroutine(timeInSeconds));
  }
}
