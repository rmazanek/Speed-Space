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
    
    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        levelManager = FindObjectOfType<LevelManager>();
        levelList = levelManager.GetLevelList();
        GetWaypoints();
        if(isShop)
        {
            colliders = gameObject.GetComponentsInChildren<Collider2D>();
            foreach(Collider2D collider in colliders)
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
        if(waveConfig != null)
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
    private void Move()
    {
        currentLevel = levelList[gameSession.GetLevelToStart()-1];
        if(canMove)
        {
            if(waypointIndex <=  waypoints.Count - 1)
            {
                var targetPosition = waypoints[waypointIndex].transform.position;
                var movementThisFrame = waveConfig.GetMoveSpeed() * currentLevel.EnemyMoveSpeedCumulativeMultiplier * Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);

                if(transform.position == targetPosition)
                {
                    waypointIndex++;
                }
            }
            else
            {
                if(loopRoute)
                {
                    waypointIndex = 0;
                    Move();
                }
                else if(destroyAtEnd)
                {
                    Destroy(gameObject);
                }
                else if(isShop)
                {
                    foreach(Collider2D collider in colliders)
                    {
                        collider.enabled = true;
                    }
                    isShop = false;
                }
                else if(teleportBackToStart)
                {
                    waypointIndex = 0;
                    transform.position = waypoints[waypointIndex].transform.position;
                }
            }
        }
    }
}
