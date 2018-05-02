using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSystem : Weapon
{
    public float grenadeThrowStrength;
    public GameObject grenadePrefab;

    public AudioClip throwClip;

    public override void Fire()
    {
        if (!CanFire())
        {
            return;
        }

        AudioSource.PlayClipAtPoint(throwClip, transform.position);
        magazine.Fire();

        Camera c = Camera.main;
        Vector3 cameraForward = c.transform.forward;

        GameObject grenade = Instantiate(grenadePrefab);
        grenade.transform.position = muzzle.position;
        grenade.GetComponent<Rigidbody>().AddForce(cameraForward * grenadeThrowStrength);
    }

}
