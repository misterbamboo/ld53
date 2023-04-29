using UnityEngine;

public class TukTukController : MonoBehaviour
{
    [SerializeField] AxleInfo Axles;
    [SerializeField] float maxMotorTorque = 1000;
    [SerializeField] float maxBreakTorque = 2000;
    [SerializeField] float maxSteeringAngle = 60;
    [SerializeField] GameObject centerOfMass;

    private float horizontal;
    private float vertical;
    private float steering;
    private float motor;
    private float brake;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.transform.localPosition;
    }
    private void FixedUpdate()
    {
        GetPlayerInput();
        ApplyPlayerInputs();
    }

    private void GetPlayerInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = -Input.GetAxis("Vertical");

        steering = maxSteeringAngle * horizontal;
        motor = maxMotorTorque * vertical;
        brake = CalculateBreaks();
    }

    private float CalculateBreaks()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            return maxBreakTorque;
        }
        else if (Input.GetAxis("Vertical") == 0)
        {
            return maxBreakTorque * 0.1f;
        }
        return 0;
    }

    private void ApplyPlayerInputs()
    {
        Axles.frontLeftWheel.steerAngle = steering;
        Axles.frontRightWheel.steerAngle = steering;

        Axles.frontLeftWheel.motorTorque = motor;
        Axles.frontRightWheel.motorTorque = motor;
        Axles.backRightWheel.motorTorque = motor;
        Axles.backLeftWheel.motorTorque = motor;

        Axles.frontLeftWheel.brakeTorque = brake * 0.1f;
        Axles.frontRightWheel.brakeTorque = brake * 0.1f;
        Axles.backRightWheel.brakeTorque = brake;
        Axles.backLeftWheel.brakeTorque = brake;

        // print($"{Axles.frontLeftWheel.rpm} - brake {brake}");
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider frontRightWheel;
    public WheelCollider frontLeftWheel;
    public WheelCollider backLeftWheel;
    public WheelCollider backRightWheel;
}