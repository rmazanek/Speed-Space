using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfChildless : MonoBehaviour
{
    void Update()
    {
        if(gameObject.transform.childCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
