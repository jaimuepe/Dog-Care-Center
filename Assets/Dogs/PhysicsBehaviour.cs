
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PhysicsBehaviour : MonoBehaviour
{
    NavMeshAgent agent;
    [NonSerialized]
    public Rigidbody rb;
    Transform myTransform;

    enum ControllerType { AGENT, RIGID_BODY, TRANSFORM };
    ControllerType controllerType;

    public bool ControlledByAgent { get { return controllerType == ControllerType.AGENT; } }

    public bool ControlledByPhysics { get { return controllerType == ControllerType.RIGID_BODY; } }

    bool readyToEnableAgent = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        myTransform = transform;

        controllerType = ControllerType.AGENT;
    }

    private void Update()
    {
        if (controllerType == ControllerType.AGENT && !agent.isActiveAndEnabled)
        {
            rb.isKinematic = false;
            controllerType = ControllerType.RIGID_BODY;
            // Debug.Log("Switching to RB");
        }
        else if (controllerType == ControllerType.RIGID_BODY && rb.IsSleeping())
        {
            rb.isKinematic = true;
            controllerType = ControllerType.TRANSFORM;
            // Debug.Log("Switching to TRANSFORM");
            StartCoroutine(RotateFaceDown());
        }
        else if (controllerType == ControllerType.TRANSFORM)
        {
            if (readyToEnableAgent)
            {
                controllerType = ControllerType.AGENT;
                rb.isKinematic = true;
                agent.enabled = true;
                // Debug.Log("Switching to AGENT");
            }
        }
    }

    public void DisableAgentAndSetupForPhysics()
    {
        agent.enabled = false;
    }

    IEnumerator RotateFaceDown()
    {
        readyToEnableAgent = false;
        Vector3 rotationEuler = myTransform.rotation.eulerAngles;

        float elapsedTime = 0.0f;
        float totalTime = .5f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            float targetX;
            float targetZ;

            if (rotationEuler.x >= 180)
            {
                targetX = 360f;
            }
            else
            {
                targetX = 0f;
            }

            if (rotationEuler.z >= 180)
            {
                targetZ = 360f;
            }
            else
            {
                targetZ = 0f;
            }

            float x = Mathf.Lerp(rotationEuler.x, targetX, elapsedTime / totalTime);
            float z = Mathf.Lerp(rotationEuler.z, targetZ, elapsedTime / totalTime);

            myTransform.rotation = Quaternion.Euler(x, rotationEuler.y, z);

            yield return null;
        }

        myTransform.rotation = Quaternion.Euler(0.0f, rotationEuler.y, 0.0f);

        readyToEnableAgent = true;
    }

}
