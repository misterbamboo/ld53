using System;
using System.Collections;
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

    public bool IsEmpty { get; private set; }

    private float horizontal;
    private float vertical;
    private float steering;
    private float motor;
    private float brake;

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
    }

    private void UpdateMovingForward()
    {
        var leftRPM = axles.backLeftWheel.rpm;
        var rightRPM = axles.backRightWheel.rpm;

        var totalRPM = leftRPM + rightRPM;
        movingDirection = totalRPM == 0 ? 0 : Mathf.Sign(totalRPM);
        // print($"movingForward: {movingForward} (leftRPM: {leftRPM} | rightRPM: {rightRPM} | totalRPM: {totalRPM})");
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
            print($"break");
            return maxBreakTorque;
        }
        else if (vertical == 0)
        {
            print($"vertical");
            return maxBreakTorque * 0.1f;
        }
        else if (vertical != 0)
        {
            var verticalDirection = vertical == 0 ? 0 : Mathf.Sign(vertical);
            if (movingDirection > 0 && verticalDirection < 0)
            {
                print($"BREAK ... movingDirection: {movingDirection} | vertical sign: {Mathf.Sign(vertical)}");
                return maxBreakTorque;

            }
            else if (movingDirection < 0 && verticalDirection > 0)
            {
                print($"BREAK ... movingDirection: {movingDirection} | vertical sign: {Mathf.Sign(vertical)}");
                return maxBreakTorque;
            }
        }

        print($"default");
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

        //axles.frontLeftWheel.brakeTorque = brake * 0.1f;
        //axles.frontRightWheel.brakeTorque = brake * 0.1f;
        axles.backRightWheel.brakeTorque = brake;
        axles.backLeftWheel.brakeTorque = brake;
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

        yield return new WaitForSeconds(secsBeforeAutodestroyAnimated);

        // Fade out in floor before to destroy the animationObject
        while (animated.transform.position.y > -10f)
        {
            var pos = animated.transform.position;
            pos.y -= 0.1f;
            animated.transform.position = pos;
            yield return new WaitForEndOfFrame();
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
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider frontRightWheel;
    public WheelCollider frontLeftWheel;
    public WheelCollider backLeftWheel;
    public WheelCollider backRightWheel;
}