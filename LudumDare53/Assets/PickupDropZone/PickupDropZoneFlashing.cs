using System;
using UnityEngine;

public class PickupDropZoneFlashing : MonoBehaviour
{
    private const string PlayerTag = "Player";

    private Guid id;

    [SerializeField] GameObject flashingPlane;
    [SerializeField] Transform[] targetSpots;
    [SerializeField] ZoneType zoneType = ZoneType.PickupZone;
    [SerializeField] bool firstWarehouse = false;

    private IGameState gameState;
    private bool currentState;

    private GPS gps;

    [SerializeField]
    AudioSource ringAudioSource;

    void Start()
    {
        gps = FindObjectOfType<GPS>();

        id = Guid.NewGuid();
        gameState = GameManager.GetGameState();

        if (gameObject.activeInHierarchy)
        {
            if (zoneType == ZoneType.DropZone)
            {
                gameState.SubscribeDropZone(id);
            }
            else if (zoneType == ZoneType.PickupZone)
            {
                gameState.SubscribeWarehouse(id);
                if (firstWarehouse) 
                {
                    gameState.SubscribeAsFirstWarehouse(id);
                }
            }
        }

        currentState = flashingPlane.gameObject.activeInHierarchy;

        foreach (Transform t in targetSpots)
        {
            t.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        var state = FlashingPlaneShouldBeActive();
        if (state != currentState)
        {
            currentState = state;
            flashingPlane.SetActive(currentState);

            if (IsActive())
            {
                gps.TravelClosest(transform);
            }
        }
    }

    private bool FlashingPlaneShouldBeActive()
    {
        var isCarEmpty = gameState.IsCarEmpty();
        if (zoneType == ZoneType.PickupZone && isCarEmpty && IsActive())
        {
            return true;
        }
        else if (zoneType == ZoneType.DropZone && !isCarEmpty && IsActive())
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(PlayerTag))
        {
            var isCarEmpty = gameState.IsCarEmpty();
            if (isCarEmpty && zoneType == ZoneType.PickupZone && IsActive())
            {
                var controller = other.GetComponent<TukTukController>();
                ringAudioSource.pitch = 0.8f;
                ringAudioSource.Play();
                controller.PickupFrom(targetSpots[0].position);
            }
            else if (!isCarEmpty && zoneType == ZoneType.DropZone && IsActive())
            {
                var controller = other.GetComponent<TukTukController>();
                ringAudioSource.pitch = 1.0f;
                ringAudioSource.Play();
                controller.DropTo(targetSpots);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsActive())
        {
            return;
        }

        if (other.gameObject.CompareTag(PlayerTag))
        {
            if (zoneType == ZoneType.PickupZone)
            {
                gameState.DefineNextDropZone();
            }
            else if (zoneType == ZoneType.DropZone)
            {
                gameState.DefineNextWarehouse();
            }
        }
    }

    private bool IsActive()
    {
        return id == gameState.CurrentZoneId;
    }
}


public enum ZoneType
{
    PickupZone,
    DropZone
}