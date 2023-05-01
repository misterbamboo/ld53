using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GPS : MonoBehaviour
{
    [SerializeField]
    float refreshTimeInSeconds = 0.5f;

    [SerializeField]
    Waypoint destination = null;

    List<Waypoint> waypointsShortestRoute = new List<Waypoint>();

    GameObject player;
    Waypoints waypointsManager;

    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    PointerIcon pointerIcon;

    [SerializeField]
    Image deliveryTicket;

    [SerializeField]
    List<Sprite> possibleTicket;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        waypointsManager = GameObject.FindObjectOfType<Waypoints>();
        TurnOffGPS();
        StartCoroutine(StartGPSRefresh());
    }

    private IEnumerator StartGPSRefresh()
    {
        yield return new WaitForSecondsRealtime(refreshTimeInSeconds);

        if (destination != null)
        {
            var start = waypointsManager.GetClosestFromLocation(player.transform);

            waypointsShortestRoute = waypointsManager.AskShortestWay(start, destination);

            var positions = waypointsShortestRoute.Select(w => w.transform.position).ToArray();
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }
        else
        {
            TurnOffGPS();
        }
        
        StartCoroutine(StartGPSRefresh());
    }

    public void TravelClosest(Transform trans)
    {
        destination = waypointsManager.GetClosestFromLocation(trans);
    }

    public Transform GetDestination()
    {
        return destination?.transform;
    }

    public void IsOnDelivery(bool value)
    {
        if (!deliveryTicket.enabled)
        {
            deliveryTicket.enabled = true;
        }

        if (value)
        {
            deliveryTicket.sprite = possibleTicket[0];
        }
        else
        {
            deliveryTicket.sprite = possibleTicket[1];
        }
    }

    private void TurnOffGPS()
    {
        lineRenderer.positionCount = 0;
        pointerIcon.transform.position = new Vector3(500, 500, 500);
    }
}
