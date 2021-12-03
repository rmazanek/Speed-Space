using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePingPong : MonoBehaviour
{
    [SerializeField] float delay = 0f;
    [SerializeField] float period = 2f;
    [SerializeField] float distanceToPingPong = 10f;
    [SerializeField] Vector3 startDirection;
    Vector3 startPosition;
    float timePassed;
    // Start is called before the first frame update
    void Start()
    {
        timePassed = 0f - delay;
        startDirection = startDirection.normalized;
        startPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(timePassed > 0f)
        {
            gameObject.transform.position = NewPosition();
        }
        timePassed += Time.deltaTime;
    }
    private Vector3 NewPosition()
    {
        float timePassedNormalized = timePassed * 2 * Mathf.PI;
        Vector3 newPosition = startPosition + startDirection * distanceToPingPong * Mathf.Sin(timePassedNormalized / period);
        return newPosition;
    }
}
