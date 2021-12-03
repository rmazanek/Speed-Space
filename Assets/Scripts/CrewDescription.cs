using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrewDescription : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI crewName;
    [SerializeField] TextMeshProUGUI roundsSurvived;
    [SerializeField] GameObject crewImageContainer;
    [SerializeField] GameObject shipImageContainer;
    public string Name {get; set;}
    public string MaxHitPoints {get; set;}
    public string RoundsSurvived {get; set;}
    public Sprite CrewSprite {get; set;}
    public Sprite ShipSprite {get; set;}
    ShipWindowDisplay windowDisplay;
    CrewStatsCreator crewStatsCreator;
    public int Index {get; set;}
    // Start is called before the first frame update
    void Start()
    {
        crewImageContainer.GetComponent<Image>().sprite = CrewSprite;
        shipImageContainer.GetComponent<Image>().sprite = ShipSprite;
        crewName.text = Name;
        //maxHitPoints.text = MaxHitPoints;
        roundsSurvived.text = RoundsSurvived;
        transform.localScale = new Vector3(1,1,1);
        windowDisplay = GameObject.FindObjectOfType<ShipWindowDisplay>();
        crewStatsCreator = GameObject.FindObjectOfType<CrewStatsCreator>();
    }

    public void DisplayCrew()
    {
        //Debug.Log(windowDisplay.name);
        //Debug.Log(Index);
        //Debug.Log(crewStatsCreator.name);
        windowDisplay.DisplaySelectedCrew(Index);
        crewStatsCreator.BuildStatsList(Index);
    }
}
