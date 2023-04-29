using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
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
            float minDistance = float.MaxValue;
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


                if (transformAndTarget.ContainsKey(otherTransform) && transformAndTarget[otherTransform])
                {
                    var targetOfTarget = transformAndTarget[otherTransform];
                    if (transformAndTarget.ContainsKey(targetOfTarget) && transformAndTarget[targetOfTarget].position == transform.position)
                    {
                        continue;
                    }
                }

                float distance = Vector3.Distance(transform.position, otherTransform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
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
}
