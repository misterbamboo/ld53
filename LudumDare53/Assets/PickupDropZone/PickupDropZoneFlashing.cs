using UnityEngine;

public class PickupDropZoneFlashing : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] GameObject flashingPlane;
    [SerializeField] Transform targetSpot;

    private IGameState gameState;
    private bool currentState;

    void Start()
    {
        gameState = GameManager.GetGameState();
        currentState = flashingPlane.gameObject.activeInHierarchy;
    }

    void Update()
    {
        var isCarEmpty = gameState.IsCarEmpty();
        if (isCarEmpty != currentState)
        {
            currentState = isCarEmpty;
            print("change to: " + currentState);
            flashingPlane.SetActive(currentState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(PlayerTag))
        {
            if(gameState.IsCarEmpty())
            {
                var controller = other.GetComponent<TukTukController>();
                controller.PickupFrom(targetSpot.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(PlayerTag))
        {
            print("player quit");
        }
    }
}
