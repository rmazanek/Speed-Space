using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    float speedOfSpin = 900f;
    [SerializeField] float speedOfSpinMin = 900f;
    [SerializeField] float speedOfSpinMax = 900f;
    [SerializeField][Range(-1,1)] int spinDirection;

    float spinDirectionFactor;
    bool spinOn = true;

    private void Start()
    {
        DetermineSpin();
    }
    // Update is called once per frame
    void Update()
    {
      if(spinOn)
      {  
        transform.Rotate(spinDirectionFactor * Vector3.forward, speedOfSpin * Time.deltaTime, Space.Self);
      }
    }

    void DetermineSpin()
    {
      DetermineSpinDirection();
      DetermineSpinSpeed();
    }

  private void DetermineSpinSpeed()
  {
    speedOfSpin = UnityEngine.Random.Range(speedOfSpinMin, speedOfSpinMax);
  }

  private void DetermineSpinDirection()
  {
    switch (spinDirection)
    {
      case -1:
      {
        spinDirectionFactor = -1f;
        break;
      }
      case 0:
      {
        float rand;
        rand = UnityEngine.Random.Range(0f,1f);
        
        if(rand <= 0.5)
        {
          spinDirectionFactor = -1f;
        }
        else
        {
          spinDirectionFactor = 1f;
        }
        break;
      }
      case 1:
      {
        spinDirectionFactor = 1f;
        break;
      }
      default:
      {
        spinDirectionFactor = 0f;
        break;
      }
    }
  }
}
