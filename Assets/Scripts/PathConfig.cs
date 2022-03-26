using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConfig : MonoBehaviour
{
    [SerializeField] int numberOfWaypoints = 5;
    [SerializeField] float xPositionMinViewPort = -0.1f;
    [SerializeField] float xPositionMaxViewPort = 1.1f;
    [SerializeField] float yPositionMinViewPort = 0f;
    [SerializeField] float yPositionMaxViewPort = 1f;
    [SerializeField] float xDistanceBetweenWaypointsMaxViewPort = 1f;
    // [SerializeField] bool evenlySpacedWaypoints = true;
    [SerializeField] [Range(0,1)] float yRandomFactor = 0.5f;
    //[SerializeField] bool startsOnLeft = true;
    GameObject waypoint;
    Vector3 position;
    float xPositionMin;
    float xPositionMax;
    float yPositionMin;
    float yPositionMax;
    public List<Transform> Waypoints;

    // Start is called before the first frame update
    void Start()
    {
        xDistanceBetweenWaypointsMaxViewPort = xPositionMaxViewPort - xPositionMinViewPort;
        SetUpWayPointBoundaries();
        transform.position = new Vector3(xPositionMin, yPositionMin,0);
        GenerateWaypoints();
        GenerateList(gameObject);
    }

    private void SetUpWayPointBoundaries()
    {
        Camera gameCamera = Camera.main;
        xPositionMin = gameCamera.ViewportToWorldPoint(new Vector3(xPositionMinViewPort,0,0)).x;
        xPositionMax = gameCamera.ViewportToWorldPoint(new Vector3(xPositionMaxViewPort,0,0)).x;
        yPositionMin = gameCamera.ViewportToWorldPoint(new Vector3(0,yPositionMinViewPort,0)).y;
        yPositionMax = gameCamera.ViewportToWorldPoint(new Vector3(0,yPositionMaxViewPort,0)).y;
    }
    private void GenerateWaypoints()
    {
        for(int waypointIndex = 0; waypointIndex < numberOfWaypoints; waypointIndex++)
        {
            position = GetNextPosition(waypointIndex);

            waypoint = new GameObject();
            waypoint.transform.position = position;
            waypoint.transform.parent = gameObject.transform;
            waypoint.name = "Waypoint (" + waypointIndex + ")";
        }
    }

    private Vector3 GetNextPosition(int waypointIndex)
    {
        var yRandomPosition = UnityEngine.Random.Range(yPositionMin, yPositionMax);
        float deltaX = (xPositionMax - xPositionMin) / (numberOfWaypoints - 1);
        
        var newXPos = Mathf.Clamp(xPositionMin + deltaX * waypointIndex, xPositionMin, xPositionMax);
        var newYPos = Mathf.Clamp(yRandomPosition * yRandomFactor, yPositionMin, yPositionMax);

        Vector3 position = new Vector3 (newXPos, newYPos, transform.position.z);
        return position;
    }
    private void GenerateList(GameObject instantiatedPath)
    {
        foreach(Transform p in instantiatedPath.transform)
        {
            Waypoints.Add(p);
        }
    }
}
