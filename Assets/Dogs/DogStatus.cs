using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attributes { FULLNESS, CLEANLINESS, ENTERTAINMENT };

public class DogStatus : MonoBehaviour
{
    [Header("Fullness")]
    [SerializeField]
    float fullness;
    [SerializeField]
    float currentFullnessMultiplier = 0f;
    public float fullnessMaxValue;
    public float fullnessMultiplierIncrement;
    public float fullnessCriticValue;
    public float fullnessDecayRate;
    public float fullnessOverchargedMultiplier;

    public float Fullness { get { return fullness; } }

    [Header("Cleanliness")]
    [SerializeField]
    float cleanliness;
    [SerializeField]
    float currentCleanlinessMultiplier = 0f;
    public float cleanlinessMaxValue;
    public float cleanlinessMultiplierIncrement;
    public float cleanlinessCriticValue;
    public float cleanlinessDecayRate;
    public float cleanlinessOverchargedMultiplier;

    public float Cleanliness { get { return cleanliness; } }

    [Header("Entertainment")]
    [SerializeField]
    float entertainment;
    [SerializeField]
    float currentEntertainmentMultiplier = 0f;
    public float entertainmentMaxValue;
    public float entertainmentMultiplierIncrement;
    public float entertainmentCriticValue;
    public float entertainmentDecayRate;
    public float entertainmentOverchargedMultiplier;

    public float Entertainment { get { return entertainment; } }

    [Header("Timers")]
    public float fullnessBoostDuration;
    public float cleanlinessBoostDuration;
    public float entertainmentBoostDuration;
    public float pattingMultiplierDuration;

    [Header("Effects")]
    public ParticleSystem psHunger;
    public ParticleSystem psBoredom;
    public ParticleSystem psDirty;
    public ParticleSystem psShiny;

    [Header("Face textures")]
    public float blinkPeriod;
    public float munchPeriod;
    public Texture catching;
    public Texture[] happy;
    public Texture[] munching;
    public Texture[] neutral;
    public Texture[] sad;

    SkinnedMeshRenderer meshRenderer;

    Doggo doggo;
    bool isOvercharged;

    public bool IsHungry { get { return fullness < fullnessCriticValue; } }

    public bool IsDirty { get { return cleanliness < cleanlinessCriticValue; } }

    public bool IsBored { get { return entertainment < entertainmentCriticValue; } }

    private void Start()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        doggo = GetComponent<Doggo>();
        // StartCoroutine(Blink());
    }

    // TODO atributos iniciales del perrete
    public void Setup()
    {

    }

    private void Update()
    {
        float dF = 0f;
        float dC = 0f;
        float dE = 0f;

        // Fullness
        dF -= fullnessDecayRate;
        dF -= currentEntertainmentMultiplier;
        dF += currentFullnessMultiplier
            * (isOvercharged ? fullnessOverchargedMultiplier : 1f);

        // Cleanliness
        dC -= cleanlinessDecayRate;
        dC -= currentFullnessMultiplier;
        dC += currentCleanlinessMultiplier
                * (isOvercharged ? cleanlinessOverchargedMultiplier : 1f);

        // Entertainment
        dE -= entertainmentDecayRate;
        dE -= currentCleanlinessMultiplier;
        dE += currentEntertainmentMultiplier
            * (isOvercharged ? entertainmentOverchargedMultiplier : 1f);

        fullness = Mathf.Clamp(fullness + dF * Time.deltaTime, 0f, fullnessMaxValue);
        cleanliness = Mathf.Clamp(cleanliness + dC * Time.deltaTime, 0f, cleanlinessMaxValue);
        entertainment = Mathf.Clamp(entertainment + dE * Time.deltaTime, 0f, entertainmentMaxValue);

        bool hungry = false;
        bool bored = false;
        bool dirty = false;

        if (fullness < fullnessCriticValue)
        {
            hungry = true;
            if (!psHunger.isPlaying)
            {
                psHunger.Play();
            }
        }
        else
        {
            psHunger.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (cleanliness < cleanlinessCriticValue)
        {
            dirty = true;
            if (!psDirty.isPlaying)
            {
                psDirty.Play();
            }
        }
        else
        {
            psDirty.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (entertainment < entertainmentCriticValue)
        {
            bored = true;
            if (!psBoredom.isPlaying)
            {
                psBoredom.Play();
            }
        }
        else
        {
            psBoredom.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (!hungry && !bored && !dirty)
        {
            if (!psShiny.isPlaying)
            {
                psShiny.Play();
            }
        }
        else
        {
            psShiny.Stop();
        }
        /* 
        Debug.Log(string.Format("F: {0}/{1} (x{2}), C: {3}/{4} (x{5}), E: {6}/{7} (x{8})",
           fullness, fullnessMaxValue, dF,
           cleanliness, cleanlinessMaxValue, dC,
           entertainment, entertainmentMaxValue, dE));
         */

        UpdateFaceTexture(hungry, bored, dirty);
    }

    void UpdateFaceTexture(bool hungry, bool bored, bool dirty)
    {
        if (doggo.IsHoldingBall)
        {
            meshRenderer.materials[1].mainTexture = catching;
        }
        else
        {
            if (doggo.IsMunching())
            {
                Texture t = munching[(int)Mathf.Floor(Time.time / munchPeriod) % munching.Length];
                meshRenderer.materials[1].mainTexture = t;
            }
            else
            {
                int count = (IsHungry ? 1 : 0) + (IsBored ? 1 : 0) + (IsDirty ? 1 : 0);

                float t = Time.time;
                int cycle = (int)Mathf.Floor(t / blinkPeriod);
                int pos = 0;
                if (Mathf.Abs(t / blinkPeriod - cycle) < 0.1f)
                {
                    pos = 1;
                }
                else
                {
                    pos = 0;
                }

                Texture tex;
                if (count == 3)
                {
                    tex = sad[pos];
                }
                else if (count == 0)
                {
                    tex = happy[pos];
                }
                else
                {
                    tex = neutral[pos];
                }

                meshRenderer.materials[1].mainTexture = tex;
            }
        }
    }

    IEnumerator Blink()
    {
        while (true)
        {
            if (!doggo.IsHoldingBall && !doggo.IsMunching())
            {

            }

            yield return null;
        }
    }

    public void AddBoost(Attributes attribute)
    {
        StartCoroutine(Boost(attribute));
    }

    IEnumerator Boost(Attributes attribute)
    {
        float boostTime;
        switch (attribute)
        {
            case Attributes.FULLNESS:
                boostTime = fullnessBoostDuration;
                currentFullnessMultiplier += fullnessMultiplierIncrement;
                break;
            case Attributes.CLEANLINESS:
                boostTime = cleanlinessBoostDuration;
                currentCleanlinessMultiplier += cleanlinessMultiplierIncrement;
                break;
            case Attributes.ENTERTAINMENT:
                boostTime = entertainmentBoostDuration;
                currentEntertainmentMultiplier += entertainmentMultiplierIncrement;
                break;
            default:
                throw new UnityException("Unknown attribute " + attribute.ToString());
        }

        yield return new WaitForSeconds(boostTime);

        switch (attribute)
        {
            case Attributes.FULLNESS:
                currentFullnessMultiplier -= fullnessMultiplierIncrement;
                break;
            case Attributes.CLEANLINESS:
                currentCleanlinessMultiplier -= cleanlinessMultiplierIncrement;
                break;
            default:
                currentEntertainmentMultiplier -= entertainmentMultiplierIncrement;
                break;
        }
    }
}
