using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPositionMover : MonoBehaviour
{
    float xPositionMin;
    float xPositionMax;
    float yPositionMin;
    float yPositionMax;
    ///////PlayerControls controls;
    Vector2 move;
    [SerializeField] float yPositionMinViewPort;
    [SerializeField] float yPositionMaxViewPort;
    [SerializeField] public float RadiusToShip = 1f;
    [SerializeField] bool radiusToShipUsesFormula;
    [SerializeField] float logCoefficient = 7/4;
    public bool ShipPositionsNeedUpdate = true;

    private void Awake()
    {
        //////controls = new PlayerControls();
        //////controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        //////controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;
        //Debug.Log(gameObject.name + " just woke up with " + gameObject.transform.childCount + " children.");
        //Debug.Log("There were " + FindObjectsOfType(GetType()).Length + " of type " + GetType().ToString());

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        SetUpMoveBoundaries();
        RadiusToShipUpdate();
    }

    public void Move(float moveX, float moveY)
    {
        //var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * xMovementSensitivity;
        //var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * yMovementSensitivity;
        
        Vector2 delta = new Vector2(moveX, moveY);

        //var newXPos = Mathf.Clamp(transform.position.x + deltaX, xPositionMin, xPositionMax);
        //var newYPos = Mathf.Clamp(transform.position.y + deltaY, yPositionMin, yPositionMax);

        //transform.position = new Vector2(newXPos, newYPos);
        if(transform != null)
        {
            transform.Translate(delta, Space.World);

            var newXPos = Mathf.Clamp(transform.position.x, xPositionMin, xPositionMax);
            var newYPos = Mathf.Clamp(transform.position.y, yPositionMin, yPositionMax);

            transform.position = new Vector2(newXPos, newYPos);
        }
    }
    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xPositionMin = gameCamera.ViewportToWorldPoint(new Vector3(0,0,0)).x;
        xPositionMax = gameCamera.ViewportToWorldPoint(new Vector3(1,0,0)).x;
        yPositionMin = gameCamera.ViewportToWorldPoint(new Vector3(0,yPositionMinViewPort,0)).y;
        yPositionMax = gameCamera.ViewportToWorldPoint(new Vector3(0,yPositionMaxViewPort,0)).y;
    }

    public void RadiusToShipUpdate()
    {
        if(radiusToShipUsesFormula)
        {
            //GameSession gameSession = FindObjectOfType<GameSession>();
            PlayerBindings playerBindings = FindObjectOfType<PlayerBindings>();
            float newRadius = Mathf.Log(playerBindings.GetPlayerShips().Count)*logCoefficient;
            RadiusToShip = newRadius;
        }
        ShipPositionsNeedUpdate = true;
    }
}
