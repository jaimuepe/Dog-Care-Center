using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Telephone : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip pickupClip;
    public AudioClip conversationClip;

    public ErrandPanel errandPanel;

    public int numberOfRingTones;
    public float delayBetweenTonesInSeconds;

    bool bussy = false;
    public bool IsBussy { get { return bussy; } }

    Coroutine ringingCoroutine;

    Interactive interactive;
    DogGenerator dogGenerator;
    GameManager gm;

    private void Start()
    {
        dogGenerator = FindObjectOfType<DogGenerator>();
        gm = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        interactive = GetComponent<Interactive>();
        interactive.enabled = false;
        audioSource.loop = false;
    }

    public void StartRinging()
    {
        if (!bussy)
        {
            bussy = true;
            interactive.enabled = true;
            ringingCoroutine = StartCoroutine(Ring());
        }
    }

    public void PickupPhone()
    {
        StartCoroutine(PickupCoroutine());
    }

    IEnumerator PickupCoroutine()
    {
        audioSource.Stop();
        audioSource.time = 0;

        interactive.enabled = false;

        if (ringingCoroutine != null) { StopCoroutine(ringingCoroutine); }

        int dogCount = gm.GetRandomNumberOfSpawnedDogs();

        AudioSource.PlayClipAtPoint(pickupClip, transform.position);
        AudioSource.PlayClipAtPoint(conversationClip, transform.position);

        string[] dogNames = gm.GetRandomDogNames(dogCount);
        Character c = gm.GetRandomCharacter();

        errandPanel.SetMessage(BuildConversationString(dogNames),
            c.sprite, conversationClip.length);

        yield return new WaitForSeconds(conversationClip.length);
        AudioSource.PlayClipAtPoint(pickupClip, transform.position);

        dogGenerator.SpawnDogs(dogNames, c);
        PhoneCallEnded(true);
    }

    string BuildConversationString(string[] dogNames)
    {
        string template = gm.GetRandomTelephoneMessage();
        if (dogNames.Length == 1)
        {
            return string.Format(template, dogNames[0]);
        }
        else
        {
            string names = dogNames[0];
            for (int i = 1; i < dogNames.Length; i++)
            {
                if (i == dogNames.Length - 1)
                {
                    names += " and " + dogNames[i];
                }
                else
                {
                    names += ", " + dogNames[i];
                }
            }
            return string.Format(template, names);
        }
    }

    void PhoneCallEnded(bool wasPickedUp)
    {
        bussy = false;
        interactive.enabled = false;
        if (!wasPickedUp)
        {
            gm.RemoveHeart();
        }
    }

    IEnumerator Ring()
    {
        for (int i = 0; i < numberOfRingTones; i++)
        {
            audioSource.Play();
            yield return new WaitForSeconds(0.5f);
            audioSource.Play();
            yield return new WaitForSeconds(delayBetweenTonesInSeconds);
        }

        PhoneCallEnded(false);
    }
}
