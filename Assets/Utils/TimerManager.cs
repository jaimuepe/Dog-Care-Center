using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("Telephone")]
    public float firstPhoneCallDelay;
    public float secondPhoneCallDelay;
    public float delayBetweenPhoneCallsMin;
    public float delayBetweenPhoneCallsMax;
    public float delayBetweenPhoneCallsAbsMin;
    Telephone telephone;

    [Header("Door")]
    public float msgTimeAtDoor;
    public float fastMsgTimeAtDoor;
    public float timeBetweenRings;
    public float maxAdmittedWaitTime;
    Door door;

    [Header("Doggos")]
    public float maxSpawnedDogsAtOnce = 1f;
    public float firstDoggoStayTime;
    public float stayTimeMin;
    public float stayTimeMax;

    [Header("SFX")]
    public AudioClip awesomeJobClip;
    public AudioClip averageJobClip;
    public AudioClip badJobClip;

    List<DoggoStayData> doggosData = new List<DoggoStayData>();
    List<ClientAtTheDoorData> clientsData = new List<ClientAtTheDoorData>();

    GameManager gm;
    ErrandPanel messagesPanel;

    private void Start()
    {
        door = FindObjectOfType<Door>();
        telephone = FindObjectOfType<Telephone>();
        StartCoroutine(PhoneCallCoroutine());
        gm = FindObjectOfType<GameManager>();
        messagesPanel = FindObjectOfType<ErrandPanel>();
    }

    bool firstDoggo = true;

    public void RegisterDogs(Doggo[] dogs)
    {
        float time;
        if (firstDoggo)
        {
            firstDoggo = false;
            time = firstDoggoStayTime;
        }
        else
        {
            time = UnityEngine.Random.Range(stayTimeMin, stayTimeMax);
        }

        DoggoStayData dsd = new DoggoStayData(dogs, time);
        doggosData.Add(dsd);
        gm.totalNumberOfDoggos += dogs.Length;
    }

    void Update()
    {
        if (firstDoggo) { return; }

        for (int i = doggosData.Count - 1; i >= 0; i--)
        {
            DoggoStayData dsd = doggosData[i];
            dsd.remainingTime -= Time.deltaTime;
            if (dsd.remainingTime <= 0)
            {
                doggosData.RemoveAt(i);
                ClientAtTheDoorData catdt = new ClientAtTheDoorData(dsd.dogs);
                Coroutine doorBellCoroutine = StartCoroutine(DoorbellCoroutine());
                catdt.doorbellCoroutine = doorBellCoroutine;
                clientsData.Add(catdt);
            }
        }

        for (int i = 0; i < clientsData.Count; i++)
        {
            ClientAtTheDoorData catdt = clientsData[i];
            catdt.totalWaitTime += Time.deltaTime;
        }

        if (clientsData.Count > 0)
        {
            door.EnableInteractive();
        }
        else
        {
            door.DisableInteractive();
        }

        maxSpawnedDogsAtOnce += Time.deltaTime / 120;

        delayBetweenPhoneCallsMin -= Time.deltaTime / 5;
        delayBetweenPhoneCallsMax -= Time.deltaTime / 5;

        delayBetweenPhoneCallsMin = Mathf.Max(delayBetweenPhoneCallsMin,
            delayBetweenPhoneCallsAbsMin);

        delayBetweenPhoneCallsMax = Mathf.Max(delayBetweenPhoneCallsMax,
            delayBetweenPhoneCallsAbsMin);
    }

    IEnumerator DoorbellCoroutine()
    {
        while (true)
        {
            door.Ring();
            yield return new WaitForSeconds(timeBetweenRings);
        }
    }

    IEnumerator PhoneCallCoroutine()
    {
        yield return new WaitForSeconds(firstPhoneCallDelay);
        telephone.StartRinging();

        yield return new WaitForSeconds(secondPhoneCallDelay);

        while (true)
        {
            telephone.StartRinging();

            while (telephone.IsBussy)
            {
                yield return null;
            }

            yield return new WaitForSeconds(
                 UnityEngine.Random.Range(delayBetweenPhoneCallsMin, delayBetweenPhoneCallsMax));
        }
    }

    IEnumerator SpawnDogAtDoor()
    {
        yield return new WaitForSeconds(1);
    }

    class DoggoStayData
    {
        public float stayTime;
        public Doggo[] dogs;

        public float remainingTime;

        public DoggoStayData(Doggo[] dogs, float stayTime)
        {
            this.dogs = dogs;
            this.stayTime = stayTime;
            remainingTime = stayTime;
        }
    }

    public void ResolveClientsAtTheDoor()
    {
        messagesPanel.ClearMessages();

        for (int i = 0; i < clientsData.Count; i++)
        {
            ClientAtTheDoorData catdd = clientsData[i];
            StopCoroutine(catdd.doorbellCoroutine);

            int errors = 0;
            if (catdd.totalWaitTime > maxAdmittedWaitTime)
            {
                errors++;
            }

            for (int j = 0; j < clientsData[i].dogs.Length; j++)
            {
                Doggo dog = clientsData[i].dogs[j];

                if (dog.status.Fullness < dog.status.fullnessCriticValue)
                {
                    errors++;
                }
                if (dog.status.Entertainment < dog.status.entertainmentCriticValue)
                {
                    errors++;
                }
                if (dog.status.Cleanliness < dog.status.cleanlinessCriticValue)
                {
                    errors++;
                }
            }

            float duration = clientsData.Count > 1 ? fastMsgTimeAtDoor : msgTimeAtDoor;
            messagesPanel.Enqueue(errors, clientsData[i].dogs, duration);
        }

        clientsData.Clear();
    }

    class ClientAtTheDoorData
    {
        public float totalWaitTime;
        public Doggo[] dogs;
        public Coroutine doorbellCoroutine;

        public ClientAtTheDoorData(Doggo[] dogs)
        {
            this.dogs = dogs;
            totalWaitTime = 0f;
        }
    }
}
