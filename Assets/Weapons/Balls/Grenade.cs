using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    Interactive interactive;

    AudioSource audioSource;

    bool collisionEnabled = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interactive = GetComponent<Interactive>();
        audioSource.playOnAwake = false;
        interactive.enabled = false;
        StartCoroutine(EnableInteractive());
    }


    IEnumerator EnableInteractive()
    {
        yield return new WaitForSeconds(0.5f);
        collisionEnabled = true;
        interactive.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collisionEnabled && collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        audioSource.Play();
    }
}
