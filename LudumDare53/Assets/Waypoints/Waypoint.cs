using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField]
    List<Waypoint> AvailableWaypoints = new List<Waypoint>();

    [SerializeField]
    MeshRenderer meshRenderer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var w in AvailableWaypoints)
        {        
            Gizmos.DrawLine(transform.position, w.transform.position);
        }
    }

    public void Start()
    {
        meshRenderer.enabled = false;

        var otherWaypoints = GameObject.FindObjectsOfType<Waypoint>();

        foreach (var waypoint in otherWaypoints)
        {
            if (waypoint.GetAvailableWaypoints().Contains(this))
            {
                if (!AvailableWaypoints.Contains(waypoint))
                {
                    AvailableWaypoints.Add(waypoint);
                }
            }
        }
    }

    public List<Waypoint> GetAvailableWaypoints()
    {
        return AvailableWaypoints;
    }
}
