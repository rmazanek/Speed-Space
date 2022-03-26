using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tooltipText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image tooltipIcon;
    [SerializeField] Image tooltipPanel;
    [SerializeField] float timeBeforeFade = 1f;
    [SerializeField] float timeSpentFading = 1f;
    CanvasGroup canvasGroup;
    IEnumerator fadeOutProcess;
    [SerializeField] float heightPadding = 2f;
    Vector2 defaultPanelSize;
    private void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        defaultPanelSize = tooltipPanel.rectTransform.sizeDelta;
    }
    public void DisplayChange(string itemName, string text, Sprite sprite)
    {
        DisplayText(itemName, text);
        DisplayIcon(sprite);
        DisplayResize();

        if(fadeOutProcess != null)
        {
            StopCoroutine(fadeOutProcess);
        }
        fadeOutProcess = StartFadeOut();
        StartCoroutine(fadeOutProcess);
    }
    private void DisplayText(string itemName, string text)
    {
        nameText.text = itemName;
        tooltipText.text = text;
        tooltipText.ForceMeshUpdate();
    }
    private void DisplayIcon(Sprite icon)
    {
        tooltipIcon.sprite = icon;
    }
    IEnumerator StartFadeOut()
    {
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(timeBeforeFade);
        float timerInSeconds = timeSpentFading;
        
        while(timerInSeconds > 0)
        {
            canvasGroup.alpha = Mathf.Max(0,timerInSeconds / timeSpentFading);
            timerInSeconds -= Time.deltaTime;
            yield return null;
        }
    }
    private void DisplayResize()
    {
        float newHeight = heightPadding + Mathf.Max(tooltipIcon.rectTransform.sizeDelta.y, tooltipText.textBounds.size.y + 2f + nameText.textBounds.size.y);
        float currentWidth = tooltipPanel.rectTransform.sizeDelta.x;
        tooltipPanel.rectTransform.sizeDelta = new Vector2 (currentWidth, newHeight);
    }
    private void DisplaySizeReset()
    {
        tooltipPanel.rectTransform.sizeDelta = defaultPanelSize;
    }
}
