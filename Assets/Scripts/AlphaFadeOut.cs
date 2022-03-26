using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlphaFadeOut : MonoBehaviour
{
    [SerializeField] float duration = 2f;
    [SerializeField] float alphaEnd = 0f;
    TextMeshPro textMeshProObject;    
    float fadePercent;
    Color objectColor;
    Color newColor;
    float newAlpha;
    float timePassed;
    private void Start()
    {
        textMeshProObject = gameObject.GetComponent<TextMeshPro>();
        objectColor = gameObject.GetComponent<TextMeshPro>().color;
        newAlpha = objectColor.a;
    }
    // Update is called once per frame
    void Update()
    {
        LinearFade();
    }
    private void LinearFade()
    {
        timePassed += Time.deltaTime;
        fadePercent = Mathf.Max(alphaEnd, (duration - timePassed)/duration);
        newAlpha = objectColor.a * fadePercent;
        newColor = new Color (objectColor.r, objectColor.g, objectColor.b, newAlpha);
        textMeshProObject.color = newColor;
    }
}
