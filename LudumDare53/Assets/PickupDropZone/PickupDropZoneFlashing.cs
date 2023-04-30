using System;
using UnityEngine;

public class PickupDropZoneFlashing : MonoBehaviour
{
    private const string PlayerTag = "Player";

    private Guid id;

    [SerializeField] GameObject flashingPlane;
    [SerializeField] Transform[] targetSpots;
    [SerializeField] ZoneType zoneType = ZoneType.PickupZone;

    private IGameState gameState;
    private bool currentState;

    void Start()
    {
        id = Guid.NewGuid();
        gameState = GameManager.GetGameState();

        if (gameObject.activeInHierarchy)
        {
            gameState.SubscribeDropZone(id);
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

            GPS gps = FindObjectOfType<GPS>();
            gps.TravelClosest(transform.position);
        }
    }

    private bool FlashingPlaneShouldBeActive()
    {
        var isCarEmpty = gameState.IsCarEmpty();
        if (zoneType == ZoneType.PickupZone && isCarEmpty)
        {
            return true;
        }
        else if (zoneType == ZoneType.DropZone && !isCarEmpty && id == gameState.DropZoneId)
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
            if (isCarEmpty && zoneType == ZoneType.PickupZone)
            {
                var controller = other.GetComponent<TukTukController>();
                controller.PickupFrom(targetSpots[0].position);
            }
            else if (!isCarEmpty && zoneType == ZoneType.DropZone && id == gameState.DropZoneId)
            {
                var controller = other.GetComponent<TukTukController>();
                controller.DropTo(targetSpots);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(PlayerTag))
        {
            if (zoneType == ZoneType.PickupZone)
            {
                gameState.DefineNextDropZone();
            }
        }
    }
}


public enum ZoneType
{
    PickupZone,
    DropZone
}