using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    ParticleSystem particle;

    [SerializeField] AudioSource crashAudioSource;

    private IGameState gameState;

    void Start()
    {
        gameState = GameManager.GetGameState();
    }

    void Awake()
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




        // Map the total speed to a volue range
        float volume = Mathf.Lerp(0.01f, 0.05f, Mathf.InverseLerp(0.0f, 80.0f, gameState.GetSpeed()));

        // Update the pitch of the audio source
        crashAudioSource.volume = (volume);



        // TODO : ADD COLISION SOUND
        particle.gameObject.transform.position = other.ClosestPointOnBounds(transform.position);
        particle.Play();
        crashAudioSource.Play();
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
