using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCohesion : MonoBehaviour
{
    public float DesiredCloseness;
    public List<GameObject> Targets;
    public float Weight;
    public SteeringSettings GetSteering()
    {
        SteeringSettings steer = new SteeringSettings();
        int count = 0;

        foreach (GameObject other in Targets)
        {
            if (other != null)
            {
                float distanceToFriend = (transform.position - other.transform.position).magnitude;

                if ((distanceToFriend > 0 ) && (distanceToFriend < DesiredCloseness))
                {
                    steer.linear += other.transform.position;
                    count++;
                }
            }
        }
        if(count > 0)
        {
            steer.linear /= count;
            steer.linear = steer.linear - transform.position;
        }
        
        return steer;
    }
    
}
