using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TukTukController : MonoBehaviour
{
    [SerializeField] AxleInfo axles;
    [SerializeField] float maxMotorTorque = 1000;
    [SerializeField] float maxBreakTorque = 2000;
    [SerializeField] float maxSteeringAngle = 60;
    [SerializeField] GameObject centerOfMass;

    [SerializeField] Transform boxesHolder;
    [SerializeField] Transform[] boxes;

    public bool IsEmpty { get; private set; }

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

        for (int i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];
            box.gameObject.SetActive(false);
            IsEmpty = true;
        }
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
        axles.frontLeftWheel.steerAngle = steering;
        axles.frontRightWheel.steerAngle = steering;

        axles.frontLeftWheel.motorTorque = motor;
        axles.frontRightWheel.motorTorque = motor;
        axles.backRightWheel.motorTorque = motor;
        axles.backLeftWheel.motorTorque = motor;

        axles.frontLeftWheel.brakeTorque = brake * 0.1f;
        axles.frontRightWheel.brakeTorque = brake * 0.1f;
        axles.backRightWheel.brakeTorque = brake;
        axles.backLeftWheel.brakeTorque = brake;
    }

    public void PickupFrom(Vector3 fromPosition)
    {
        StartCoroutine(PickupBoxes(fromPosition));
    }

    private IEnumerator PickupBoxes(Vector3 fromPosition)
    {
        IsEmpty = false;
        SetBoxesInactive();

        for (int i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];
            var animatedBox = Instantiate(box, fromPosition, Quaternion.identity, null);
            animatedBox.gameObject.SetActive(true);

            StartCoroutine(MoveAnimatedBoxToRealBox(animatedBox, box));

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SetBoxesInactive()
    {
        foreach (var box in boxes)
        {
            box.gameObject.SetActive(false);
        }
    }

    private IEnumerator MoveAnimatedBoxToRealBox(Transform animated, Transform box)
    {
        var startingPos = animated.position;
        var startingRot = animated.rotation;

        var startTime = Time.realtimeSinceStartup;
        var t = 0f;
        while (t < 1)
        {
            var currentTime = Time.realtimeSinceStartup;
            t = currentTime - startTime;

            var endPos = box.position;
            var endRot = box.rotation;

            var pos = Vector3.Lerp(startingPos, endPos, t);
            var rot = Quaternion.Lerp(startingRot, endRot, t);

            pos = AddHeightToPos(pos, endPos.y, t);

            animated.transform.position = pos;
            animated.transform.rotation = rot;

            yield return new WaitForEndOfFrame();
        }

        Destroy(animated.gameObject);
        box.gameObject.SetActive(true);
    }

    private Vector3 AddHeightToPos(Vector3 pos, float y, float t)
    {
        var maxY = y * 2;
        var heightFactor = Mathf.Sin(Mathf.PI * t);
        var targetY = maxY * heightFactor;
        return new Vector3(pos.x, targetY, pos.z);
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