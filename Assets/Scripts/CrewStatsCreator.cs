using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CrewStatsCreator : MonoBehaviour
{
    [SerializeField] SingleStatDisplay prefabSingleStatDisplay;
    PlayerBindings playerBindings;
    //List<Player> playerShips;
    List<CrewMember> crewMembers;
    Dictionary<string, string> statsList;
    // Start is called before the first frame update
    void Start()
    {
        playerBindings = FindObjectOfType<PlayerBindings>();
    }

    public void BuildStatsList(int index)
    {
        ClearOldStats();
        crewMembers = playerBindings.CrewMembers;
        statsList = crewMembers[index].GetStats();

        for (int i=0; i < statsList.Count; i++) 
        {
            SingleStatDisplay singleStatDisplay = Instantiate(prefabSingleStatDisplay);
            singleStatDisplay.transform.SetParent(gameObject.transform);
            singleStatDisplay.transform.localScale = new Vector3 (1f, 1f, 1f);
            singleStatDisplay.StatName = statsList.ElementAt(i).Key;
            singleStatDisplay.StatValue = statsList.ElementAt(i).Value;
        }
    }
    private void ClearOldStats()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }   
}
