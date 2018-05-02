using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public AudioClip knockKnock;
    Interactive interactive;

    void Start()
    {
        interactive = GetComponent<Interactive>();
    }

    public void EnableInteractive()
    {
        interactive.enabled = true;
    }

    public void DisableInteractive()
    {
        interactive.enabled = false;
    }

    internal void Ring()
    {
        AudioSource.PlayClipAtPoint(knockKnock, Camera.main.transform.position);
    }
}
