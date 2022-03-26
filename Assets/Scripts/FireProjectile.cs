using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] float selfDestructTimer = 0.4f;
    void Start()
    {
        StartCoroutine(SelfDestruct());        
    }
    IEnumerator SelfDestruct()
    {
        float timer = selfDestructTimer;
        while(timer >= 0f)
        {
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
