using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    float moveSpeed = 0.5f;
    Vector2 targetPosition;
    Vector2 targetPositionClamped;
    float xPositionMin = -0.06f;
    float xPositionMax = 0.06f;
    float yPositionMin = -0.06f;
    float yPositionMax = 0.06f;
    [SerializeField] float minPauseTime = 0.1f;
    [SerializeField] float maxPauseTime = 0.5f;    

    // Start is called before the first frame update
    void Start()
    {
        //SetUpMoveBoundaries();
        StartCoroutine(SetNewPosition());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void SetUpMoveBoundaries()
    {
        float padding = transform.GetComponent<RectTransform>().sizeDelta.x/2f;
        
        xPositionMin = -transform.parent.GetComponent<RectTransform>().sizeDelta.x/2f + padding;
        xPositionMax = -xPositionMin;
        yPositionMin = -transform.parent.GetComponent<RectTransform>().sizeDelta.y/2f + padding;
        yPositionMax = -yPositionMin;
    }

    private void Move()
    {
        var movementThisFrame = moveSpeed * Time.deltaTime;
        targetPositionClamped = new Vector2 (Mathf.Clamp(targetPosition.x, xPositionMin, xPositionMax), Mathf.Clamp(targetPosition.y, yPositionMin, yPositionMax));

        transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPositionClamped, movementThisFrame);
    }
    private Vector2 NewPosition()
    {
        float randX = Random.Range(xPositionMin, xPositionMax);
        float randY = Random.Range(yPositionMin, yPositionMax);

        return new Vector2 (randX, randY);
    }

    IEnumerator SetNewPosition()
    {
        while(true)
        {
            targetPosition = new Vector2();
            targetPosition = NewPosition();
            transform.right = -(targetPosition - (Vector2)transform.localPosition);
            yield return (Vector2)transform.localPosition == targetPosition;
            yield return new WaitForSeconds(Random.Range(minPauseTime, maxPauseTime));
        }
    }
}
