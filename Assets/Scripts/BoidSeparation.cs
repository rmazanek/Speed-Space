using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSeparation : MonoBehaviour
{
    public float DesiredSeparation;
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

                if ((distanceToFriend > 0 ) && (distanceToFriend < DesiredSeparation))
                {
                    Vector3 diff = transform.position - other.transform.position;
                    diff.Normalize();
                    diff /= distanceToFriend;
                    steer.linear += diff;
                    count++;
                }
            }
        }
        if(count > 0)
        {
            //steer.linear /= (float)count;
        }
        
        return steer;
    }
}
