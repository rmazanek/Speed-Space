using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipListCreator : MonoBehaviour
{
    [SerializeField] ShipDescription prefabShipContainer;
    GameSession gameSession;
    PlayerBindings playerBindings;
    List<Player> playerShips;
    int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        playerBindings = FindObjectOfType<PlayerBindings>();
        BuildList();
    }

    private void BuildList()
    {
        playerShips = playerBindings.PlayerShips;

        foreach (Player p in playerShips) {
            ShipDescription newShipStatsContainer = Instantiate(prefabShipContainer);
            newShipStatsContainer.transform.SetParent(gameObject.transform);
            newShipStatsContainer.Name = p.name.ToString();
            newShipStatsContainer.MaxHitPoints = p.GetMaxHealth().ToString();
            //newShipStatsContainer.CrewSize = p.GetCrewSize().ToString();
            //newShipStatsContainer.CrewSizeMax = p.GetMaxCrewSize().ToString();
            newShipStatsContainer.RoundsSurvived = p.RoundsSurvived.ToString();
            newShipStatsContainer.ShipSprite = p.GetShipSprite();
            newShipStatsContainer.Index = index;
            index++;
        }
    }
}
