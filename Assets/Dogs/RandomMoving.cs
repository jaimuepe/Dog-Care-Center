using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class RandomMoving : MonoBehaviour
{
    [System.NonSerialized]
    public NavMeshAgent agent;

    Doggo doggo;

    [Tooltip("Radio en el que se moverá nuestro doggo")]
    public float moveRadius;
    [Tooltip("Tiempo que pasará hasta que vuelva a moverse una vez llegue a su destino")]
    public float timerForMoveAgain;
    [Tooltip("Ignora los dos parámetros anteriores y genera un movimiento con parámetros dentro de lo normal")]
    public bool timerForMoveRandom;

    // define si el doggo está quieto en este momento
    bool isStationary;

    private Transform target;
    private float timer, stopTimer, stoppedTimer;
    private float timeToStop, timeStationary, timeMoving;
    // intervalos durante los cuales el doggo va a estar caminando en cada iteración de caminar
    private const float MIN_TIME_MOVING = 1f;
    private const float MAX_TIME_MOVING = 3f;
    // intervalos entre los cuales el doggo se parará durante un tiempo
    private const float MIN_TIME_TO_STOP = 15f;
    private const float MAX_TIME_TO_STOP = 150f;
    // el tiempo que el doggo estará parado
    private const float MIN_TIME_STATIONARY = 2f;
    private const float MAX_TIME_STATIONARY = 8f;

    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        doggo = GetComponent<Doggo>();

        // el doggo está quieto
        isStationary = false;

        // se mueve en base a los atributos seteados en unity
        if (timerForMoveRandom == false)
        {
            // Debug.Log("Modo de movimiento seleccionado: CONTROLADO");
            timer = timerForMoveAgain;
        }
        else
        // se mueve en base al algoritmo de la class RandomMoving
        {
            // Debug.Log("Modo de movimiento seleccionado: ALEATORIO");
            moveRadius = 10f;
            timeMoving = Random.Range(MIN_TIME_MOVING, MAX_TIME_MOVING);
            timeToStop = Random.Range(MIN_TIME_TO_STOP, MAX_TIME_TO_STOP);
            timeStationary = Random.Range(MIN_TIME_STATIONARY, MAX_TIME_STATIONARY);
            timer = timeMoving;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!doggo.IsRoaming())
        {
            return;
        }

        if (!agent.isActiveAndEnabled)
        {
            return;
        }

        timer += Time.deltaTime;
        stopTimer += Time.deltaTime;

        if (stopTimer >= timeToStop)
        {
            isStationary = true;
        }

        if (isStationary == true)
        {
            // Debug.Log("El doggo se ha parado un rato");
            stoppedTimer += Time.deltaTime;
            if (stoppedTimer >= timeStationary)
            {
                isStationary = false;
                // Debug.Log("El doggo recupera el ánimo para caminar");
                timeToStop = Random.Range(MIN_TIME_TO_STOP, MAX_TIME_TO_STOP);
                timeStationary = Random.Range(MIN_TIME_STATIONARY, MAX_TIME_STATIONARY);
            }
        }

        if (isStationary == false)
        {
            if (timerForMoveRandom == true)
            {
                timeMoving = Random.Range(MIN_TIME_MOVING, MAX_TIME_MOVING);
                if (timer >= timeMoving)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, moveRadius, -1);
                    agent.SetDestination(newPos);
                    timer = 0;
                    // Debug.Log("El doggo se para y cambia de dirección a " + transform.position);
                }
            }
            else
            {
                if (timer >= timerForMoveAgain)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, moveRadius, -1);
                    agent.SetDestination(newPos);
                    timer = 0;
                    // Debug.Log("El doggo se para y cambia de dirección a " + transform.position);
                }
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layer)
    {
        Vector3 randDirection = Random.insideUnitSphere * distance;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, distance, layer);

        return navHit.position;
    }
}