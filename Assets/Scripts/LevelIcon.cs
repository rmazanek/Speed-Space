using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIcon : MonoBehaviour
{
    MapManager mapManager;
    public int LevelIconIndex {get; set;}
    // Start is called before the first frame update
    void Start()
    {
        mapManager = FindObjectOfType<MapManager>();
        //mapManager.CountLevelIcons();
        //LevelIconIndex = mapManager.LevelIconCount;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Other name " + other.name);
        if(other.gameObject.GetComponent<MapMover>() != null)
        {
            mapManager = FindObjectOfType<MapManager>();
            mapManager.LevelIndex = LevelIconIndex;
            //Debug.Log("Telling mapManager that LevelIndex is " + LevelIconIndex);
            mapManager.OnMapIcon = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        mapManager.OnMapIcon = false;
    }
}
