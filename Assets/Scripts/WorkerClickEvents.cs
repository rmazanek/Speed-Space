using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerClickEvents : MonoBehaviour
{
    float scalePercent = 0.4f;
    public void ClickScaleUp()
    {
        gameObject.transform.localScale = gameObject.transform.localScale * (1 + scalePercent);
    }
}
