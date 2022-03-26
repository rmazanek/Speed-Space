using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
  GameSession gameSession;
  LevelManager levelManager;
  List<LevelContainer> levelList;
  LevelContainer currentLevel;
  WaveConfig waveConfig;
  List<Transform> waypoints;
  int waypointIndex = 0;
  bool canMove = true;
  [SerializeField] bool isShop = false;
  [SerializeField] bool loopRoute = false;
  [SerializeField] bool destroyAtEnd = true;
  [SerializeField] bool teleportBackToStart = false;
  Collider2D[] colliders;
  float temporarySpeedFactor = 1f;
  [SerializeField] float minimumTemporarySpeedFactor = 0.15f;
  [SerializeField] float maximumTemporarySpeedFactor = 1.0f;
  BossDuties isBoss;
  public bool PartOfArrangement = false;
  public int ArrangementIndex = 0;
  private Vector3 waypointOffset;
  private Enemy enemy;
  // Start is called before the first frame update
  void Start()
  {
    gameSession = FindObjectOfType<GameSession>();
    isBoss = gameObject.GetComponent<BossDuties>();
    enemy = gameObject.GetComponent<Enemy>();
    levelManager = FindObjectOfType<LevelManager>();
    levelList = levelManager.GetLevelList();
    GetWaypoints();
    waypointOffset = WaypointOffset();
    if (isShop)
    {
      colliders = gameObject.GetComponentsInChildren<Collider2D>();
      foreach (Collider2D collider in colliders)
      {
        collider.enabled = false;
      }
    }
  }

  // Update is called once per frame
  void Update()
  {
    Move();
  }

  private void GetWaypoints()
  {
    if (waveConfig != null)
    {
      waypoints = waveConfig.GetWaypoints();
      transform.position = waypoints[waypointIndex].transform.position;
    }
    else
    {
      canMove = false;
    }
  }
  public void SetWaveConfig(WaveConfig waveConfig)
  {
    this.waveConfig = waveConfig;
  }
  public bool IsShop()
  {
    return isShop;
  }
  private void Move()
  {
    currentLevel = levelList[gameSession.GetLevelToStart() - 1];
    if (canMove)
    {
      if (waypointIndex <= waypoints.Count - 1)
      {
        var targetPosition = waypoints[waypointIndex].transform.position + waypointOffset;
        var movementThisFrame = waveConfig.GetMoveSpeed() * currentLevel.EnemyMoveSpeedCumulativeMultiplier * temporarySpeedFactor * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);

        if (transform.position == targetPosition)
        {
          waypointIndex++;
        }
      }
      else
      {
        if (loopRoute)
        {
          waypointIndex = 0;
          Move();
        }
        else if (destroyAtEnd)
        {
          if (!enemy)
          {
            Destroy(gameObject);
          }
          else
          {
            enemy.DestroyWithoutReward();
          }
        }
        else if (isShop)
        {
          foreach (Collider2D collider in colliders)
          {
            collider.enabled = true;
          }
          isShop = false;
        }
        else if (teleportBackToStart)
        {
          waypointIndex = 0;
          transform.position = waypoints[waypointIndex].transform.position;
        }
      }
    }
  }
  IEnumerator MultiplySpeedByFactorTemporarilyCoroutine(float factor, float timeInSeconds)
  {
    temporarySpeedFactor = Mathf.Clamp(temporarySpeedFactor * factor, minimumTemporarySpeedFactor, maximumTemporarySpeedFactor);
    yield return new WaitForSeconds(timeInSeconds);
    temporarySpeedFactor = Mathf.Clamp(temporarySpeedFactor / factor, minimumTemporarySpeedFactor, maximumTemporarySpeedFactor);
  }
  public void MultiplySpeedByFactorTemporarily(float factor, float timeInSeconds)
  {
    if (isBoss == null)
    {
      StartCoroutine(MultiplySpeedByFactorTemporarilyCoroutine(factor, timeInSeconds));
    }
  }
  public bool CanMove()
  {
    return canMove;
  }
  public Vector3 WaypointOffset()
  {
    ChildArranger childArranger = gameObject.GetComponentInParent<ChildArranger>();

    if (PartOfArrangement && childArranger != null)
    {
      Vector3 initialOffset = childArranger.InitialOffset;
      Vector3 offset = childArranger.Offset;
      return -(offset * ArrangementIndex) / 2;
    }
    else
    {
      return new Vector3(0f, 0f, 0f);
    }
  }
}
