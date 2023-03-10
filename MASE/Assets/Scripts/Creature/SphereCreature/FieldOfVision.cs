using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfVision : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    public List<(Transform, float)> visibleTargets = new List<(Transform, float)>();

    private void Start()
    {
        StartCoroutine("FindtargetsWithDelay", .2f);
    }

    IEnumerator FindtargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewRadius, TargetMask);

        for (int i = 0; i < targetsInView.Length; i++)
        {
            if (targetsInView[i] != null)
            {
                Transform target = targetsInView[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float distToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, ObstacleMask))
                    {
                        if (target != this.transform)
                        {
                            visibleTargets.Add((target, distToTarget));
                        }     
                    }
                }
            }
        }
    }

    public Vector3 DirectionFromAngle(float angle, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angle += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
