using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointPlayer : MonoBehaviour
{
  LineRenderer lineRenderer;
  Vector2 firePoint;
  Vector2 playerPosition;
  GameSession gameSession;
  PlayerBindings playerBindings;
  bool lookAtPlayer = true;
  bool turnOnDamage = false;

  [Header("Shooting")]
  [SerializeField] bool pointAtPlayer = true;
  [SerializeField] float laserDamage = 1;
  [SerializeField] int laserHitsPerSecond = 100;
  [SerializeField] float shotCounter;
  [SerializeField] float minTimeBetweenShots = 1f;
  [SerializeField] float maxTimeBetweenShots = 1.5f;
  [SerializeField] float laserDuration = 2f;
  [SerializeField] float countDownToLaser = 2f;
  [SerializeField] Material pointerMaterial;
  [SerializeField] float pointerWidthMultiplier = 0.20f;
  [SerializeField] Material laserMaterial;
  [SerializeField] AudioClip lockOnSound;
  [SerializeField] float lockOnSoundVolume = 0.5f;
  [SerializeField] AudioClip laserSound;
  [SerializeField] float laserSoundVolume = 0.5f;
  [SerializeField] GameObject damageVfxPrefab;
  [SerializeField] float damageDuration = 0.1f;
  [SerializeField] GameObject endpointVfxPrefab;
  [SerializeField] float endpointDuration = 0.2f;
  [SerializeField] Transform nonPlayerTarget;
  float laserDamageFrameAdjusted;

  ParticleSystem.EmissionModule particleEmission;
  //ParticleSystem.ShapeModule particleShape;
  // Start is called before the first frame update
  void Start()
  {
    gameSession = FindObjectOfType<GameSession>();
    playerBindings = FindObjectOfType<PlayerBindings>();
    lineRenderer = this.GetComponent<LineRenderer>();
    lineRenderer.material = pointerMaterial;
    lineRenderer.widthMultiplier = pointerWidthMultiplier;
    particleEmission = GetComponent<ParticleSystem>().emission;
    //particleShape = GetComponent<ParticleSystem>().shape;
  }

  private void CalculateFrameAdjustedLaserDamage()
  {
    laserDamageFrameAdjusted = laserDamage * laserHitsPerSecond * Time.deltaTime;
  }

  // Update is called once per frame
  void Update()
  {
    firePoint = gameObject.transform.Find("FirePoint").GetComponent<Transform>().position;
    lineRenderer.SetPosition(0, firePoint);
    ActivateAim();
    CountDownAndShoot();
    DamagePlayer();
  }

  private void ActivateAim()
  {
    if (pointAtPlayer)
    {
      PlayerAimingLogic();
    }
    else
    {
      PointAtNonPlayerObject();
    }
  }
  private void PlayerAimingLogic()
  {
    int remainingShips = playerBindings.GetPlayerShips().Count - 1;
    if (lookAtPlayer && remainingShips >= 0)
    {
      PointAtPlayer();
    }
    else
    {
      transform.up = -((Vector2)lineRenderer.GetPosition(1) - (Vector2)transform.position);
    }
  }

  public void PointAtPlayer()
  {
    if (playerBindings.GetMainPlayer() != null)
    {
      playerPosition = playerBindings.GetMainPlayer().transform.position;
      transform.up = -(playerPosition - (Vector2)transform.position);

      lineRenderer.SetPosition(1, playerPosition - (Vector2)transform.up.normalized);
    }
  }
  private void PointAtNonPlayerObject()
  {
    if (nonPlayerTarget != null)
    {
      transform.up = -((Vector2)nonPlayerTarget.position - (Vector2)transform.position);
      lineRenderer.SetPosition(1, (Vector2)nonPlayerTarget.position);
      //Removed last part of the formula so the line ends at the target position and not beyond
      //lineRenderer.SetPosition(1, (Vector2)nonPlayerTarget.position - (Vector2)transform.up.normalized);
    }
  }
  private void CountDownAndShoot()
  {
    shotCounter -= Time.deltaTime;
    if (shotCounter <= 0f)
    {
      lookAtPlayer = !lookAtPlayer;
      StartCoroutine(Fire());

      shotCounter = Random.Range(minTimeBetweenShots + countDownToLaser + laserDuration, maxTimeBetweenShots + countDownToLaser + laserDuration);
    }
  }
  IEnumerator Fire()
  {
    if (lockOnSound != null) ChargeUpAnimation();
    yield return new WaitForSeconds(countDownToLaser);
    particleEmission.rateOverTime = 25;
    StartCoroutine(FireAnimation());
  }
  private void ChargeUpAnimation()
  {
    particleEmission.rateOverTime = 2000;
    AudioSource.PlayClipAtPoint(lockOnSound, Camera.main.transform.position, lockOnSoundVolume);
  }
  IEnumerator FireAnimation()
  {
    CalculateFrameAdjustedLaserDamage();
    LaserBlastSettings();
    turnOnDamage = !turnOnDamage;
    yield return new WaitForSeconds(laserDuration);
    turnOnDamage = !turnOnDamage;
    lookAtPlayer = !lookAtPlayer;
    LaserPointerSettings();
  }

  private void LaserBlastSettings()
  {
    lineRenderer.material = laserMaterial;
    lineRenderer.widthMultiplier = 1 / pointerWidthMultiplier;
    AudioSource.PlayClipAtPoint(laserSound, Camera.main.transform.position, laserSoundVolume);
  }
  private void LaserPointerSettings()
  {
    lineRenderer.material = pointerMaterial;
    lineRenderer.widthMultiplier = pointerWidthMultiplier;
  }
  private void DamagePlayer()
  {
    if (turnOnDamage)
    {
      Vector2 lineOfFire = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
      RaycastHit2D[] hit = Physics2D.RaycastAll(firePoint, lineOfFire.normalized, lineOfFire.magnitude);
      Vector3 addToZ = new Vector3(0, 0, 1);
      GameObject endpointVfx = Instantiate(endpointVfxPrefab, lineRenderer.GetPosition(1) - addToZ, Quaternion.identity);
      Destroy(endpointVfx, endpointDuration);
      for (int index = 0; index < hit.Length; index++)
      {
        Player player = hit[index].collider.GetComponent<Player>();

        if (player != null)
        {
          PlayerManager playerManager = player.GetComponent<PlayerManager>();

          if (playerManager.GetMainShipStatus())
          {
            player.ReceiveLaserDamage(laserDamageFrameAdjusted);
            GameObject damageVfx = Instantiate(damageVfxPrefab, playerManager.transform.position, Quaternion.identity);
            Destroy(damageVfx, damageDuration);
          }
        }
      }
    }
  }
  public void ChangeMinTimeBetweenShots(float newMinTime)
  {
    minTimeBetweenShots = newMinTime;
  }
  public void ChangeMaxTimeBetweenShots(float newMaxTime)
  {
    maxTimeBetweenShots = newMaxTime;
  }
}
