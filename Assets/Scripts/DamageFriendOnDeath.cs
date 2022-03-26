using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFriendOnDeath : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField, Range(0f,1f)] float percentMaxHealthDamageToFriend = 0.30f;
    [SerializeField] bool usePercent = true;
    [SerializeField] int flatDamageToFriend = 100;
    public void DoDamage()
    {
        if(usePercent)
        {
            float damageToDeal = enemy.GetMaxHealth() * percentMaxHealthDamageToFriend;
            enemy.ReceiveDamage((int)damageToDeal);
        }
        else
        {
            enemy.ReceiveDamage((int)flatDamageToFriend);
        }
    }
}
