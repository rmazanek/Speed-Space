using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoDropperSpawn : MonoBehaviour
{
    [SerializeField] Enemy bossObject;
    [SerializeField] GameObject[] ufoDropperPrefabs;
    [SerializeField] float[] phasePercents = {1f, 0.75f, 0.10f};
    [SerializeField] GameObject[] phaseColors;
    [SerializeField] float[] phaseRotationSpeeds = {1f, 3f, 4f};
//    [SerializeField] float[] phaseProjectileSpeeds = {5f, 7f, 10f};
//    [SerializeField] float[] phaseProjectileCooldown = {0.5f, 0.4f, 0.2f};
    [SerializeField] Transform[] ufoDropperUndeployedPositions;
    [SerializeField] Transform[] ufoDropperDeployedPositions;
    [SerializeField] float dropperTransitionTime = 1.0f;
    [SerializeField] AudioClip dropperTransitionSound;
    [SerializeField] float dropperTransitionSoundVolume = 0.075f;
    GameObject currentHalo;
    List<GameObject> droppersCurrentlySpawned = new List<GameObject> {};
    int currentPhase = 0;
    int maxPhase = 0;
    float bossMaxHealth;
    float bossHealthPercent;
    SpinnerAdvanced spinner;
    // Start is called before the first frame update
    void Start()
    {
        maxPhase = phasePercents.Length;
        spinner = GetComponent<SpinnerAdvanced>();
        SpawnDroppers();
    }
    private void Update()
    {
        PhaseChanged();    
    }
    private int GetPhase()
    {
        int phase = 0;
        bossMaxHealth = bossObject.GetMaxHealth();
        bossHealthPercent = bossObject.GetHealth() / bossMaxHealth;

        for (int i = 0; i < maxPhase; i++)
        {
            if(bossHealthPercent < phasePercents[i])
            {
                phase++;
            }    
        }
        return phase;
    }
    private bool PhaseChanged()
    {
        return GetPhase() > currentPhase;
    }
    private void SpawnDroppers()
    {
        StartCoroutine(SpawnDroppersCoroutine());
    }
    IEnumerator SpawnDroppersCoroutine()
    {
        if(currentPhase < maxPhase)
        {
            if(currentHalo != null)
            {
                Destroy(currentHalo);
            }
            currentHalo = Instantiate(phaseColors[currentPhase], transform);
            currentHalo.transform.SetParent(transform);

            if(droppersCurrentlySpawned != null)
            {
                for(int i = droppersCurrentlySpawned.Count - 1; i > -1; i--)
                {
                    if(droppersCurrentlySpawned[i] != null)
                    {
                        droppersCurrentlySpawned[i].transform.position = Vector3.Lerp(droppersCurrentlySpawned[i].transform.position, ufoDropperUndeployedPositions[i].position, dropperTransitionTime);
                        AudioSource.PlayClipAtPoint(dropperTransitionSound, Camera.main.transform.position, dropperTransitionSoundVolume);
                        yield return new WaitForSeconds(dropperTransitionTime);
                        Destroy(droppersCurrentlySpawned[i]);
                        droppersCurrentlySpawned.RemoveAt(i);
                    }
                    else
                    {
                        droppersCurrentlySpawned.RemoveAt(i);
                    }
                }
            }
            for(int j = 0; j < ufoDropperUndeployedPositions.Length; j++)
            {
                droppersCurrentlySpawned.Add(DeployNewUFODropper(currentPhase, j));
                yield return new WaitForSeconds(dropperTransitionTime);
                spinner.SpinSpeed = phaseRotationSpeeds[currentPhase];
            }
            currentPhase++;
        }
        yield return new WaitUntil(() => PhaseChanged());
        SpawnDroppers();
    }
    private GameObject DeployNewUFODropper(int phase, int dropperPos)
    {
        GameObject ufoDropper = Instantiate(ufoDropperPrefabs[phase], ufoDropperUndeployedPositions[dropperPos]);
        ufoDropper.transform.SetParent(ufoDropperUndeployedPositions[dropperPos]);
        ufoDropper.transform.position = Vector3.Lerp(ufoDropperUndeployedPositions[dropperPos].position, ufoDropperDeployedPositions[dropperPos].position, dropperTransitionTime);
        AudioSource.PlayClipAtPoint(dropperTransitionSound, Camera.main.transform.position, dropperTransitionSoundVolume);
        return ufoDropper;
    }

}
