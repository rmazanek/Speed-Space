using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    [SerializeField] AudioClip spawnSound;
    [SerializeField] float spawnSoundVolume = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource.PlayClipAtPoint(spawnSound, Camera.main.transform.position, spawnSoundVolume);
    }


}
