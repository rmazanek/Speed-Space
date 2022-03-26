using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownOthers : MonoBehaviour
{
  [SerializeField] float slowFactor = 0.60f;
  [SerializeField] float slowTime = 0.50f;
  [SerializeField] GameObject particleEffectToApplyPrefab;
  EnemyPathing otherPathing;
  bool isBoss;
  public void ApplySlowFactor(Collider2D other)
  {
    if (CanBeSlowed(other))
    {
      otherPathing.MultiplySpeedByFactorTemporarily(slowFactor, slowTime);
      DisplaySlowEffect(other.gameObject);
    }
    else
    {
      if (!IsBoss(other) && IsEnemy(other))
      {
        if (IsBoid(other))
        {
          other.GetComponentInParent<BoidBaseBehavior>().MultiplySpeedByFactorTemporarily(slowFactor, slowTime);
        }
        else
        {
          other.GetComponent<Enemy>().MultiplySpeedByFactorTemporarily(slowFactor, slowTime);
        }
        DisplaySlowEffect(other.gameObject);
      }
    }
  }

  private bool IsEnemy(Collider2D other)
  {
    return other.gameObject.GetComponent<Enemy>() != null;
  }

  private void DisplaySlowEffect(GameObject attackedGameObject)
  {
    Snowflakes snowflakeEffect = attackedGameObject.GetComponentInChildren<Snowflakes>();
    if (snowflakeEffect != null)
    {
      snowflakeEffect.ShowParticles(slowTime);
    }
    else
    {
      GameObject particleEffect = Instantiate(particleEffectToApplyPrefab, attackedGameObject.transform);
      particleEffect.transform.SetParent(attackedGameObject.transform);
      Sprite shapeToChange = particleEffect.GetComponent<ParticleSystem>().shape.sprite;
      shapeToChange = attackedGameObject.GetComponent<SpriteRenderer>().sprite;
      particleEffect.GetComponent<Snowflakes>().ShowParticles(slowTime);
    }
  }
  private bool CanBeSlowed(Collider2D collider)
  {
    if (isBoss) return false;
    otherPathing = collider.gameObject.GetComponent<EnemyPathing>();
    if (otherPathing == null)
    {
      return false;
    }
    else
    {
      return otherPathing.CanMove();
    }
  }
  private bool IsBoss(Collider2D collider)
  {
    return collider.gameObject.GetComponent<BossDuties>() != null;
  }
  private bool IsBoid(Collider2D collider)
  {
    return collider.gameObject.GetComponentInParent<BoidBaseBehavior>() != null;
  }
}
