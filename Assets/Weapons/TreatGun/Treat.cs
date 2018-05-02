using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treat : MonoBehaviour
{
    Rigidbody rb;

    public bool RigidbodyIsSleeping { get { return rb.IsSleeping(); } }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}
