using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float shakeMagnitude = 0.1f;
    Vector3 initialPosition;
    public void Play()
    {
        initialPosition = transform.position;
        StartCoroutine(ShakeMe());
    }
    IEnumerator ShakeMe()
    {
        float timer = shakeDuration;
        while(timer >= 0)
        {
            transform.position = initialPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = initialPosition;
    }
}
