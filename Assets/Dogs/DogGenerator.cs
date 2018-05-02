using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogGenerator : MonoBehaviour
{
    public Doggo doggoPrefab;
    public Doggo doggo2Prefab;

    public BoxCollider spawnRegion;

    public float delayBetweenPhoneCallAndSpawnMin;
    public float delayBetweenPhoneCallAndSpawnMax;

    public Texture[] doggo1bodyTextures;
    public Texture[] doggo2bodyTextures;

    TimerManager timerManager;
    GameManager gm;

    private void Start()
    {
        timerManager = FindObjectOfType<TimerManager>();
        gm = FindObjectOfType<GameManager>();
    }

    public void SpawnDogs(string[] dogNames, Character owner)
    {
        StartCoroutine(SpawnDogCoroutine(dogNames, owner));
    }

    IEnumerator SpawnDogCoroutine(string[] dogNames, Character owner)
    {
        yield return new WaitForSeconds(Random.Range(
            delayBetweenPhoneCallAndSpawnMin, delayBetweenPhoneCallAndSpawnMax));

        Doggo[] dogs = new Doggo[dogNames.Length];

        for (int i = 0; i < dogNames.Length; i++)
        {
            NavMeshHit navHit;
            NavMesh.SamplePosition(
                spawnRegion.transform.position,
                out navHit,
                5f,
                -1);

            float pitch = gm.GetRandomBarkPitch();
            AudioClip barkClip = gm.GetRandomBarkClip();

            Doggo dog;
            Texture[] textures;

            if (Random.Range(0f, 1f) > 0.5f)
            {
                dog = Instantiate(doggoPrefab);
                textures = doggo1bodyTextures;
            }
            else
            {
                dog = Instantiate(doggo2Prefab);
                textures = doggo2bodyTextures;
            }

            SkinnedMeshRenderer meshRenderer = dog.GetComponentInChildren<SkinnedMeshRenderer>();
            meshRenderer.material.mainTexture = textures[Random.Range(0, textures.Length)];

            dog.dogName = dogNames[i];

            AudioSource audioSource = dog.GetComponent<AudioSource>();
            audioSource.clip = barkClip;
            audioSource.pitch = pitch;

            dog.owner = owner;
            dog.transform.localScale *= Random.Range(0.8f, 1.2f);
            dog.transform.position = spawnRegion.transform.position;

            dogs[i] = dog;
        }

        timerManager.RegisterDogs(dogs);
    }
}
