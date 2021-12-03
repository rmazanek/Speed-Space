using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpriteBuilder : MonoBehaviour
{
    [SerializeField] Sprite[] shipSprites;
    [SerializeField] bool equalChances;
    [SerializeField] int[] weights;
    Sprite shipSprite;
    Sprite spriteToUpdate;
    public void ChooseAndAssignSprite(SpriteRenderer spriteRenderer)
    {
        shipSprite = ChooseShipSprite(Random.Range(0, GetTotalWeight()));
        AssignSprite(spriteRenderer, shipSprite);
    }
    private void AssignSprite(SpriteRenderer spriteRenderer, Sprite sprite)
    {
        //Debug.Log(spriteRenderer.sprite.name);
        spriteRenderer.sprite = sprite;
        //Debug.Log(sprite.name);
    }
    private Sprite ChooseShipSprite(int rand)
    {
        //Debug.Log("Sprite random number chosen is " + rand + " out of " + GetTotalWeight());
        for(int i = 0; i < weights.Length; i++)
        {
            if(rand <= weights[i])
            {
                return shipSprites[i];
            }
            else
            {
                rand -= weights[i];
                continue;
            }
        }
        
        return null;
    }
    private int GetTotalWeight()
    {
        int totalWeight = 0;

        for (int i = 0; i < shipSprites.Length; i++)
            {
                totalWeight += weights[i];
            }
        
        return totalWeight;
    }
}
