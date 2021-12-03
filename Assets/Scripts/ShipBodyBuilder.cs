using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBodyBuilder : MonoBehaviour
{
    [SerializeField] ShipSpriteBuilder[] shipBodies;
    [SerializeField] bool equalChances;
    [SerializeField] int[] weights;
    ShipSpriteBuilder shipBody;
    PolygonCollider2D attachedCollider2D;
    Vector2[] colliderPointsToUpdate;
    // Start is called before the first frame update
    void Awake()
    {
        shipBody = ChooseShipSpriteBuilder(Random.Range(0, GetTotalWeight()));
        BuildCollider(shipBody);
        BuildSprite(shipBody);
        Destroy(this);
    }
    private void BuildCollider(ShipSpriteBuilder shipSpriteBuilder)
    {
        attachedCollider2D = gameObject.GetComponent<PolygonCollider2D>();

        if(attachedCollider2D != null)
        {
            //Debug.Log("There was already a collider, called: " + attachedCollider2D.name);
            colliderPointsToUpdate = gameObject.GetComponent<PolygonCollider2D>().GetPath(0);
            colliderPointsToUpdate = shipSpriteBuilder.GetComponent<PolygonCollider2D>().GetPath(0);
        }
        else
        {
            PolygonCollider2D newCollider = gameObject.AddComponent<PolygonCollider2D>();
            Vector2[] newPath = shipSpriteBuilder.GetComponent<PolygonCollider2D>().GetPath(0);
            newCollider.SetPath(0,newPath);
            //Debug.Log("There was no collider. New path set: " + newPath);
        }
    }
    private void BuildSprite(ShipSpriteBuilder shipSpriteBuilder)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        shipSpriteBuilder.ChooseAndAssignSprite(spriteRenderer);
    }
    private ShipSpriteBuilder ChooseShipSpriteBuilder(int rand)
    {
        //Debug.Log("ShipSpriteBuilder random number chosen is " + rand + " out of " + GetTotalWeight());
        for(int i = 0; i < weights.Length; i++)
        {
            if(rand <= weights[i])
            {
                return shipBodies[i];
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

        for (int i = 0; i < shipBodies.Length; i++)
            {
                totalWeight += weights[i];
            }
        
        return totalWeight;
    }
}
