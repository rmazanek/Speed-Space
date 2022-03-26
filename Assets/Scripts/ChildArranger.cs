using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildArranger : MonoBehaviour
{
  [SerializeField] List<Transform> children;
  Vector3 screenBounds;
  private Vector3 centerPosition = new Vector3(0f, 0f, 0f);
  public Vector3 InitialOffset;
  public Vector3 Offset;
  public int ChildrenNumber;
  [SerializeField] Vector3 direction;
  [SerializeField] float spaceBetween = 1f;
  // Start is called before the first frame update
  void Start()
  {
    direction = direction.normalized;
    //centerPosition = gameObject.transform.position;
    Offset = direction * spaceBetween;
    //Vector3 totalChildrenSize = GetTotalChildrenSize(children);
    InitialOffset = Offset * children.Count / 2;
    //ArrangeChildren(children);
    //screenBounds = GetScreenBounds();
    //float numberOfChildren = children.Count;
  }
  //private void ArrangeChildren(List<Transform> transforms)
  //{
  //  for (int i = 0; i < transforms.Count; i++)
  //  {
  //    transforms[i].position = FirstPosition + i * Offset;
  //  }
  //}
  //private Vector3 GetScreenBounds()
  //{
  //  Camera mainCamera = Camera.main;
  //  Vector3 screenVector = new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z);
  //  return mainCamera.ScreenToWorldPoint(screenVector);
  //}
  //
  //private Vector3 GetTotalChildrenSize(List<Transform> transforms)
  //{
  //  Vector3 totalWidth = new Vector3(0f, 0f, gameObject.transform.position.z);
  //  for (int i = 0; i < transforms.Count; i++)
  //  {
  //    totalWidth.x += transforms[i].GetComponent<SpriteRenderer>().bounds.size.x;
  //    totalWidth.y += transforms[i].GetComponent<SpriteRenderer>().bounds.size.y;
  //  }
  //  return totalWidth;
  //}
}
