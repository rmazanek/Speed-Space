using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapBindings : MonoBehaviour
{
  PlayerControls playerControls;
  PlayerInput playerInput;
  PlayerBindings playerBindings;
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
    if (inventoryDisableNeeded)
    {
      inventory = GameObject.Find("Inventory Canvas");
      inventory.SetActive(false);
      playerBindings.SwitchInputActions("Map");
      inventoryDisableNeeded = false;
    }
  }
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
    mapManager = FindObjectOfType<MapManager>();
    mapManager.StartSelectedLevel();
  }
  public void Move(InputAction.CallbackContext context)
  {
    mapMover = FindObjectOfType<MapMover>();
    if (context.started && context.ReadValue<Vector2>().x > 0)
    {
      moveX = 1f;
      mapMover.NavigateMap(moveX);
    }
    else if (context.started && context.ReadValue<Vector2>().x < 0)
    {
      moveX = -1f;
      mapMover.NavigateMap(moveX);
    }
  }
  public void Inventory()
  {
    inventoryControlsOn = !inventoryControlsOn;
    if (inventoryControlsOn)
    {
      inventory.SetActive(true);
      playerControls.Map.Disable();
      playerControls.Inventory.Enable();
    }
    if (!inventoryControlsOn)
    {
      inventory.SetActive(false);
      playerControls.Inventory.Disable();
      playerControls.Map.Enable();
    }
  }
  public void Menu()
  {
    PauseMenu pauseMenu = GameObject.FindObjectOfType<PauseMenu>();
    if (PauseMenu.GamePaused)
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
    playerInput.SwitchCurrentActionMap("Map");
  }
  public void DisableMapControls()
  {
    playerInput = FindObjectOfType<PlayerInput>();
    playerInput.SwitchCurrentActionMap("UI");
  }
}
