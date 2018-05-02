using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDetector : MonoBehaviour
{
    public GameObject trackedBall;

    public bool IsTrackingBall { get { return trackedBall != null; } }

    public LayerMask layerMask;
    Transform myTransform;

    public float radius;
    Doggo doggo;

    private void Start()
    {
        myTransform = transform;
        doggo = transform.parent.GetComponentInChildren<Doggo>();
    }

    private void Update()
    {
        trackedBall = null;

        if (!doggo.WantsToPlay())
        {
            return;
        }

        Collider[] balls = new Collider[3];
        int count = Physics.OverlapSphereNonAlloc(myTransform.position, radius, balls, layerMask);
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = Vector3.Normalize(balls[i].transform.position - myTransform.position);
                float dot = Vector3.Dot(dir, myTransform.forward);
                // 90º FOV
                if (dot >= 0f)
                {
                    trackedBall = balls[i].gameObject;
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
