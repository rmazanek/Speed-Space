using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterScreenExit : MonoBehaviour
{
    float halfHeight;
    Vector3 screenBounds;
    // Start is called before the first frame update
    void Start()
    {
        halfHeight = gameObject.GetComponent<SpriteRenderer>().bounds.extents.y;
        screenBounds = GetScreenBounds();
    }

    // Update is called once per frame
    void Update()
    {
        DestroyOnScreenExit();
    }
    private Vector3 GetScreenBounds()
    {
        Camera mainCamera = Camera.main;
        Vector3 screenVector = new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(screenVector);
    }
    private void DestroyOnScreenExit()
    {
        float bottom = transform.position.y + halfHeight;
        if(bottom <= -screenBounds.y)
        {
            Destroy(gameObject);
        }
    }
}
