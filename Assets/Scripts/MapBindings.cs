using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapBindings : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerInput playerInput;
    PlayerBindings playerBindings;
    //bool initializeMapObjectsNeeded = false;
    bool inventoryControlsOn = false;
    bool inventoryDisableNeeded = false;
    GameSession gameSession;
    GameObject inventory;
    MapMover mapMover;
    MapManager mapManager;
    float moveX;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerBindings = FindObjectOfType<PlayerBindings>();
    }
    private void Update()
    {
        if(inventoryDisableNeeded)
        {
            inventory = GameObject.Find("Inventory Canvas");
            inventory.SetActive(false);
            playerBindings.SwitchInputActions("Map");
            inventoryDisableNeeded = false;
        }
    }
    // Start is called before the first frame update
    public void SetPlayerControls(PlayerControls controls)
    {
        playerControls = controls;
    }
    public void InventoryDisableNeeded()
    {
        inventoryDisableNeeded = true;
    }
    public void Submit()
    {
        Debug.Log("Submit Map.");
        mapManager = FindObjectOfType<MapManager>();
        mapManager.StartSelectedLevel();
    }
    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Move Map.");
        mapMover = FindObjectOfType<MapMover>();
        if(context.started && context.ReadValue<Vector2>().x > 0)
        {
            moveX = 1f;
            mapMover.NavigateMap(moveX);
            Debug.Log("Move map right.");
        }
        else if(context.started && context.ReadValue<Vector2>().x < 0)
        {
            moveX = -1f;
            mapMover.NavigateMap(moveX);
            Debug.Log("Move map left.");
        }
    }
    public void Inventory()
    {
        inventoryControlsOn = !inventoryControlsOn;
        if(inventoryControlsOn)
        {
            Debug.Log("Disable Map controls and Enable Inventory controls.");
            inventory.SetActive(true);
            playerControls.Map.Disable();
            playerControls.Inventory.Enable();
        }
        if(!inventoryControlsOn)
        {
            Debug.Log("Disable Inventory controls and Enable Map controls.");
            inventory.SetActive(false);
            playerControls.Inventory.Disable();
            playerControls.Map.Enable();
        }
    }
    public void Menu()
    {
        Debug.Log("Menu Map.");
        PauseMenu pauseMenu = GameObject.FindObjectOfType<PauseMenu>();
        if(PauseMenu.GamePaused)
        {
            pauseMenu.ResumeGame();
        }
        else
        {
            pauseMenu.PauseGame();
        }
    }
    public void EnableMapControls()
    {
        Debug.Log("Enable Map Controls called.");
        playerInput.SwitchCurrentActionMap("Map");
    }
    public void DisableMapControls()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");
    }
}
