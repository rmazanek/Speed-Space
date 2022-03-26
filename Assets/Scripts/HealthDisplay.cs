using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    Player player;
    //GameSession gameSession;
    PlayerBindings playerBindings;
    public void UpdateHealth()
    {
        //gameSession = FindObjectOfType<GameSession>();
        playerBindings = FindObjectOfType<PlayerBindings>();
        player = playerBindings.GetMainPlayer();
        this.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(player.GetHealth()).ToString();
    }
}
