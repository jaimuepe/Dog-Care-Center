using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Weapon[] weapons;
    public GrenadeSystem grenadeSystem;

    int currentWeaponIdx;

    [Header("UI")]
    public Image currentWeaponSprite;
    public Image secondaryWeaponSprite;
    GrenadesUI grenadesUI;

    public Weapon CurrentEquipedWeapon { get { return weapons[currentWeaponIdx]; } }

    public Animator armAnimator;

    [NonSerialized]
    public WaterGun waterGun;
    [NonSerialized]
    public TreatGun treatGun;

    public AudioClip swapGunClip;

    private void Start()
    {
        currentWeaponIdx = 0;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == 0)
            {
                weapons[i].gameObject.SetActive(true);
            }
            else
            {
                weapons[i].gameObject.SetActive(false);
            }
            if (weapons[i].GetType() == typeof(WaterGun))
            {
                waterGun = weapons[i] as WaterGun;
            }
            else
            {
                treatGun = weapons[i] as TreatGun;
            }
        }

        grenadesUI = FindObjectOfType<GrenadesUI>();
        grenadesUI.Initialize(grenadeSystem.magazine.maxAmmo);
        UpdateWeaponUI();
    }

    public void EquipNextWeapon()
    {
        currentWeaponIdx++;
        if (currentWeaponIdx >= weapons.Length) { currentWeaponIdx = 0; }

        AudioSource.PlayClipAtPoint(swapGunClip, transform.position, 0.1f);
        EnableCurrentGunDisableRest();
        UpdateWeaponUI();
    }

    public void EquipPreviousWeapon()
    {
        currentWeaponIdx--;
        if (currentWeaponIdx < 0) { currentWeaponIdx = weapons.Length - 1; }

        AudioSource.PlayClipAtPoint(swapGunClip, transform.position);
        EnableCurrentGunDisableRest();
        UpdateWeaponUI();
    }

    internal bool CurrentWeaponIsAutomatic()
    {
        return weapons[currentWeaponIdx].isFullyAutomatic;
    }

    public bool CurrentWeaponIsWaterGun()
    {
        return weapons[currentWeaponIdx].GetType() == typeof(WaterGun);
    }

    public bool CurrentWeaponIsTreatGun()
    {
        return weapons[currentWeaponIdx].GetType() == typeof(TreatGun);
    }

    internal void ThrowGrenade()
    {
        if (grenadeSystem.magazine.CurrentAmmo <= 0)
        {
            return;
        }
        armAnimator.SetBool("throw_grenade", true);
        StartCoroutine(DisablePropertyNextFrame("throw_grenade"));
    }

    IEnumerator DisablePropertyNextFrame(string propertyName)
    {
        yield return new WaitForEndOfFrame();
        armAnimator.SetBool(propertyName, false);
    }

    public void InstantiateBall()
    {
        grenadeSystem.Fire();
        UpdateWeaponUI();
    }

    public void Pet()
    {
        // armAnimator.SetBool("pet", true);
    }

    public void AddGrenade()
    {
        grenadeSystem.AddAmmo(1);
        UpdateWeaponUI();
    }

    internal void FireEquipedWeapon()
    {
        weapons[currentWeaponIdx].Fire();
        UpdateWeaponUI();
    }

    void EnableCurrentGunDisableRest()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            Weapon weapon = weapons[i];
            if (i == currentWeaponIdx)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateWeaponUI()
    {
        Weapon primary = weapons[currentWeaponIdx];
        Weapon secondary = weapons[(currentWeaponIdx + 1) % weapons.Length];

        float max = primary.magazine.maxAmmo;
        float maxL = max.ToString().Length;
        float current = primary.magazine.CurrentAmmo;

        currentWeaponSprite.sprite = primary.weaponSprite;
        currentWeaponSprite.material.SetFloat("_FillPercentage", primary.magazine.CurrentAmmo / primary.magazine.maxAmmo);

        secondaryWeaponSprite.sprite = secondary.weaponSprite;
        secondaryWeaponSprite.material.SetFloat("_FillPercentage", secondary.magazine.CurrentAmmo / secondary.magazine.maxAmmo);

        grenadesUI.UpdateUI();
    }
}
