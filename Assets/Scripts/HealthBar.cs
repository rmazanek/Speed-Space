using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject healthContainer;
    [SerializeField] float healthPerContainer = 200f;
    Image healthBarImage;

    public void UpdateHealthBar(float fillPercent, float maxHealth)
    {   
        RefreshFill(fillPercent);
        RefreshHeartContainers(maxHealth);
    }

    private void RefreshFill(float fillPercent)
    {
        healthBarImage = GetComponent<Image>();
        healthBarImage.fillAmount = fillPercent;
    }
    private void RefreshHeartContainers(float maxHealth)
    {
        int numberOfHealthContainers = (int)Mathf.Max(Mathf.Floor(maxHealth / healthPerContainer), 1);
        int numberOfChildren = gameObject.transform.childCount;
        if(numberOfHealthContainers > numberOfChildren)
        {
            GameObject newHeart = Instantiate(healthContainer, transform.position, Quaternion.identity);
            newHeart.transform.SetParent(transform, false);
        }
        else if(numberOfHealthContainers < numberOfChildren)
        {
            Destroy(gameObject.transform.GetChild(numberOfChildren - 1).gameObject);
        }
    }
}
