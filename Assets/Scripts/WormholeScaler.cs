using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormholeScaler : MonoBehaviour
{
    [SerializeField] float maxSize = 100f;
    [SerializeField] float growFactor = 100f;
    [SerializeField] float maxDistanceDelta = 10f;
    [SerializeField] float moveSpeed = 10f;
    GameObject parentGameObject;
    private void Start()
    {
        parentGameObject = gameObject.transform.parent.gameObject;
    }
    public void WormholeTransition()
    {
        StartCoroutine(WormholeScaleCoroutine());
    }

    IEnumerator WormholeScaleCoroutine()
    {
        while(maxSize > gameObject.transform.localScale.x)
        {
            Vector3 middleOfViewPort = new Vector3(0.5f, 0.5f, 0f);
            Vector3 middleOfWorldSpace = Camera.main.ViewportToWorldPoint(middleOfViewPort);
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, middleOfWorldSpace, maxDistanceDelta * Time.deltaTime * moveSpeed);
            gameObject.transform.localScale += new Vector3(10,10,0) * Time.deltaTime * growFactor;
            yield return null;   
        }
    }
}
