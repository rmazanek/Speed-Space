using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
  float xPositionMin;
  float xPositionMax;
  float yPositionMin;
  float yPositionMax;
  [SerializeField] float keepDistanceFromParent = 500f;

  [Header("Player")]
  [SerializeField] float yPositionMinViewPort;
  [SerializeField] float yPositionMaxViewPort;
  [SerializeField] float xPositionMinViewPort = 0f;
  [SerializeField] float xPositionMaxViewPort = 1f;
  [SerializeField] float maxHealth = 200;
  [SerializeField] float health;
  [SerializeField] HealthBar healthBar;
  [SerializeField] GameObject explosionVfxPrefab;
  [SerializeField] float explosionDuration = 0.3f;
  [SerializeField] AudioClip explosionSound;
  [SerializeField] float explosionSoundVolume = 0.05f;
  [SerializeField] AudioClip laserDamageSound;
  [Header("Crew")]
  [SerializeField] GameObject interiorCanvas;
  [SerializeField] int maxCrewSize = 1;
  [Header("Shield")]
  [SerializeField] Shield shield;
  int currentCrewSize = 0;
  float xPadding;
  float yPadding;
  Player player;
  //Vector3 weaponLocation;
  Coroutine firingCoroutine;
  Coroutine weaponCooldownCoroutine;
  SceneLoader sceneLoader;
  HealthDisplay healthDisplay;
  GameSession gameSession;
  PlayerBindings playerBindings;
  GameObject parent;
  public float FirePeriodReduction { get; set; }
  PlayerControls controls;
  //public bool IsFiring = false;
  public int RoundsSurvived = 0;
  ExperienceCounter experienceCounter;
  public Dictionary<String, String> StatsList = new Dictionary<String, String>();
  Weapon weapon;
  WeaponMod weaponMod;
  public List<CrewMember> CrewMembers;
  PlayerManager playerManager;
  public List<ShipModifier> ShipModifiers;
  public List<ShipModifier> HealthModifiers;
  public List<ShipModifier> ShieldModifiers;
  private float startingHealth;
  CameraShake cameraShake;
  AudioSource mainCameraAudioSource;
  // Start is called before the first frame update
  private void Awake()
  {
    controls = new PlayerControls();
    controls.Gameplay.Fire.performed += ctx => Fire(true);
    controls.Gameplay.Fire.canceled += ctx => Fire(false);
  }

  void Start()
  {
    player = FindObjectOfType<Player>();
    playerManager = gameObject.GetComponent<PlayerManager>();
    InitializeShakeCamera();
    CrewMembers = GetCrewMembersList();
    InitializeCrew();
    gameSession = FindObjectOfType<GameSession>();
    SetName();
    playerBindings = FindObjectOfType<PlayerBindings>();
    parent = FindObjectOfType<ParentPositionMover>().gameObject;
    parent.GetComponent<ParentPositionMover>().RadiusToShipUpdate();
    health = maxHealth;
    healthDisplay = FindObjectOfType<HealthDisplay>();
    healthBar = GetComponentInChildren<HealthBar>();
    healthBar.UpdateHealthBar(1f, maxHealth);
    sceneLoader = FindObjectOfType<SceneLoader>();
    experienceCounter = gameObject.GetComponent<ExperienceCounter>();
    FirePeriodReduction = 1f;
    SetUpMoveBoundaries();
    weapon = gameObject.GetComponent<Weapon>();
    weaponMod = gameObject.GetComponent<WeaponMod>();
    interiorCanvas.SetActive(false);
    startingHealth = maxHealth;
  }
  private List<ShipModifier> GetShipModifiers()
  {
    List<ShipModifier> shipModifiers = new List<ShipModifier>();
    shipModifiers.AddRange(weaponMod.WeaponModifiers);
    if (HealthModifiers != null)
    {
      shipModifiers.AddRange(HealthModifiers);
    }
    if (ShieldModifiers != null)
    {
      shipModifiers.AddRange(ShieldModifiers);
    }
    shipModifiers = shipModifiers.Where(o => o != null).ToList();

    return shipModifiers;
  }
  public void StoreShipModifiers()
  {
    StoreHealthModifiers();
    StoreShieldModifiers();
    ShipModifiers = GetShipModifiers();
  }
  private void StoreHealthModifiers()
  {
    HealthModifiers = GetHealthModifiers();
  }
  private void StoreShieldModifiers()
  {
    ShieldModifiers = GetShieldModifiers();
  }
  private ShipModifier ShipModifierDatum(string name, Sprite sprite, string modifier)
  {
    ShipModifier shipModifier = new ShipModifier();
    shipModifier.Name = name;
    shipModifier.Sprite = sprite;
    shipModifier.Modifier = modifier;
    return shipModifier;
  }
  public List<ShipModifier> GetHealthModifiers()
  {
    List<ShipModifier> shipModifiers = new List<ShipModifier>();
    if (maxHealth - startingHealth > 0f)
    {
      shipModifiers.Add(ShipModifierDatum("Health Modifier", gameSession.MaxHealthSprite, "+" + (maxHealth - startingHealth).ToString("F0")));
    }

    return shipModifiers;
  }
  public List<ShipModifier> GetShieldModifiers()
  {
    List<ShipModifier> shipModifiers = new List<ShipModifier>();
    if (shield != null)
    {
      if (shield.GetMaxHealth() - shield.GetStartingHealth() > 0)
      {
        shipModifiers.Add(ShipModifierDatum("Shield Health Modifier", gameSession.MaxShieldHealthSprite, "+" + (shield.GetMaxHealth() - shield.GetStartingHealth()).ToString("F0")));
      }
      if (shield.GetRegenRate() - shield.GetStartingRegenRate() > 0)
      {
        shipModifiers.Add(ShipModifierDatum("Shield Regen Modifier", gameSession.ShieldRegenSprite, "+" + (shield.GetRegenRate() - shield.GetStartingRegenRate()).ToString("F0") + "/s"));
      }
    }
    return shipModifiers;
  }
  private void SetName()
  {
    int rand = UnityEngine.Random.Range(0, gameSession.GetComponent<NameList>().NamesList.Length);
    this.name = gameSession.GetComponent<NameList>().NamesList[rand];
  }
  private void InitializeCrew()
  {
    foreach (CrewMember crewMember in CrewMembers)
    {
      crewMember.Initialize();
    }
  }
  private void OnEnable()
  {
    controls.Gameplay.Enable();
  }
  private void OnDisable()
  {
    controls.Gameplay.Disable();
  }
  private void ClampPosition()
  {
    var newXPos = Mathf.Clamp(transform.position.x, xPositionMin, xPositionMax);
    var newYPos = Mathf.Clamp(transform.position.y, yPositionMin, yPositionMax);

    transform.position = new Vector2(newXPos, newYPos);
  }
  private void KeepMinimumDistance()
  {
    var parentPosition = parent.transform.position;
    float distance = Vector3.Distance(parentPosition, transform.position);

    if (distance != keepDistanceFromParent)
    {
      Vector3 vectorToCenter = parentPosition - transform.position;
      vectorToCenter = vectorToCenter.normalized;
      vectorToCenter *= distance - keepDistanceFromParent;
      transform.position += vectorToCenter;
    }

    ClampPosition();
  }
  public void InitializeShakeCamera()
  {
    cameraShake = Camera.main.GetComponent<CameraShake>();
  }
  void ShakeCamera()
  {
    if (cameraShake != null)
    {
      cameraShake.Play();
    }
  }
  private void OnTriggerEnter2D(Collider2D other)
  {
    DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
    if (!damageDealer) { return; }
    ProcessHit(damageDealer);
  }
  public void ReceiveLaserDamage(float damage)
  {
    if (!PauseMenu.GamePaused)
    {
      health -= damage;
      ShakeCamera();
      Camera.main.GetComponent<AudioSource>().PlayOneShot(laserDamageSound, 1f);
      HealthDisplayUpdate();
      DeathCheck();
    }
  }

  private void ProcessHit(DamageDealer damageDealer)
  {
    health -= (float)damageDealer.GetDamage();
    damageDealer.Hit();
    ShakeCamera();
    HealthDisplayUpdate();
    DeathCheck();
  }

  private void DeathCheck()
  {
    if (health <= 0)
    {
      DestroyPlayer();
      int remainingShips = playerBindings.GetPlayerShips().Count - 1;
      if (remainingShips == 0)
      {
        gameSession.GameStarted = false;
        sceneLoader = FindObjectOfType<SceneLoader>();
        sceneLoader.LoadGameOver();
      }
      else
      {
        gameSession.CycleMainShip = true;
        GameObject.FindObjectOfType<ParentPositionMover>().RadiusToShipUpdate();
      }
    }
  }
  private void DestroyPlayer()
  {
    GameObject explosionVfx = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
    AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
    Destroy(gameObject);
    Destroy(explosionVfx, explosionDuration);
  }
  private void SetUpMoveBoundaries()
  {
    SetUpPlayerHitboxMovePadding();
    Camera gameCamera = Camera.main;
    xPositionMin = gameCamera.ViewportToWorldPoint(new Vector3(xPositionMinViewPort, 0, 0)).x + xPadding;
    xPositionMax = gameCamera.ViewportToWorldPoint(new Vector3(xPositionMaxViewPort, 0, 0)).x - xPadding;
    yPositionMin = gameCamera.ViewportToWorldPoint(new Vector3(0, yPositionMinViewPort, 0)).y + yPadding;
    yPositionMax = gameCamera.ViewportToWorldPoint(new Vector3(0, yPositionMaxViewPort, 0)).y - yPadding;
  }

  private void SetUpPlayerHitboxMovePadding()
  {
    SpriteRenderer spriteRenderer = FindObjectOfType<Player>().GetComponent<SpriteRenderer>();

    xPadding = spriteRenderer.sprite.bounds.extents.x;
    yPadding = spriteRenderer.sprite.bounds.extents.y;
  }
  public void Fire(bool playerFiring, bool optionalStartFiring = false)
  {
    if ((optionalStartFiring | playerFiring) && !PauseMenu.GamePaused && !weapon.WeaponIsFiring)
    {
      firingCoroutine = StartCoroutine(weapon.FireContinuously());
      weaponCooldownCoroutine = StartCoroutine(weapon.CooldownTimer());
    }
    if (!playerFiring & firingCoroutine != null)
    {
      StopCoroutine(firingCoroutine);
    }
  }
  public void AddToHealth(int healAmount)
  {
    health += Mathf.Min(healAmount, maxHealth - health);
    HealthDisplayUpdate();
  }
  public void AddToMaxHealth(int additionalHealth)
  {
    maxHealth += additionalHealth;
    HealthDisplayUpdate();
  }
  public void HealthDisplayUpdate()
  {
    healthDisplay = FindObjectOfType<HealthDisplay>();
    healthDisplay.UpdateHealth();
    healthBar.UpdateHealthBar(GetFillPercent(), GetMaxHealth());
  }
  public float GetHealth() { return health; }
  public float GetMaxHealth() { return maxHealth; }
  private float GetFillPercent() { return GetHealth() / GetMaxHealth(); }
  public void ReducedFiringPeriod(float reduction)
  {
    weaponMod = gameObject.GetComponent<WeaponMod>();
    if (weaponMod != null)
    {
      weaponMod.MultiplyFiringPeriod(reduction);
    }
  }
  public void MultiplyDamage(float factor)
  {
    weaponMod = gameObject.GetComponent<WeaponMod>();
    if (weaponMod != null)
    {
      weaponMod.MultiplyDamage(factor);
    }
  }
  public void SetNewWeapon(Weapon weapon)
  {
    this.weapon.ProjectilePrefab = weapon.ProjectilePrefab;
    this.weapon.SetBaseUseCooldown(weapon.GetUnmodifiedUseCooldown());
    this.weapon.SetBaseProjectileSpeed(weapon.GetUnmodifiedProjectileSpeed());
    UpdateWeapon();
  }
  private void UpdateWeapon()
  {
    weapon = gameObject.GetComponent<Weapon>();
  }
  public void AddToRounds()
  {
    RoundsSurvived++;
    AddToRoundsForCrew();
  }

  private void AddToRoundsForCrew()
  {
    List<CrewMember> crewMembers = new List<CrewMember>();
    crewMembers = GetCrewMembersList();
    foreach (CrewMember crewMember in crewMembers)
    {
      crewMember.AddToRounds();
    }
  }

  public Sprite GetShipSprite()
  {
    Sprite shipSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    return shipSprite;
  }
  public Dictionary<String, String> GetStats()
  {
    UpdateStatsList();
    return StatsList;
  }
  private void UpdateStatsList()
  {
    StatsList.Clear();
    StatsList.Add("Name", this.name);
    StatsList.Add("Active Time", (this.experienceCounter.GetActiveTimeInMinutesAndSeconds()));
    StatsList.Add("Level", this.experienceCounter.GetExperienceLevel().ToString());
    StatsList.Add("Experience", this.experienceCounter.GetExperience().ToString("F0"));
    StatsList.Add("Health", this.health.ToString("F0") + " / " + this.maxHealth.ToString("F0"));
    StatsList.Add("Rounds Survived", this.RoundsSurvived.ToString("F0"));
    StatsList.Add("Crew", this.currentCrewSize.ToString("F0") + " / " + this.maxCrewSize.ToString("F0"));
    StatsList.Add("Position", this.GetPositionIndexLabelPosition0());
    GetProjectileStats();
    GetWeaponModStats();
  }
  public string GetPositionIndexLabelPosition0()
  {
    if (playerManager.PositionIndex == 0)
    {
      return "Main Ship";
    }
    else
    {
      return playerManager.PositionIndex.ToString();
    }
  }
  private void GetProjectileStats()
  {
    DamageDealer[] projectiles = weapon.ProjectilePrefab.GetComponentsInChildren<DamageDealer>();
    string suffix;

    for (int i = 0; i < projectiles.Length; i++)
    {
      if (projectiles.Length > 1)
      {
        suffix = " " + (i + 1).ToString("F0");
      }
      else
      {
        suffix = "";
      }

      StatsList.Add("Projectile Type" + suffix, projectiles[i].name);
      StatsList.Add("Projectile Damage" + suffix, (projectiles[i].GetDamage() * this.weaponMod.DamageMultiplier).ToString("F0"));
    }
  }
  private void GetWeaponModStats()
  {
    weapon = gameObject.GetComponent<Weapon>();
    weaponMod = gameObject.GetComponent<WeaponMod>();
    StatsList.Add("Projectile Firing Period", this.weapon.GetModifiedUseCooldown().ToString("F3") + "s");
    StatsList.Add("Projectile Speed", this.weapon.GetModifiedProjectileSpeed().ToString("F1"));
  }
  public List<CrewMember> GetCrewMembersList()
  {
    List<CrewMember> crewMembers = new List<CrewMember>();
    int numberOfCrewMembers = gameObject.GetComponentsInChildren<CrewMember>(true).Length;

    foreach (CrewMember crewMember in gameObject.GetComponentsInChildren<CrewMember>(true))
    {
      crewMembers.Add(crewMember);
    }
    return crewMembers;
  }
  public void SaveCrewMemberList()
  {
    CrewMembers = GetCrewMembersList();
  }
  public Transform GetFirstCompartment()
  {
    return gameObject.GetComponentInChildren<Compartment>().transform;
  }
  public void SetNewShield(ShieldItem newShield)
  {
    shield.gameObject.SetActive(true);
    shield.GetComponent<SpriteRenderer>().sprite = newShield.GetSprite();
    shield.AddToMaxHealth((int)Mathf.Max(0, newShield.GetMaxHealth() - shield.GetMaxHealth()));
    shield.SetRegenRate(newShield.GetRegenRate());
    shield.SetBrokenCooldown(newShield.GetBrokenCooldown());
    shield.Activate();
    playerManager.SetShield(shield);
  }
}
