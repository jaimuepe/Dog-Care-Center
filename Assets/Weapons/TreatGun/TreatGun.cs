using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FireRateLimiter))]
public class TreatGun : Weapon
{
    public GameObject treatPrefab;
    public float projectileSpeed;

    Animator animator;

    AudioSource audioSource;

    public void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    override public void Fire()
    {
        if (!CanFire())
        {
            return;
        }

        float r = UnityEngine.Random.Range(0.7f, 1.3f);
        audioSource.pitch = r;
        audioSource.Play();

        animator.SetTrigger("Fire");

        limiter.Fire();
        magazine.Fire();

        GameObject treat = Instantiate(treatPrefab);
        Quaternion q = muzzle.rotation;
        q *= Quaternion.Euler(90 * Vector3.up);

        treat.transform.rotation = q;
        treat.transform.position = muzzle.position;

        Rigidbody treatRb = treat.GetComponent<Rigidbody>();
        treatRb.velocity = muzzle.forward * projectileSpeed;
    }
}
