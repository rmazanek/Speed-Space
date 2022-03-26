using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterCollectiblesDisplay : MonoBehaviour
{
  [SerializeField] SpriteRenderer[] spriteRenderers;
  [SerializeField] Color uncollectedColor = new Color(255, 255, 255, 0);
  [SerializeField] Color collectedColor = new Color(255, 255, 255, 100);

  private void Start()
  {
    MakeSpritesInvisible();
  }
  private void MakeSpritesInvisible()
  {
    for (int i = 0; i < spriteRenderers.Length; i++)
    {
      spriteRenderers[i].color = uncollectedColor;
    }
  }
  public void ColorInSprite(int index)
  {
    spriteRenderers[index].color = collectedColor;
  }

}
