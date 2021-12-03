using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipDescription : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI shipName;
    [SerializeField] TextMeshProUGUI maxHitPoints;
    [SerializeField] TextMeshProUGUI roundsSurvived;
    [SerializeField] GameObject shipImageContainer;
    [SerializeField] GameObject shipContents;
    [SerializeField] ShipItemIndicator shipItemIndicatorPrefab;
    public string Name {get; set;}
    public string MaxHitPoints {get; set;}
    public string RoundsSurvived {get; set;}
    public Sprite ShipSprite {get; set;}
    ShipWindowDisplay shipWindowDisplay;
    ShipStatsCreator shipStatsCreator;
    public int Index {get; set;}
    PlayerBindings playerBindings;
    // Start is called before the first frame update
    void Start()
    {
        shipImageContainer.GetComponent<Image>().sprite = ShipSprite;
        shipName.text = Name;
        maxHitPoints.text = MaxHitPoints;
        roundsSurvived.text = RoundsSurvived;
        transform.localScale = new Vector3(1,1,1);
        shipWindowDisplay = GameObject.FindObjectOfType<ShipWindowDisplay>();
        shipStatsCreator = GameObject.FindObjectOfType<ShipStatsCreator>();
        playerBindings = FindObjectOfType<PlayerBindings>();
        InstantiateShipContentIcons();
    }

    public void DisplayShip()
    {
        shipWindowDisplay.DisplaySelectedShip(Index);
        shipStatsCreator.BuildStatsList(Index);
    }
    private void InstantiateShipContentIcons()
    {
        List<ShipModifier> shipModifiers = playerBindings.PlayerShips[Index].ShipModifiers;
        if(shipModifiers != null)
        {
            foreach(ShipModifier shipModifier in shipModifiers)
            {
                ShipItemIndicator shipModifierIcon = Instantiate(shipItemIndicatorPrefab);
                shipModifierIcon.transform.SetParent(shipContents.transform);
                shipModifierIcon.GetComponent<Image>().sprite = shipModifier.Sprite;
                shipModifierIcon.ModifierTextMesh.text = shipModifier.Modifier;
                shipModifierIcon.transform.localScale = new Vector3(1,1,1);
            }
        }
    }
}
