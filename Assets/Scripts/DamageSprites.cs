using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSprites : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] GameObject[] objectsWithSprites;
    [SerializeField] float[] healthPctBreakpoints;
    float enemyHealthPct;
    int spritesApplied = 0;
    Shake shake;
    // Start is called before the first frame update
    void Start()
    {
        shake = gameObject.GetComponent<Shake>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyHealthPct = enemy.GetHealth() / enemy.GetMaxHealth();

        if(spritesApplied < objectsWithSprites.Length)
        {
            if(enemyHealthPct < healthPctBreakpoints[spritesApplied])
            {
                objectsWithSprites[spritesApplied].SetActive(true);
                shake?.Play();
                spritesApplied++;
            }
        }
    }
}
