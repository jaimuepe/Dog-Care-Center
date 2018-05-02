using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateLimiter : MonoBehaviour
{
    [Tooltip("Fire rate en segundos")]
    public float fireRate;

    float fireRateCountdown;

    public bool IsReadyToFire { get { return fireRateCountdown <= 0f; } }

    private void Start()
    {
        fireRateCountdown = 0f;
    }

    private void Update()
    {
        if (fireRateCountdown > 0)
        {
            fireRateCountdown -= Time.deltaTime;
        }
    }

    public void Fire()
    {
        fireRateCountdown = fireRate;
    }
}
