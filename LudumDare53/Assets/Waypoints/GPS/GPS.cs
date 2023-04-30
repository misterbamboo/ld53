using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        waypointsManager = GameObject.FindObjectOfType<Waypoints>();
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
        
        StartCoroutine(StartGPSRefresh());
    }

    public void TravelClosest(Transform trans)
    {
        destination = waypointsManager.GetClosestFromLocation(trans);
    }
}
