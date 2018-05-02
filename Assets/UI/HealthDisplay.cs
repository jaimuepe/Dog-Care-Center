using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Sprite fullHeartSprite;
    public Sprite emptyHeartsprite;

    public Image heartPrefab;
    Image[] hearts;

    GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        hearts = new Image[gm.maxHearts];

        for (int i = 0; i < gm.maxHearts; i++)
        {
            Image heart = Instantiate(heartPrefab);
            heart.sprite = fullHeartSprite;
            heart.transform.SetParent(transform, false);
            hearts[i] = heart;
        }
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i > gm.Hearts - 1)
            {
                hearts[i].sprite = emptyHeartsprite;
            }
            else
            {
                hearts[i].sprite = fullHeartSprite;
            }
        }
    }
}
