using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
  //string tooltipText = "Tooltip text.";
  //Sprite sprite;
  public virtual string GetTooltipText() 
  {
    PopulateTooltipText();
    return null;
  }
  public virtual Sprite GetSprite()
  {
    return null;
  }
  public virtual void PopulateTooltipText() {}
}
