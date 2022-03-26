using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropHandler : MonoBehaviour
{
    [Header("Drops")]
    [SerializeField] GameObject[] items;
    [SerializeField] int[] weights;
    GameSession gameSession;

    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        weights[0] = Mathf.FloorToInt((float)weights[0] * (1 - gameSession.PlusLuckModifier)); // reduce the chance of getting nothing
    }
    private int GetTotalWeight()
    {
        int totalWeight = 0;

        foreach(int w in weights)
        {
            totalWeight += w;
        }

        return totalWeight;
    }

    private GameObject GetItem(int rand)
    {
        for(int index = 0; index < weights.Length; index++)
        {
            if(rand <= weights[index])
            {
                return items[index];
            }
            else
            {
                rand -= weights[index];
                continue;
            }
        }

        return null;
    }
    public void DropItem()
    {
        int rand = Random.Range(0, GetTotalWeight());
        GameObject prefabToInstantiate = GetItem(rand);
        if(prefabToInstantiate.tag != "EmptyItem")
        {
            GameObject dropItem = Instantiate(prefabToInstantiate, gameObject.transform.position, Quaternion.identity);
            dropItem.GetComponent<Rigidbody2D>().velocity = new Vector2 (0f, -gameSession.DropSpeed);
        }
    }
    public int GetNumberOfItems()
    {
        return items.Length;
    }
    public void SetItemWeights(int[] weightArray)
    {
        for(int i = 0; i < items.Length; i++)
        {
            weights[i] = weightArray[i];
        }
    }
}
