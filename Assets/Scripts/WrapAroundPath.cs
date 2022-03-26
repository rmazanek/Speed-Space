using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapAroundPath : MonoBehaviour
{
  float halfHeight;
  float halfWidth;
  Vector3 screenBounds;
  Vector3 screenMinimumBounds;
  [SerializeField] MeshRenderer planetMeshRenderer;
  [SerializeField] float paddingFactor = 1.5f;
  // Start is called before the first frame update
  void Start()
  {
    halfWidth = planetMeshRenderer.bounds.extents.x * paddingFactor;
    halfHeight = planetMeshRenderer.bounds.extents.y * paddingFactor;
    screenBounds = GetScreenBounds();
    screenMinimumBounds = GetMinimumScreenBounds();
  }

  private Vector3 GetMinimumScreenBounds()
  {
    Camera mainCamera = Camera.main;
    Vector3 zeroVector = new Vector3(0, 0, mainCamera.transform.position.z);
    return mainCamera.ScreenToWorldPoint(zeroVector);
  }

  // Update is called once per frame
  void Update()
  {
    ValidatePosition(gameObject.transform.position);
  }
  private Vector3 GetScreenBounds()
  {
    Camera mainCamera = Camera.main;
    Vector3 screenVector = new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z);
    return mainCamera.ScreenToWorldPoint(screenVector);
  }
  private void ValidatePosition(Vector3 currentPosition)
  {
    Vector3 newPosition = currentPosition;

    if (currentPosition.x < screenMinimumBounds.x - halfWidth)
    {
      newPosition.x = screenBounds.x + halfWidth;
    }
    if (currentPosition.x > screenBounds.x + halfWidth)
    {
      newPosition.x = screenMinimumBounds.x - halfWidth;
    }
    if (currentPosition.y < screenMinimumBounds.y - halfHeight)
    {
      newPosition.y = screenBounds.y + halfHeight;
    }
    if (currentPosition.y > screenBounds.y + halfHeight)
    {
      newPosition.y = screenMinimumBounds.y - halfHeight;
    }
    gameObject.transform.position = newPosition;
  }
}
