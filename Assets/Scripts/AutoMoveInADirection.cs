using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveInADirection : MonoBehaviour
{
  [SerializeField] Vector3 direction;
  [SerializeField] float speed = 2f;
  void Update()
  {
    gameObject.transform.position += direction.normalized * speed * Time.deltaTime;
  }
}
