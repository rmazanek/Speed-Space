using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteBossPhases : MonoBehaviour
{
  [SerializeField] Enemy enemy;
  [SerializeField] SpriteRenderer spriteRenderer;
  [SerializeField] float[] phaseChanges;
  [SerializeField] Color[] phaseChangeColors;
  [SerializeField] float[] minTimeBetweenShotsAtPhase;
  [SerializeField] float[] maxTimeBetweenShotsAtPhase;
  [SerializeField] LaserPointPlayer laserPointer;
  bool[] phaseChanged;
  int phaseChangeIndex;
  private void Start()
  {
    phaseChanged = new bool[phaseChanges.Length];
    phaseChangeIndex = GetPhaseChangeIndex();
  }
  private void Update()
  {
    if (!phaseChanged[phaseChangeIndex] && enemy.GetHealth() / enemy.GetMaxHealth() < phaseChanges[phaseChangeIndex])
    {
      phaseChanged[phaseChangeIndex] = true;
      UpdatePhaseValues(phaseChangeIndex);
      phaseChangeIndex = GetPhaseChangeIndex();
    }
  }

  private void UpdatePhaseValues(int index)
  {
    laserPointer.ChangeMinTimeBetweenShots(minTimeBetweenShotsAtPhase[index]);
    laserPointer.ChangeMaxTimeBetweenShots(minTimeBetweenShotsAtPhase[index]);
    spriteRenderer.color = phaseChangeColors[index];
  }

  private int GetPhaseChangeIndex()
  {
    for (int i = 0; i < phaseChanged.Length; i++)
    {
      if (!phaseChanged[i])
      {
        return i;
      }
    }
    return phaseChanged.Length - 1;
  }
}
