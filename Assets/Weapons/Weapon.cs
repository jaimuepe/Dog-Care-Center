using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Magazine))]
public abstract class Weapon : MonoBehaviour
{
    public Transform muzzle;
    public bool isFullyAutomatic;

    public Sprite weaponSprite;

    [NonSerialized]
    public Magazine magazine;
    protected FireRateLimiter limiter;

    private void Awake()
    {
        magazine = GetComponent<Magazine>();
        limiter = GetComponent<FireRateLimiter>();
    }

    public abstract void Fire();

    public bool CanFire()
    {
        if (magazine.CurrentAmmo <= 0)
        {
            return false;
        }
        if (limiter && !limiter.IsReadyToFire)
        {
            return false;
        }
        return true;
    }

    public void AddAmmo(float ammount)
    {
        magazine.AddAmmo(ammount);
    }

    private void OnDrawGizmos()
    {
        if (muzzle)
        {
            Gizmos.DrawWireSphere(muzzle.transform.position, 0.05f);
        }
    }
}
