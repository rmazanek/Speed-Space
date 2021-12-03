using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePathing : MonoBehaviour
{
    [Header("Projectile")]
    float directionChangeTimer;
    [SerializeField] float minTimeBetweenDirectionChange = 0.01f;
    [SerializeField] float maxTimeBetweenDirectionChange = 0.01f;
    [SerializeField] float safeDistance = 1f;
    [SerializeField] float timeBeforeExplosion = 3f;
    [SerializeField] float projectileSpeedTowardsPlayer = 10f;
    [SerializeField] bool startUpsideDown;
    enum TargetType {Player, Enemy};
    [SerializeField] TargetType targetType;
    GameObject target;
    DamageDealer damageDealer;
    float detonationTimer;
    bool lockOn;
    // Start is called before the first frame update
    void Start()
    {
        if(GetTargetReference() != null)
        {
            target = GetTargetReference();
        }
        //target = FindObjectOfType<Player>();
        damageDealer = FindObjectOfType<DamageDealer>();
        directionChangeTimer = Random.Range(minTimeBetweenDirectionChange, maxTimeBetweenDirectionChange);
        detonationTimer = timeBeforeExplosion;
        lockOn = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        ExplosionCheck();
        if(lockOn && target != null)
        {
            ChangeDirection();
        }
        AngleToPlayer();
    }
    private void AngleToPlayer()
    {
        if(GetTargetReference() != null)
        {
            target = GetTargetReference();
        }
        //target = FindObjectOfType<Player>();
        
        if(target != null)
        {
            float angleToPlayer = Vector3.Angle(Vector3.up, target.transform.position);

            gameObject.transform.Rotate(Vector3.forward, angleToPlayer * Time.deltaTime);
        }
    }
    private void ExplosionCheck()
    {
        detonationTimer -= Time.deltaTime;

        if(detonationTimer <= 0)
        {
            if(damageDealer != null)
            {
                damageDealer.PlayHitEffects();
            }
            
            Destroy(gameObject);
        }
    }

    private void ChangeDirection()
    {
        if(GetTargetReference() != null)
        {
            target = GetTargetReference();
        }
        //target = FindObjectOfType<Player>();

        if(target != null)
        {
            if((transform.position - target.transform.position).magnitude > safeDistance)
            {
                directionChangeTimer -= Time.deltaTime;
                float movementThisFrame = projectileSpeedTowardsPlayer * Time.deltaTime;
                Quaternion targetRotation = Quaternion.AngleAxis(Vector3.Angle(transform.position, target.transform.position), Vector3.forward);

                if(directionChangeTimer <= 0)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, movementThisFrame);
                    directionChangeTimer = Random.Range(minTimeBetweenDirectionChange, maxTimeBetweenDirectionChange);
                }
            }
            else
            {
                lockOn = false;
            }
        }
    }
    private GameObject GetTargetReference()
    {
        if(targetType == TargetType.Player)
        {
            if(GameObject.FindObjectOfType<Player>() != null)
            {
                return GameObject.FindObjectOfType<Player>().gameObject;
            }
        }
        if(targetType == TargetType.Enemy)
        {
            if(GameObject.FindObjectOfType<Enemy>() != null)
            {
                return GameObject.FindObjectOfType<Enemy>().gameObject;
            }
        }
        return null;
    }
}
