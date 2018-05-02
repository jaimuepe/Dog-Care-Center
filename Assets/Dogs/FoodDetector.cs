using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDetector : MonoBehaviour
{
    public GameObject trackedTreat;

    public bool IsTrackingTreat { get { return trackedTreat != null; } }

    public LayerMask layerMask;
    Transform myTransform;

    public float radius;

    private void Start()
    {
        myTransform = transform;
    }

    private void Update()
    {
        trackedTreat = null;

        Collider[] treats = new Collider[3];
        int count = Physics.OverlapSphereNonAlloc(myTransform.position, radius, treats, layerMask);
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = Vector3.Normalize(treats[i].transform.position - myTransform.position);
                float dot = Vector3.Dot(dir, myTransform.forward);
                // 90º FOV
                if (dot >= 0)
                {
                    trackedTreat = treats[i].gameObject;
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
