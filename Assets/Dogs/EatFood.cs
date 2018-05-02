using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFood : MonoBehaviour
{
    Doggo doggo;

    private void Start()
    {
        doggo = transform.parent.GetComponentInChildren<Doggo>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Treats"))
        {
            if (doggo.IsHoldingBall)
            {
                doggo.DropBall();
            }
            doggo.EatTreat(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Ball"))
        {
            if (doggo.WantsToPlay())
            {
                doggo.FetchBall(other.gameObject);
            }
        }
    }
}
