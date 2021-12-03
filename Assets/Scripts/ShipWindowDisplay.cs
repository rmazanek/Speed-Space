using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipWindowDisplay : MonoBehaviour
{
    //GameSession gameSession;
    PlayerBindings playerBindings;
    ShipStatsCreator shipStatsCreator;
    List<Player> playerShips;
    //List<CrewMember> crewMembers;
    [SerializeField] Image spriteToDisplay;
    
    // Start is called before the first frame update
    void Start()
    {
        //gameSession = FindObjectOfType<GameSession>();
        playerBindings = FindObjectOfType<PlayerBindings>();
        playerShips = playerBindings.PlayerShips;
        //crewMembers = playerBindings.CrewMembers;
        shipStatsCreator = FindObjectOfType<ShipStatsCreator>();
        DisplaySelectedShip(0);
        shipStatsCreator.BuildStatsList(0);
    }

    public void DisplaySelectedShip(int index)
    {
        spriteToDisplay.sprite = playerShips[index].GetComponent<SpriteRenderer>().sprite;
        gameObject.GetComponent<RectTransform>().localScale = new Vector3 (gameObject.GetComponent<RectTransform>().localScale.x, Mathf.Abs(gameObject.GetComponent<RectTransform>().localScale.y), gameObject.GetComponent<RectTransform>().localScale.z);
    }
    public void DisplaySelectedCrew(int index)
    {
        spriteToDisplay.sprite = playerBindings.CrewMembers[index].Sprite;
        gameObject.GetComponent<RectTransform>().localScale = new Vector3 (gameObject.GetComponent<RectTransform>().localScale.x, Mathf.Abs(gameObject.GetComponent<RectTransform>().localScale.y)*-1, gameObject.GetComponent<RectTransform>().localScale.z);
    }
}
