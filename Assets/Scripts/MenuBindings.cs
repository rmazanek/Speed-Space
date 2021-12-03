using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuBindings : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerControls playerControls;
    List<Button> buttons = new List<Button>();
    int buttonSelectedIndex = 0;
    int maxButtonSelectedIndex = 0;
    bool startNeeded = false;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        playerControls = new PlayerControls();
        playerControls.UI.Enable();
    }
    private void Start()
    {
        ListButtons();
        SelectButton(buttonSelectedIndex);
    }
    private void Update()
    {
        if(startNeeded)
        {
            buttonSelectedIndex = 0;
            ListButtons();
            SelectButton(buttonSelectedIndex+1);
            startNeeded = false;
        }
    }
    public void Submit(InputAction.CallbackContext context)
    {
        Debug.Log("Submit UI.");
        //Doesn't work
        //buttons[buttonSelectedIndex].onClick.Invoke();
    }
    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Move UI.");
        if(context.started)
        {
            AddToButtonSelectedIndex();
            SelectButton(buttonSelectedIndex);
        }
    }
    public void Back()
    {
        Debug.Log("Back UI.");
    }
    private void ListButtons()
    {
        buttons.Clear();
        foreach(Button button in GameObject.FindObjectsOfType<Button>())
        {
           //Debug.Log(button.name);
           //Debug.Log(buttons.Count);
           buttons.Add(button);
        }
        maxButtonSelectedIndex = buttons.Count;
    }
    private void SelectButton(int index)
    {
        if(buttons[index] != null)
        {
            buttons[index].GetComponent<Button>().Select();
        }
    }
    private void AddToButtonSelectedIndex()
    {
        if((buttonSelectedIndex + 1) >= maxButtonSelectedIndex)
        {
            buttonSelectedIndex = 0;
        }
        else
        {
            buttonSelectedIndex += 1;
        }
    }
    public void EnableUIControls()
    {
        Debug.Log("Enable UI Controls called.");
        playerControls.UI.Enable();
        startNeeded = true;
    }
    public void DisableUIControls()
    {
        playerControls.UI.Disable();
    }
}
