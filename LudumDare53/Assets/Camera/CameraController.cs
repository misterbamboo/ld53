using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject followTarget;

    [SerializeField] Vector3 offset;

    [SerializeField] Quaternion rotation;


    private void OnDrawGizmos()
    {
        if (followTarget != null)
        {
            Refresh();
        }
    }

    void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        var newPos = followTarget.transform.position + offset;
        transform.position = newPos;
        transform.rotation = rotation;
    }
}
