using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SingleStatDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statName;
    [SerializeField] TextMeshProUGUI statValue;
    public string StatName {get; set;}
    public string StatValue {get; set;}
    // Start is called before the first frame update
    void Start()
    {
        statName.text = StatName;
        statValue.text = StatValue;
    }
}
