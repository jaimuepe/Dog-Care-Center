using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstantiateMaterialUI : MonoBehaviour {

    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        Material mat = Instantiate(image.material);
        image.material = mat;
    }
}
