using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionTrigger : MonoBehaviour
{
    BoxCollider bc;
    Transform myTransform;

    public LayerMask ignoreLayers;

    Interactive interactive;

    public bool CanInteract { get { return interactive != null; } }

    Player player;
    public Text interactionHint;

    void Start()
    {
        bc = GetComponent<BoxCollider>();
        myTransform = transform;
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        Collider[] colliders = new Collider[3];

        int count = Physics.OverlapBoxNonAlloc(
            myTransform.position + bc.center,
            bc.size / 2,
            colliders,
            myTransform.rotation,
            ignoreLayers);

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (interactive == null || !interactive.enabled
                    || (interactive.gameObject.GetInstanceID() != colliders[i].gameObject.GetInstanceID()))
                {
                    if (interactive)
                    {
                        interactive.Dehighlight();
                    }

                    interactive = null;

                    Interactive n = colliders[i].GetComponent<Interactive>();
                    if (n && n.enabled)
                    {
                        interactive = n;
                        interactive.Highlight();
                        break;
                    }
                }
            }
        }
        else
        {
            if (interactive)
            {
                interactive.Dehighlight();
                interactive = null;
            }
        }

        UpdateUIHint();
    }

    void UpdateUIHint()
    {
        if (interactive == null)
        {
            interactionHint.gameObject.SetActive(false);
        }
        else
        {
            string text;
            if (interactive.gameObject.CompareTag("Ball"))
            {
                if (player.grenadeSystem.magazine.CurrentAmmo == player.grenadeSystem.magazine.maxAmmo)
                {
                    text = "You can't carry more balls! (throw them with 'G' or RMB)";
                }
                else
                {
                    text = "Press 'E' to pick up the ball";
                }
            }
            else if (interactive.gameObject.CompareTag("FoodSack"))
            {
                if (player.CurrentWeaponIsTreatGun())
                {
                    if (player.treatGun.magazine.CurrentAmmo < player.treatGun.magazine.maxAmmo)
                    {
                        text = "Press 'E' to pick up food";
                    }
                    else
                    {
                        text = "Your treatgun is full!";
                    }
                }
                else
                {
                    text = "Wrong weapon!";
                }
            }
            else if (interactive.CompareTag("Door"))
            {
                text = "Press 'E' to answer the door";
            }
            else if (interactive.CompareTag("Phone"))
            {
                text = "Press 'E' to answer the phone";
            }
            else
            {
                return;
            }

            interactionHint.text = text;
            interactionHint.gameObject.SetActive(true);
        }
    }
    public void Interact()
    {
        Telephone telephone = interactive.GetComponent<Telephone>();
        if (telephone)
        {
            telephone.PickupPhone();
        }
        else if (interactive.CompareTag("Ball"))
        {
            if (player.grenadeSystem.magazine.CurrentAmmo == player.grenadeSystem.magazine.maxAmmo)
            {
                return;
            }

            Transform ball = interactive.gameObject.transform;
            if (ball.parent != null)
            {
                Transform parent = ball.transform.parent;
                if (parent)
                {
                    Doggo doggo = parent.GetComponentInParent<Doggo>();
                    doggo.ReturnBall();
                }
            }
            Destroy(ball.gameObject);
            player.AddGrenade();
        }
        else if (interactive.CompareTag("FoodSack"))
        {
            if (player.CurrentWeaponIsTreatGun())
            {
                player.CurrentEquipedWeapon.AddAmmo(int.MaxValue);
                player.UpdateWeaponUI();
            }
        }
        else if (interactive.CompareTag("Door"))
        {
            TimerManager tm = FindObjectOfType<TimerManager>();
            tm.ResolveClientsAtTheDoor();
        }
    }
}
