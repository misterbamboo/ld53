using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDetection : MonoBehaviour
{
    ParticleSystem particle;

    [SerializeField] AudioSource grassAudioSource;

    private void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain")
        {
            particle.Play();

            if (!grassAudioSource.isPlaying)
            {
                grassAudioSource.Play();
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Terrain")
        {
            particle.Stop();
            StartCoroutine(StopGrassAudioSource());
        }
    }

    private IEnumerator StopGrassAudioSource()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        grassAudioSource.Stop();
    }
}
