using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerProtectorMovement : MonoBehaviour
{
    [SerializeField] Animator powerProtector;
    [SerializeField] PowerProtectorMovement powerProtectorBuddy;
    public float RepositionSpeed {get; private set;}
    [SerializeField] string idleAnimation = "Idle";
    [SerializeField] string repositionAnimation = "PowerProtectorReposition";
    // Start is called before the first frame update
    void Start()
    {  
        powerProtector.Play(idleAnimation);
        Reposition();
    }
    private void Reposition()
    {
        StartCoroutine(RepositionCoroutine());
    }
    IEnumerator RepositionCoroutine()
    {
        RepositionSpeed = Random.Range(0f,1f);
        if(powerProtectorBuddy.RepositionSpeed > RepositionSpeed)
        {
            powerProtector.Play(repositionAnimation);
        }
        else
        {
            powerProtector.Play(idleAnimation);
        }
        yield return new WaitForSeconds(5.5f);
        Reposition(); 
    }
}
