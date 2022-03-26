using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] float alphaShipInactive = 0.5f;
    int numberOfPlayerShips;
    GameSession gameSession;
    PlayerBindings playerBindings;
    [SerializeField] bool mainPlayerShip = false;
    bool[] mainPlayerShipStatuses;
    Color tempColor;
    ParentPositionMover parentPositionMover;
    Vector3 parentPosition;
    List<Player> playerShips;
    public int PositionIndex { get; set;}
    [SerializeField] Shield shield;
   // Start is called before the first frame update
    private void Update()
    {
        if(parentPositionMover.ShipPositionsNeedUpdate)
        {
            MoveToShipPosition();
            parentPositionMover.ShipPositionsNeedUpdate = false;
        }
        //Debug.Log("My name is " + gameObject.name + " and my mainPlayerShip status is " + mainPlayerShip);
    }
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        playerBindings = FindObjectOfType<PlayerBindings>();
        playerShips = playerBindings.GetPlayerShips();
        numberOfPlayerShips = playerBindings.GetPlayerShips().Count;
        parentPositionMover = FindObjectOfType<ParentPositionMover>();
        tempColor = GetComponent<SpriteRenderer>().color;
    }

    public void SetMainShipStatus(bool status)
    {
        mainPlayerShip = status;
        SetMainShipEffects();
    }
    public void SetMainShipEffects()
    {
        ActivateHitBox();
        ChangeShipTransparency();
    }
    public bool GetMainShipStatus()
    {
        return mainPlayerShip;
    }
    private void ActivateHitBox()
    {
        if(GetComponent<Collider2D>() != null)
        { 
            if (mainPlayerShip)
            {
                GetComponent<Collider2D>().enabled = true;
                EnableShield();
            }
            else
            {
                GetComponent<Collider2D>().enabled = false;
                DisableShield();
            }
        }
    }
    public void SetShield(Shield newShield)
    {
        shield = newShield;
    }
    void EnableShield()
    {
        if(shield.isActiveAndEnabled)
        {
            shield.EnableCollider2D();
        }
    }
    void DisableShield()
    {
        if(shield.isActiveAndEnabled)
        {
            shield.DisableCollider2D();
        }
    }
    private void ChangeShipTransparency()
    {
        if (!mainPlayerShip)
        {
            tempColor = new Color (255f, 255f, 255f, alphaShipInactive);
            
            SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if(!spriteRenderer.GetComponent<Shield>())
                {
                    spriteRenderer.color = tempColor;
                }
                else
                {
                    Color inactiveAlphaCurrentColor = new Color (spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a);
                    spriteRenderer.color = inactiveAlphaCurrentColor;
                }
            }
            
            ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem particleSystem in particleSystems)
            {
                var main = particleSystem.main;
                main.startColor = tempColor;
            }

            Image[] images = gameObject.GetComponentsInChildren<Image>();
            foreach(Image image in images)
            {
                image.color = tempColor;
            }
            //GetComponent<SpriteRenderer>().color = tempColor;
        }
        else
        {
            tempColor = new Color (255f, 255f, 255f, 1f);
            SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if(!spriteRenderer.GetComponent<Shield>())
                {
                    spriteRenderer.color = tempColor;
                }
                else
                {
                    Color fullAlphaCurrentColor = new Color (spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                    spriteRenderer.color = fullAlphaCurrentColor;
                }
            }
            
            ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem particleSystem in particleSystems)
            {
                var main = particleSystem.main;
                main.startColor = tempColor;
            }

            Image[] images = gameObject.GetComponentsInChildren<Image>();
            foreach(Image image in images)
            {
                image.color = tempColor;
            }
            //GetComponent<SpriteRenderer>().color = tempColor;
        }
    }
    public void MoveToShipPosition()
    {
        playerShips = playerBindings.GetPlayerShips();
        parentPosition = parentPositionMover.transform.position;
        int shipCount = playerShips.Count;
        var angle = 360 / shipCount;

        for(int shipNumber = 0; shipNumber < shipCount; shipNumber++)
        {
            int positionInCircle = playerShips[shipNumber].GetComponent<PlayerManager>().PositionIndex;

            Vector3 newPosition = new Vector3 (Mathf.Sin(Mathf.Deg2Rad * angle * positionInCircle), Mathf.Cos(Mathf.Deg2Rad * angle * positionInCircle), parentPosition.z);
            playerShips[shipNumber].transform.position = parentPositionMover.transform.position + (parentPositionMover.RadiusToShip * newPosition);
        }
    }
}
