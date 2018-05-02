using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCallbackReceiver : MonoBehaviour
{
    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void InstantiateBall()
    {
        player.InstantiateBall();
    }
}
