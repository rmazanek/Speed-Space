using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    PlayerInput playerInput;
    MapBindings mapBindings;
    PlayerBindings playerBindings;
    // Start is called before the first frame update
    void OnEnable()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerBindings = FindObjectOfType<PlayerBindings>();
        playerInput.SwitchCurrentActionMap("UI");
        //mapBindings.DisableMapControls();
    }
    //void OnDisable()
    //{
    //    mapBindings = FindObjectOfType<MapBindings>();
    //    mapBindings.InventoryDisableNeeded();
    //      playerBindings.SwitchInputActions("Map");
    //}
}
