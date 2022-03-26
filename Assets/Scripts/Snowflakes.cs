using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowflakes : MonoBehaviour
{
  ParticleSystem particles;
  public IEnumerator ShowParticlesCoroutine(float timeInSeconds)
  {
    particles = gameObject.GetComponent<ParticleSystem>();
    particles.Play();
    yield return new WaitForSecondsRealtime(timeInSeconds);
    particles.Stop();
  }
  public void ShowParticles(float timeInSeconds)
  {
    StartCoroutine(ShowParticlesCoroutine(timeInSeconds));
  }
}
