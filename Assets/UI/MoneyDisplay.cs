using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplay : MonoBehaviour {

    GameManager gm;
    Text text;

    private void Start()
    {
        text = GetComponent<Text>();
        gm = FindObjectOfType<GameManager>();
    }

    public void UpdateDisplay()
    {
        text.text = gm.Points + "$";
    }
}
