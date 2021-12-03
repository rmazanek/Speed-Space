using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    [SerializeField] AudioClip buttonHoverSound;
    [SerializeField] AudioClip buttonPressSound;
    [SerializeField] float buttonHoverVolume = 0.05f;
    [SerializeField] float buttonPressVolume = 0.10f;

    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
          gameObject.SetActive(false);
          Destroy(gameObject);
        }
        else
        {
          DontDestroyOnLoad(gameObject);
        }
    }
    public void PlayMouseEnterSound()
    {
        AudioSource.PlayClipAtPoint(buttonHoverSound, Camera.main.transform.position, buttonHoverVolume);
    }
    public void PlayMousePressSound()
    {
        AudioSource.PlayClipAtPoint(buttonPressSound, Camera.main.transform.position, buttonPressVolume);
    }
}
