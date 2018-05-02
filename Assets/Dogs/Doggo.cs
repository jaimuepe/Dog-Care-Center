using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(DogStatus))]
public class Doggo : MonoBehaviour
{
    [Header("Dog parameters")]
    public string dogName;
    [NonSerialized]
    public float currentMovementSpeed;
    public float defaultMovementSpeed;
    public float movementSpeedWhenChasingTreatHungry;
    public float movementSpeedWhenChasingTreatNotHungry;
    public float movementSpeedWhenChasingPlayerHungry;
    public float movementSpeedWhenChasingBall;
    public float movementSpeedWhenReturningBall;

    [NonSerialized]
    public AudioSource audioSource;

    [Header("IA")]
    public float minDistanceGeneral;
    public float minDistanceWhenChasingPlayer;

    [Header("Effects")]
    public ParticleSystem spawnDespawnParticleSystem;
    public AudioClip spawnClip;
    public AudioClip munch;
    public float timeBetweenBarksMin;
    public float timeBetweenBarksMax;

    [NonSerialized]
    public DogStatus status;
    NavMeshAgent agent;

    public Transform mouthSocket;

    Transform playerTransform;
    Transform myTransform;

    DoggoState state;
    enum DoggoState { ROAMING, LOOKING_TO_PLAY, RETURNING_BALL, LOOKING_FOR_FOOD, EATING };

    [NonSerialized]
    public PhysicsBehaviour pb;

    FoodDetector foodDetector;
    BallDetector ballDetector;

    GameObject ball;

    Animator animator;

    public bool IsHoldingBall { get { return ball != null; } }

    [NonSerialized]
    public Character owner;

    private void Start()
    {
        state = DoggoState.ROAMING;
        audioSource = GetComponent<AudioSource>();

        status = GetComponent<DogStatus>();
        agent = GetComponent<NavMeshAgent>();
        pb = GetComponent<PhysicsBehaviour>();
        foodDetector = GetComponentInChildren<FoodDetector>();
        ballDetector = GetComponentInChildren<BallDetector>();
        myTransform = transform;
        playerTransform = FindObjectOfType<Player>().transform;
        animator = GetComponent<Animator>();

        audioSource.Play();
        spawnDespawnParticleSystem.Play();
        AudioSource.PlayClipAtPoint(spawnClip, transform.position);

        StartCoroutine(BarkRandomly());
    }

    IEnumerator BarkRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                UnityEngine.Random.Range(timeBetweenBarksMin, timeBetweenBarksMax));

            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
            // audioSource.Play();
        }
    }

    private void ConfigureAgentChaseFood()
    {
        if (status.IsHungry)
        {
            currentMovementSpeed = movementSpeedWhenChasingTreatHungry;
        }
        else
        {
            currentMovementSpeed = movementSpeedWhenChasingTreatNotHungry;
        }

        if (pb.ControlledByAgent)
        {
            agent.SetDestination(foodDetector.trackedTreat.transform.position);
        }
    }

    private void ConfigureAgentChasePlayer()
    {
        if (pb.ControlledByAgent)
        {
            currentMovementSpeed = movementSpeedWhenChasingPlayerHungry;
            agent.stoppingDistance = minDistanceWhenChasingPlayer;

            agent.SetDestination(playerTransform.position);
        }
    }

    private void ConfigureAgentChaseBall()
    {
        currentMovementSpeed = movementSpeedWhenChasingBall;

        if (pb.ControlledByAgent)
        {
            agent.SetDestination(ballDetector.trackedBall.transform.position);
        }
    }

    private void ConfigureAgentReturnBall()
    {
        currentMovementSpeed = movementSpeedWhenReturningBall;
        agent.stoppingDistance = minDistanceWhenChasingPlayer;

        if (pb.ControlledByAgent)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    private void Update()
    {
        CalculateState();
        agent.stoppingDistance = minDistanceGeneral;
        currentMovementSpeed = defaultMovementSpeed;

        if (foodDetector.IsTrackingTreat)
        {
            ConfigureAgentChaseFood();
        }
        else if (state == DoggoState.LOOKING_FOR_FOOD)
        {
            ConfigureAgentChasePlayer();
        }
        else if (state == DoggoState.LOOKING_TO_PLAY)
        {
            if (pb.ControlledByAgent)
            {
                if (ballDetector.IsTrackingBall)
                {
                    ConfigureAgentChaseBall();
                }
            }
        }
        else if (state == DoggoState.RETURNING_BALL)
        {
            if (pb.ControlledByAgent)
            {
                ConfigureAgentReturnBall();
            }
        }

        agent.speed = currentMovementSpeed;
        animator.SetFloat("speed", currentMovementSpeed);

        if (pb.ControlledByAgent)
        {
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                animator.SetBool("walk", true);
            }
            else
            {
                animator.SetBool("walk", false);
            }
        }
        else if (pb.ControlledByPhysics)
        {
            // animator.SetBool("run", true);
        }
    }


    public void PlayDespawnEffectAndDestroy()
    {
        ParticleSystem despawnEffect = spawnDespawnParticleSystem;
        despawnEffect.transform.parent = null;

        AudioSource.PlayClipAtPoint(spawnClip, transform.position);
        despawnEffect.Play();

        if (IsHoldingBall)
        {
            DropBall();
        }

        Destroy(gameObject);
    }

    void CalculateState()
    {
        state = DoggoState.ROAMING;

        if (status.IsHungry)
        {
            state = DoggoState.LOOKING_FOR_FOOD;
        }
        else if (status.IsBored)
        {
            if (IsHoldingBall)
            {
                state = DoggoState.RETURNING_BALL;
            }
            else
            {
                state = DoggoState.LOOKING_TO_PLAY;
            }
        }

        if (state != DoggoState.RETURNING_BALL)
        {
            DropBall();
        }
    }

    public void EatTreat(GameObject treat)
    {
        status.AddBoost(Attributes.FULLNESS);
        AudioSource.PlayClipAtPoint(munch, myTransform.position);
        StartCoroutine(ResetMunchingState());
        Destroy(treat);
    }

    public void ReturnBall()
    {
        status.AddBoost(Attributes.ENTERTAINMENT);
    }

    public bool IsRoaming()
    {
        return state == DoggoState.ROAMING ||
            (state == DoggoState.LOOKING_TO_PLAY && !ballDetector.IsTrackingBall);
    }

    public bool WantsToPlay()
    {
        return state == DoggoState.LOOKING_TO_PLAY;
    }

    IEnumerator ResetMunchingState()
    {
        munching = true;
        yield return new WaitForSeconds(munch.length);
        munching = false;
    }

    bool munching = false;
    public bool IsMunching()
    {
        return munching;
    }

    public void FetchBall(GameObject ball)
    {
        this.ball = ball;
        ball.GetComponent<Rigidbody>().isKinematic = true;
        ball.transform.position = mouthSocket.position;
        ball.transform.SetParent(mouthSocket, true);
    }

    public void DropBall()
    {
        if (ball)
        {
            ball.GetComponent<Rigidbody>().isKinematic = false;
            ball.transform.SetParent(null);
            ball = null;
        }
    }
}
