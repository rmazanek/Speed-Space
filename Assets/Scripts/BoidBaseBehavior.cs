using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidBaseBehavior : MonoBehaviour
{
  public GameObject Target;
  public float MoveSpeed = 4.0f;
  private float temporarySpeedFactor = 1.0f;
  private float minimumTemporarySpeedFactor = 0.15f;
  private float maximumTemporarySpeedFactor = 1.0f;
  [SerializeField] BoidCohesion BoidCohesion;
  [SerializeField] BoidSeparation BoidSeparation;
  //[SerializeField] float closeEnough = 0.0005f;
  protected SteeringSettings steering;
  Vector3 resetVector;
  [SerializeField] Vector3 rotateVector;
  [SerializeField] Vector3 angleVector;
  private Vector3 displacement;
  SquadParent squadParent;
  Enemy enemyComponent;
  private BossDuties isBoss;

  // Start is called before the first frame update
  void Start()
  {
    steering = new SteeringSettings();
    resetVector = new Vector3(0f, 0f, 0f);
    rotateVector = new Vector3(0f, 0f, 1f);
    angleVector = new Vector3(0f, 0f, 1f);
    displacement = new Vector3(0f, 0f, 0f);
    squadParent.IncChildCount();
    enemyComponent = GetComponentInChildren<Enemy>();
    isBoss = GetComponent<BossDuties>();

  }
  public void SetSteering(SteeringSettings _steering, float weight)
  {
    steering.angular += (weight * _steering.angular);
    steering.linear += (weight * _steering.linear);
  }

  // Update is called once per frame
  void Update()
  {
    steering.linear = resetVector;
    displacement = resetVector;
    SetSteering(BoidCohesion.GetSteering(), BoidCohesion.Weight);
    SetSteering(BoidSeparation.GetSteering(), BoidSeparation.Weight);
    if (Target != null)
    {
      displacement += (Target.transform.position - transform.position).normalized * Time.deltaTime * MoveSpeed * temporarySpeedFactor;
    }
    displacement += steering.linear * Time.deltaTime * MoveSpeed * temporarySpeedFactor;
    transform.position += displacement.normalized * Mathf.Clamp(displacement.magnitude, -Time.deltaTime * MoveSpeed * temporarySpeedFactor, Time.deltaTime * MoveSpeed * temporarySpeedFactor);
    CheckIfDestroyed();
  }
  public void SetSquadParent(SquadParent newSquadParent)
  {
    squadParent = newSquadParent;
  }
  private void OnDestroy()
  {
    squadParent.DecChildCount();
  }
  private void CheckIfDestroyed()
  {
    if (enemyComponent == null)
    {
      Destroy(gameObject);
    }
  }
  IEnumerator MultiplySpeedByFactorTemporarilyCoroutine(float factor, float timeInSeconds)
  {
    temporarySpeedFactor = Mathf.Clamp(temporarySpeedFactor * factor, minimumTemporarySpeedFactor, maximumTemporarySpeedFactor);
    yield return new WaitForSeconds(timeInSeconds);
    temporarySpeedFactor = Mathf.Clamp(temporarySpeedFactor / factor, minimumTemporarySpeedFactor, maximumTemporarySpeedFactor);
  }
  public void MultiplySpeedByFactorTemporarily(float factor, float timeInSeconds)
  {
    if (isBoss == null)
    {
      StartCoroutine(MultiplySpeedByFactorTemporarilyCoroutine(factor, timeInSeconds));
    }
  }
}
