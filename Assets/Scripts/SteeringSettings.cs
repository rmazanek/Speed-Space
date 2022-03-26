using System.Collections;
using UnityEngine;

public class SteeringSettings
{
    public float angular;
    public Vector3 linear;
    public SteeringSettings()
    {
        angular = 0f;
        linear = new Vector3();
    }
}
