using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    private IEnumerable<Waypoint> allWaypoints = new List<Waypoint>();

    public void Awake()
    {
        allWaypoints = GameObject.FindObjectsOfType<Waypoint>();
    }

    public List<Waypoint> AskShortestWay(Waypoint start, Waypoint end)
    {
        var route = new List<Waypoint>();

        var maxCount = 100;
        var currentCount = 0;

        Waypoint current = start;

        route.Add(current);

        while (current != end && currentCount < maxCount)
        {
            var availables = current.GetAvailableWaypoints();

            Waypoint closest = null;
            float distance = Mathf.Infinity;
            foreach (Waypoint w in availables)
            {
                float d = Vector3.Distance(end.transform.position, w.transform.position);
                if (d < distance)
                {
                    closest = w;
                    distance = d;
                }
            }


            route.Add(closest);
            current = closest;
            // To remove block inifite
            currentCount++;
        }

        route.Add(current);

        return route;
    }

    public Waypoint GetClosestFromLocation(Transform trans)
    {
        Waypoint closest = null;
        float distance = Mathf.Infinity;
        foreach (Waypoint w in allWaypoints)
        {
            float d = Vector3.Distance(trans.position, w.transform.position);
            if (d < distance)
            {
                closest = w;
                distance = d;
            }
        }

        return closest;
    }
}
