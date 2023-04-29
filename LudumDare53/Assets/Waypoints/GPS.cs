using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPS : MonoBehaviour
{
    [SerializeField]
    float refreshTimeInSeconds = 2.0f;

    [SerializeField]
    Waypoint destination;

    List<Waypoint> waypoints = new List<Waypoint>();

    GameObject player;
    Waypoints waypointsManager;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        waypointsManager = GameObject.FindObjectOfType<Waypoints>();
        StartCoroutine(StartGPSRefresh());
    }

    private IEnumerator StartGPSRefresh()
    {
        var start = waypointsManager.GetClosestFromLocation(player.transform);

        print("refresh");
        yield return new WaitForSecondsRealtime(refreshTimeInSeconds);
        waypoints = waypointsManager.AskShortestWay(start, destination);
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
}
