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

        var maxCount = 50;
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
}


/*
float maxDistance = 50.0f;
Dictionary<Transform, Transform> transformAndTarget = new Dictionary<Transform, Transform>();



private void OnDrawGizmos()
{
    DrawSphere();
    DrawLine();
}

private void DrawSphere()
{
    var childTransforms = gameObject.GetComponentsInChildren<Transform>().Where(t => t.GetInstanceID() != gameObject.transform.GetInstanceID());
    foreach (Transform transform in childTransforms)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}

private void DrawLine()
{
    transformAndTarget = new Dictionary<Transform, Transform>();

    Gizmos.color = Color.red;

    var childTransforms = gameObject.GetComponentsInChildren<Transform>().Where(t => t.GetInstanceID() != gameObject.transform.GetInstanceID());
    foreach (var transform in childTransforms)
    {
        float targetDistance = 0;
        Transform target = null;
        foreach (var otherTransform in childTransforms)
        {
            if (transform.position == otherTransform.position)
            {
                continue;
            }

            if (transformAndTarget.ContainsKey(otherTransform) && transformAndTarget[otherTransform].GetInstanceID() == transform.GetInstanceID())
            {
                continue;
            }

            if (transformAndTarget.ContainsKey(otherTransform))
            {
                var targetOfTarget = transformAndTarget[otherTransform];
                if (transformAndTarget.ContainsKey(targetOfTarget) && transformAndTarget[targetOfTarget].position == transform.position)
                {
                    continue;
                }

                if (transformAndTarget.ContainsKey(targetOfTarget))
                {
                    var targetOfTargetOfTarget = transformAndTarget[targetOfTarget];
                    if (transformAndTarget.ContainsKey(targetOfTargetOfTarget) && transformAndTarget[targetOfTargetOfTarget].position == transform.position)
                    {
                        continue;
                    }
                }
            }

            float distance = Vector3.Distance(transform.position, otherTransform.position);                
            if ((distance <= targetDistance || targetDistance == 0) && distance <= maxDistance)
            {
                targetDistance = distance;
                target = otherTransform;
            }
        }

        if (target != null)
        {
            print("draw line");  
            transformAndTarget.Add(transform, target);
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
*/

    /*
    private void DrawLine()
    {
        transformAndTarget = new Dictionary<Transform, Transform>();

        var childTransforms = gameObject.GetComponentsInChildren<Transform>().Where(t => t.GetInstanceID() != gameObject.transform.GetInstanceID());

        Gizmos.color = Color.red;
        Handles.color = Color.red;

        foreach (var trans in childTransforms)
        {
            Transform target = null;
            foreach (var otherTransform in childTransforms)
            {
                if (trans.position == otherTransform.position)
                {
                    continue;
                }

                var notSelectableTransform = GetNotSelectableTransform(trans);
                if (notSelectableTransform.Count > 0 && notSelectableTransform.Any(t => t.position == otherTransform.position))
                {
                    continue;
                }

                if (target == null)
                {
                    target = otherTransform;
                    continue;
                }

                float distance = Vector3.Distance(trans.position, otherTransform.position);

                bool shorterThanPreviousTarget = false;
                if (distance < Vector3.Distance(trans.position, target.position))
                {
                    shorterThanPreviousTarget = true;
                }

                if (shorterThanPreviousTarget)
                {
                    target = otherTransform;
                }
            }

            if (target != null)
            {
                print("draw line");
                transformAndTarget.Add(trans, target);
                Handles.DrawLine(trans.position, target.position);
                var endPosition = target.position + (transform.forward * 2);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(endPosition, 1.0f);
            }
        }
    }
    */

    /*
    private List<Transform> GetNotSelectableTransform(Transform trans)
    {
        var list = new List<Transform>();

        if (transformAndTarget.Count == 0)
        {
            return list;
        }

        var target = GetTargetIfAvailable(trans);
        if (target != null)
        {
            list.Add(target);
        }

        var target1 = GetTargetIfAvailable(target);
        if (target1 != null)
        {
            list.Add(target1);
        }

        var target2 = GetTargetIfAvailable(target1);
        if (target2 != null)
        {
            list.Add(target2);
        }

        var target3 = GetTargetIfAvailable(target2);
        if (target3 != null)
        {
            list.Add(target3);
        }

        var target4 = GetTargetIfAvailable(target3);
        if (target4 != null)
        {
            list.Add(target4);
        }

        var target5 = GetTargetIfAvailable(target4);
        if (target5 != null)
        {
            list.Add(target5);
        }

        return list;
    }

    private Transform GetTargetIfAvailable(Transform trans)
    {
        if (trans != null && transformAndTarget.ContainsKey(trans) && transformAndTarget[trans] != null)
        {
            print("return target");
            return transformAndTarget[trans];
        }

        return null;
    }

    */


    /*
    private bool RecursiveCheckPreviousTargetLoop(Transform currentNode, int iteration)
    {
        if (transformAndTarget.ContainsKey(currentNode))
        {
            var target = transformAndTarget[currentNode];
            if (transformAndTarget.ContainsKey(target))
            {
                var targetOfTarget = transformAndTarget[target];
                if (transformAndTarget.ContainsKey(targetOfTarget))
                {
                    if (transformAndTarget[targetOfTarget].position == transform.position)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (iteration < 5)
                {
                    iteration++;
                    return RecursiveCheckPreviousTargetLoop(targetOfTarget, iteration);
                }
            }
        }
    }
*/

