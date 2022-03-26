using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EngineSoundEffects : MonoBehaviour
{
    [SerializeField] AudioClip thrusterSound;
    [SerializeField] float thrusterSoundVolume = 0.1f;
    [SerializeField] float thrusterSoundPeriod = 0.2f;
    [SerializeField] bool thrustStarted = false;
    Coroutine thrusterSoundCoroutine;
    float thrusterInstanceSoundVolume;

    // Start is called before the first frame update
    void Start()
    {
        thrusterInstanceSoundVolume = thrusterSoundVolume;        
    }

    public void ThrusterSound(float totalMoveAxisValue)
    {
        //if(SceneManager.GetActiveScene().name == "Level 1")
        //{
            if (totalMoveAxisValue != 0)
            {
                if(!thrustStarted)
                {
                    thrusterSoundCoroutine = StartCoroutine(ThrusterSoundCoroutine());
                    thrustStarted = true;
                }
            }
            else
            {
                if(thrusterSoundCoroutine != null)
                {
                    StopCoroutine(thrusterSoundCoroutine);
                }
                thrustStarted = false;
            }
        //}
    }

    IEnumerator ThrusterSoundCoroutine()
    {
        while(true)
        {
            AudioSource.PlayClipAtPoint(thrusterSound, Camera.main.transform.position, thrusterSoundVolume);
            yield return new WaitForSeconds(thrusterSoundPeriod);
        }
    }
}
