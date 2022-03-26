using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NameList : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] TextAsset namesTextFile;
    [SerializeField] public string[] NamesList;

    void OnValidate() =>
        NamesList = namesTextFile 
        ? namesTextFile.text.Split(separator: new []{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries) 
        : null;
}
