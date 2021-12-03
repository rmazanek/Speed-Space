using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NudgeOverTime : MonoBehaviour
{
    [SerializeField] int pixelsToMove = 20;
    [SerializeField] float duration = 0.2f;
    [SerializeField] Vector3 direction = new Vector3 (1f, 1f, 0f);
    float pixelsMovedThisFrame;
    float pixelsMoved = 0f;
    // Update is called once per frame
    void Update()
    {
        if (pixelsMoved < pixelsToMove)
        {
            pixelsMovedThisFrame = Time.deltaTime / duration * pixelsToMove / 100f;
            transform.position += direction * pixelsMovedThisFrame;
            pixelsMoved += pixelsMovedThisFrame;
        }
    }
}
