using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
    [SerializeField] float maxSize = 10f;
    [SerializeField] float growFactor = 2f;
    private void Start()
    {
        Scale();
    }
    public void Scale()
    {
        StartCoroutine(ScaleCoroutine());
    }

    IEnumerator ScaleCoroutine()
    {
        while(maxSize > gameObject.transform.localScale.x)
        {
            gameObject.transform.localScale += new Vector3(1,1,0) * Time.deltaTime * growFactor;
            yield return new WaitForEndOfFrame();   
        }
    }
}
