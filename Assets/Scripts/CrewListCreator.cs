using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CrewListCreator : MonoBehaviour
{
    [SerializeField] CrewDescription prefabCrewContainer;
    GameSession gameSession;
    PlayerBindings playerBindings;
    List<CrewMember> crewMembers;
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
        crewMembers = playerBindings.CrewMembers;
        //Debug.Log(crewMembers.Count);

        foreach (CrewMember p in crewMembers) {
            CrewDescription newCrewStatsContainer = Instantiate(prefabCrewContainer);
            newCrewStatsContainer.transform.SetParent(gameObject.transform);
            newCrewStatsContainer.Name = p.name.ToString();
            newCrewStatsContainer.CrewSprite = p.GetCrewSprite();
            newCrewStatsContainer.ShipSprite = p.GetShipSprite();
            newCrewStatsContainer.Index = index;
            index++;
        }
    }
    //private List<CrewMember> GetCrewMembers()
    //{
    //    List<CrewMember> crewMembers = new List<CrewMember>();
//
    //    for(int i=0; i < playerBindings.PlayerShips.Count; i++)
    //    {
    //        crewMembers.AddRange(playerBindings.PlayerShips[i].GetCrewMembersList());
    //    }
    //    return crewMembers;
    //}
}
