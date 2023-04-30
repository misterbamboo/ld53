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
    Waypoint destination;

    List<Waypoint> waypoints = new List<Waypoint>();

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

        var start = waypointsManager.GetClosestFromLocation(player.transform);

        waypoints = waypointsManager.AskShortestWay(start, destination);

        var positions = waypoints.Select(w => w.transform.position).ToArray();
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);

        StartCoroutine(StartGPSRefresh());
    }

    public void OnDrawGizmos()
    {
        foreach (var w in waypoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(w.transform.position, 20.0f);
        }
    }

    public void TravelClosest(Vector3 position)
    {
        throw new NotImplementedException();
    }
}
