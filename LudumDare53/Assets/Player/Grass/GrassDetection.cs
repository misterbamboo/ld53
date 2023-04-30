using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDetection : MonoBehaviour
{
    ParticleSystem particle;

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Terrain")
        {
            particle.Stop();
        }
    }
}
