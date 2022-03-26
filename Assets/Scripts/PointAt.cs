using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{
    [SerializeField] GameObject target;
    //[SerializeField] enum Vector3 worldUp {Up, Right, Forwards};

    void Update()
    {
        gameObject.transform.LookAt(target.transform);        
    }
}
