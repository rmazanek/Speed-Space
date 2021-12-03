using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLinesBetween : MonoBehaviour
{
    List<Transform> transforms;
    [SerializeField] Material lineMaterial;
    [SerializeField] float widthMultiplier = 0.2f;
    
    public List<Transform> GetPoints()
    {
        foreach(Transform child in transform)
        {
            transforms.Add(child);
        }
        
        return transforms;
    }

    public void RenderLines()
    {
        List<Transform> transforms = GetPoints();

        for(int index = 0; index < transforms.Count - 1; index++)
        {
            GameObject startObject = transforms[index].gameObject;
            LineRenderer lineRenderer = startObject.AddComponent<LineRenderer>();

            lineRenderer.SetPosition(0, transforms[index].position);
            lineRenderer.SetPosition(1, transforms[index+1].position);
            lineRenderer.material = lineMaterial;
            lineRenderer.widthMultiplier = widthMultiplier;
        }
    }
}
