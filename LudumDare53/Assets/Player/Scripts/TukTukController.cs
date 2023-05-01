using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TukTukController : MonoBehaviour
{
    [Header("Car")]
    [SerializeField] AxleInfo axles;
    [SerializeField] float maxMotorTorque = 1000;
    [SerializeField] float maxBreakTorque = 2000;
    [SerializeField] float maxSteeringAngle = 60;
    [SerializeField] GameObject centerOfMass;

    [Header("Transported Boxes")]
    [SerializeField] Transform boxesHolder;
    [SerializeField] Transform[] boxes;
    [SerializeField] Transform boxesSpawnPoint;

    [Header("Trails")]
    [SerializeField] TrailRenderer trailRendererLeft;
    [SerializeField] TrailRenderer trailRendererRight;

    [Header("Reset")]
    [SerializeField] float resetSpeedLimit = 5.0f;

    public bool IsEmpty { get; private set; }

    private float horizontal;
    private float vertical;
    private float steering;
    private float motor;
    private float brake;
    private bool reseting;

    private Rigidbody rb;
    private float movingDirection;

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
        UpdateMovingForward();
        GetPlayerInput();
        ApplyPlayerInputs();
        CheckSplips();
        CheckReset();
    }

    private void UpdateMovingForward()
    {
        var forwardSpeed = Vector3.Dot(rb.velocity, rb.transform.forward);
        var movingDirectionSign = forwardSpeed == 0 ? 0 : Mathf.Sign(forwardSpeed);
        movingDirection = movingDirectionSign;
    }

    private void GetPlayerInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = -Input.GetAxis("Vertical");
        steering = EaseSteering();
        motor = maxMotorTorque * vertical;
        brake = CalculateBreaks();
        reseting = Input.GetButtonDown("Fire3");
    }

    private float EaseSteering()
    {
        // Highspeed make small movement more smooth
        var fixedDeltaSpeed = GetSpeed() * Time.fixedDeltaTime;

        var standardSteering = maxSteeringAngle * horizontal;
        var easeSteering = maxSteeringAngle * EaseInOutCubic(horizontal);

        return Mathf.Lerp(standardSteering, easeSteering, fixedDeltaSpeed);
    }

    private float EaseInOutCubic(float x)
    {
        var sign = Mathf.Sin(x);
        x = Mathf.Abs(x);
        return (x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2) * sign;
    }

    private float CalculateBreaks()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            return maxBreakTorque;
        }
        else if (vertical == 0)
        {
            return maxBreakTorque * 0.1f;
        }
        else if (vertical != 0)
        {
            var verticalDirection = vertical == 0 ? 0 : Mathf.Sign(vertical);
            if (movingDirection > 0 && verticalDirection < 0)
            {
                return maxBreakTorque;
            }
            else if (movingDirection < 0 && verticalDirection > 0)
            {
                return maxBreakTorque;
            }
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

        axles.backRightWheel.brakeTorque = brake;
        axles.backLeftWheel.brakeTorque = brake;

        if (brake != 0 && (axles.backLeftWheel.isGrounded || axles.backRightWheel.isGrounded))
        {
            var reverseForce = -rb.velocity * Time.fixedDeltaTime * 0.5f;
            var brakePercentage = brake / maxBreakTorque;
            var reversePercForce = reverseForce * brakePercentage;
            rb.velocity = rb.velocity + reversePercForce;
        }
    }

    private void CheckSplips()
    {
        CheckSlip(axles.backLeftWheel, trailRendererLeft);
        CheckSlip(axles.backRightWheel, trailRendererRight);
    }

    private void CheckSlip(WheelCollider wheelCollider, TrailRenderer trailRenderer)
    {
        trailRenderer.emitting = false;
        if (wheelCollider.GetGroundHit(out WheelHit hit))
        {
            if (hit.sidewaysSlip > 0.15 || hit.forwardSlip > 0.25)
            {
                trailRenderer.emitting = true;
            }
        }
    }

    public void PickupFrom(Vector3 fromPosition)
    {
        StartCoroutine(PickupBoxes(fromPosition));
    }

    public void DropTo(Transform[] destinationItems)
    {
        StartCoroutine(DropBoxes(destinationItems));
    }

    private IEnumerator PickupBoxes(Vector3 fromPosition)
    {
        IsEmpty = false;
        SetCarBoxesInactive();

        return AnimateToPrePlacedDestination(fromPosition, boxes, true, 0);
    }

    private IEnumerator DropBoxes(Transform[] destinationItems)
    {
        IsEmpty = true;
        SetCarBoxesInactive();

        return AnimateToPrePlacedDestination(boxesSpawnPoint.position, destinationItems, false, 60);
    }

    private void SetCarBoxesInactive()
    {
        foreach (var box in boxes)
        {
            box.gameObject.SetActive(false);
        }
    }

    private IEnumerator AnimateToPrePlacedDestination(Vector3 spawnPoint, Transform[] hidedDestinations, bool reactivateDestination, float secsBeforeAutodestroyAnimated)
    {
        for (int i = 0; i < hidedDestinations.Length; i++)
        {
            var hidedDestination = hidedDestinations[i];
            var animatedTransform = Instantiate(hidedDestination, spawnPoint, Quaternion.identity, null);
            animatedTransform.gameObject.SetActive(true);

            StartCoroutine(MoveAnimatedTransformAndReactivateDestination(animatedTransform, hidedDestination, reactivateDestination, secsBeforeAutodestroyAnimated));

            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator MoveAnimatedTransformAndReactivateDestination(Transform animated, Transform destination, bool reactivateDestination, float secsBeforeAutodestroyAnimated)
    {
        var startingPos = animated.position;
        var startingRot = animated.rotation;

        var startTime = Time.realtimeSinceStartup;
        var t = 0f;
        while (t < 1)
        {
            yield return new WaitForEndOfFrame();

            var currentTime = Time.realtimeSinceStartup;
            t = currentTime - startTime;

            var endPos = destination.position;
            var endRot = destination.rotation;

            var pos = Vector3.Lerp(startingPos, endPos, t);
            var rot = Quaternion.Lerp(startingRot, endRot, t);

            pos = AddHeightToPos(pos, endPos.y, t);

            animated.transform.position = pos;
            animated.transform.rotation = rot;
        }

        if (reactivateDestination)
        {
            destination.gameObject.SetActive(true);
        }

        if (secsBeforeAutodestroyAnimated > 0)
        {
            yield return new WaitForSeconds(secsBeforeAutodestroyAnimated);

            // Fade out in floor before to destroy the animationObject
            while (animated.transform.position.y > -10f)
            {
                var pos = animated.transform.position;
                pos.y -= 0.1f;
                animated.transform.position = pos;
                yield return new WaitForEndOfFrame();
            }
        }

        Destroy(animated.gameObject);
    }

    private Vector3 AddHeightToPos(Vector3 pos, float y, float t)
    {
        var maxY = 10;
        var heightFactor = Mathf.Sin(Mathf.PI * t);
        var targetY = maxY * heightFactor;

        var newY = Mathf.Lerp(targetY, y, t);
        return new Vector3(pos.x, newY, pos.z);
    }

    public float GetSpeed()
    {
        return rb.velocity.magnitude;
    }

    public Vector3 GetDirection()
    {
        return rb.velocity.normalized;
    }

    private void CheckReset()
    {
        if (GetSpeed() > resetSpeedLimit || !reseting)
        {
            return;
        }

        Waypoints waypoints = FindObjectOfType<Waypoints>();
        var wp = waypoints.GetClosestFromLocation(gameObject.transform);

        Vector3 wpPos = wp.transform.position;

        gameObject.transform.position = new Vector3(wpPos.x, 5.0f, wpPos.z);
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