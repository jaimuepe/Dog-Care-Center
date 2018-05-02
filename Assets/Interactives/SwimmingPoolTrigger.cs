using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingPoolTrigger : MonoBehaviour
{
    AudioSource audioSource;

    Player player;

    float audioDuration;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        audioSource = GetComponent<AudioSource>();
        audioDuration = audioSource.clip.length;

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("SwimmingPool"))
        {
            if (player.CurrentWeaponIsWaterGun())
            {
                float currentCharge = player.CurrentEquipedWeapon.magazine.CurrentAmmo;
                float maxCharge = player.CurrentEquipedWeapon.magazine.maxAmmo;

                if (currentCharge == maxCharge)
                {
                    audioSource.time = 0f;
                    audioSource.Stop();
                    return;
                }

                if (!audioSource.isPlaying)
                {
                    float perc = currentCharge / maxCharge;
                    audioSource.Play();
                    audioSource.time = audioDuration * perc;
                }

                if (Input.GetButton("Fire1"))
                {
                    audioSource.time = 0f;
                    audioSource.Stop();
                    return;
                }
                else if (Input.GetButtonUp("Fire1"))
                {
                    float perc = currentCharge / maxCharge;
                    audioSource.Stop();
                    audioSource.time = audioDuration * perc;
                }

                float rechargeRate = maxCharge / audioDuration;

                player.CurrentEquipedWeapon.AddAmmo(rechargeRate * Time.deltaTime);
                player.UpdateWeaponUI();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SwimmingPool"))
        {
            if (player.CurrentWeaponIsWaterGun())
            {
                audioSource.Stop();
            }
        }
    }
}
