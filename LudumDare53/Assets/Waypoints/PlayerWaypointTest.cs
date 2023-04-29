using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaypointTest : MonoBehaviour
{
    [SerializeField]
    Waypoint startPoint;

    [SerializeField]
    Waypoint endPoint;

    List<Waypoint> waypoints = new List<Waypoint>(); 

    public void Start()
    {
        waypoints = GameObject.FindObjectOfType<Waypoints>().AskShortestWay(startPoint, endPoint);
    }

    public void OnDrawGizmos()
    {
        foreach (var w in waypoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(w.transform.position, 2.0f);
        }
    }
}
