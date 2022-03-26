using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreDisplay : MonoBehaviour
{
    // Cached refs
    GameSession gameSession;
    
    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        UpdateScore();
    }
    public void UpdateScore()
    {
        GetComponent<TextMeshProUGUI>().text = gameSession.GetScore().ToString();
    }
}
