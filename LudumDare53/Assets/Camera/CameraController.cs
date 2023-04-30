using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject followTarget;

    [SerializeField] Vector3 offset;

    [SerializeField] Vector3 speedOffset;

    [SerializeField] Quaternion rotation;

    [SerializeField] float maxSpeed;
    [SerializeField] private float upfrontScale = 40;

    private IGameState state;
    private float newTValue;
    private float currentTValue;
    private Vector3 flatDirection;

    private void OnDrawGizmos()
    {
        if (followTarget != null)
        {
            Refresh();
        }
    }

    private void Start()
    {
        state = GameManager.GetGameState();
    }

    void Update()
    {
        float speed = state.GetSpeed();
        newTValue = Mathf.Clamp(speed / maxSpeed, 0, 1);
        currentTValue = Mathf.Lerp(currentTValue, newTValue, 0.1f);

        flatDirection = state.GetFlatDirection();

        Refresh();
    }

    private void Refresh()
    {
        var directionOffset = speedOffset + flatDirection * upfrontScale;
        var computeOffset = Vector3.Lerp(offset, directionOffset, currentTValue);

        var newPos = followTarget.transform.position + computeOffset;

        transform.position = Vector3.Lerp(transform.position, newPos, 0.95f);
        transform.rotation = rotation;
    }
}
