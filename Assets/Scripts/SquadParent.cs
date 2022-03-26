using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadParent : MonoBehaviour
{
  [SerializeField] GameObject target;
  [SerializeField] int numberOfChildren = 12;
  [SerializeField] GameObject boidShellPrefab;
  [SerializeField] GameObject enemyPrefab;
  [SerializeField] float moveSpeed = 5f;
  [SerializeField] float childMoveSpeed = 3f;
  [SerializeField] float boidCohesionWeight = 1f;
  [SerializeField] float boidSeparationWeight = 1f;
  [SerializeField] float desiredCloseness = 10f;
  [SerializeField] float desiredSeparation = 5f;
  [SerializeField] bool destroyOnChildless = true;
  [SerializeField] bool respawnSquadOnChildless = false;
  public List<GameObject> children;
  int childNumber;
  private void Start()
  {
    InitializeChildren();
  }
  private void InitializeChildren()
  {
    children = new List<GameObject>();

    for (int i = 0; i < numberOfChildren; i++)
    {
      Vector3 relativeSpawn = new Vector3(i % 4, i / 4, 0f);
      GameObject newChild = Instantiate(boidShellPrefab, transform.position + (relativeSpawn), transform.rotation);
      BoidBaseBehavior boidbase = newChild.GetComponent<BoidBaseBehavior>();
      //newChild.transform.SetParent(gameObject.transform);
      boidbase.Target = gameObject;
      boidbase.MoveSpeed = childMoveSpeed;
      boidbase.SetSquadParent(this);
      newChild.GetComponent<BoidCohesion>().Weight = boidCohesionWeight;
      newChild.GetComponent<BoidCohesion>().DesiredCloseness = desiredCloseness;
      newChild.GetComponent<BoidSeparation>().Weight = boidSeparationWeight;
      newChild.GetComponent<BoidSeparation>().DesiredSeparation = desiredSeparation;
      GameObject newEnemyChild = Instantiate(enemyPrefab, newChild.transform);
      EnemyPathing enemyPathing = newEnemyChild.GetComponent<EnemyPathing>();
      if (enemyPathing != null)
      {
        Destroy(enemyPathing);
      }
      newEnemyChild.transform.SetParent(newChild.transform);

      children.Add(newChild);
    }
    foreach (GameObject child in children)
    {
      child.GetComponent<BoidCohesion>().Targets = children;
      child.GetComponent<BoidSeparation>().Targets = children;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (target != null)
    {
      transform.position += (target.transform.position - transform.position).normalized * Time.deltaTime * moveSpeed;
    }
    if (childNumber <= 0)
    {
      if (destroyOnChildless)
      {
        Destroy(gameObject);
      }
      if (respawnSquadOnChildless)
      {
        InitializeChildren();
      }
    }
  }
  public void IncChildCount()
  {
    childNumber++;
  }
  public void DecChildCount()
  {
    childNumber--;
  }
}
