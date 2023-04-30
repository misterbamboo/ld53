using System.ComponentModel;
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
    private float tValue;
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
        tValue = Mathf.Clamp(speed / maxSpeed, 0, 1);
        flatDirection = state.GetFlatDirection();

        Refresh();
    }

    private void Refresh()
    {
        var directionOffset = speedOffset + flatDirection * upfrontScale;
        var computeOffset = Vector3.Lerp(offset, directionOffset, tValue);

        var newPos = followTarget.transform.position + computeOffset;
        transform.position = newPos;
        transform.rotation = rotation;
    }
}
