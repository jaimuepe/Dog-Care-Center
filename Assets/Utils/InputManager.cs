using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Player player;
    InteractionTrigger interactionTrigger;

    [Tooltip("Indica el retardo entre movimientos en la rueda del ratón, en segundos.")]
    public float scrollWheelDelay;

    float scrollWheelCountdown;

    bool ScrollWheelReady { get { return scrollWheelCountdown <= 0; } }

    GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        scrollWheelCountdown = scrollWheelDelay;
        player = FindObjectOfType<Player>();
        interactionTrigger = FindObjectOfType<InteractionTrigger>();
    }

    void Update()
    {
        if (gm.gameOver)
        {
            return;
        }

        if (Input.GetButtonDown("Pause"))
        {
            gm.SwapPauseState();
            return;
        }

        if (gm.gamePaused)
        {
            return;
        }

        if (Input.GetButtonDown("Interact"))
        {
            if (interactionTrigger.CanInteract)
            {
                interactionTrigger.Interact();
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            player.FireEquipedWeapon();
        }

        if (Input.GetButton("Fire1"))
        {
            if (player.CurrentWeaponIsWaterGun())
            {
                if (player.waterGun.IsFiring)
                {
                    player.FireEquipedWeapon();
                }
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            player.ThrowGrenade();
        }

        if (Input.GetButtonDown("Fire3"))
        {
            player.Pet();
        }

        float mouseScroll = Input.GetAxisRaw("Mouse ScrollWheel");

        if (ScrollWheelReady)
        {
            if (mouseScroll != 0f)
            {
                if (mouseScroll > 0f)
                {
                    player.EquipNextWeapon();
                }
                else
                {
                    player.EquipPreviousWeapon();
                }
                scrollWheelCountdown = scrollWheelDelay;
            }
        }
        else
        {
            scrollWheelCountdown -= Time.deltaTime;
        }
    }
}
