using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    public float startAmmo;
    public float maxAmmo;
    float currentAmmo;

    public float CurrentAmmo { get { return currentAmmo; } }

    private void Start()
    {
        currentAmmo = startAmmo;
    }

    public void Fire()
    {
        currentAmmo = Mathf.Clamp(currentAmmo - 1, 0.0f, maxAmmo);
    }

    public void AddAmmo(float ammount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + ammount, 0.0f, maxAmmo);
    }
}
