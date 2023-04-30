using UnityEngine;

public class FrontWheelRotation : MonoBehaviour
{
    [SerializeField] WheelCollider wheelCollider;

    private void FixedUpdate()
    {
        var rot = transform.localRotation;
        var angles = rot.eulerAngles;
        angles.y = wheelCollider.steerAngle;
        transform.localRotation = Quaternion.Euler(angles);
    }
}
