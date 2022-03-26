using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReliantObjects : MonoBehaviour
{
    [SerializeField] bool destroyIfMissing = true;
    [SerializeField] List<GameObject> reliantObjects;
    int missingObjects = 0;
    // Update is called once per frame
    void Update()
    {
        foreach (GameObject gameObject in reliantObjects)
        {
            if(gameObject==null)
            {
                missingObjects++;
            }
        }
        if(missingObjects == reliantObjects.Count && destroyIfMissing)
        {
            Destroy(gameObject);
        }
        missingObjects = 0;
    }
}
