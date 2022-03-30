using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterCollectiblesDisplay : MonoBehaviour
{
  [SerializeField] RawImage[] images;
  [SerializeField] Color uncollectedColor = new Color(255, 255, 255, 0);
  [SerializeField] Color collectedColor = new Color(255, 255, 255, 100);

  private void Start()
  {
    MakeSpritesInvisible();
  }
  private void MakeSpritesInvisible()
  {
    for (int i = 0; i < images.Length; i++)
    {
      images[i].color = uncollectedColor;
    }
  }
  public void ColorInSprite(int index)
  {
    images[index].color = collectedColor;
  }

}
