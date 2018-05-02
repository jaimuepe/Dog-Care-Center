using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadesUI : MonoBehaviour
{
    public Image grenadePrefabUI;
    public Sprite grenadeSilhouette;
    public Sprite grenadeFull;

    Player player;

    Image[] grenadesUI;

    public void Initialize(float maxGrenades)
    {
        player = FindObjectOfType<Player>();

        grenadesUI = new Image[(int) maxGrenades];

        for (int i = (int) maxGrenades - 1; i >= 0; i--)
        {
            Image grenadeUI = Instantiate(grenadePrefabUI);
            grenadeUI.gameObject.name = "grenade_" + i;
            grenadeUI.transform.SetParent(transform, false);
            grenadesUI[i] = grenadeUI;
        }
    }

    public void UpdateUI()
    {
        float current = player.grenadeSystem.magazine.CurrentAmmo;
        float max = player.grenadeSystem.magazine.maxAmmo;

        for (int i = 0; i < max; i++)
        {
            if (i < current)
            {
                grenadesUI[i].sprite = grenadeFull;
            }
            else
            {
                grenadesUI[i].sprite = grenadeSilhouette;
            }
        }
    }
}
