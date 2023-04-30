using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7)
        {
            return;
        }

        print(other.name);
        particle.gameObject.transform.position = other.ClosestPointOnBounds(transform.position);
        particle.Play();        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 7)
        {
            return;
        }

        particle.gameObject.transform.position = other.transform.position;
        particle.Stop();        
    }
}
