using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioExample : MonoBehaviour
{
    public float pitchValue = 1.0f;
    public AudioClip mySong;
    private AudioSource audioSource;
    private float low = 0.75f;
    private float high = 1.25f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = mySong;
        audioSource.loop = true;
    }

    void OnGUI()
    {
        pitchValue = GUI.HorizontalSlider(new Rect(25, 75, 100, 30), pitchValue, low, high);
        audioSource.pitch = pitchValue;
    }
}
