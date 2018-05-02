using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    Material material;
    public float outlineWidth = 0.01f;

    private void Awake()
    {
        // material = GetComponent<MeshRenderer>().material;
        Dehighlight();
    }

    private void Start()
    {
        // enabled = false;
    }

    public void Highlight()
    {
        // material.SetFloat("_OutlineExtrusion", outlineWidth);
    }

    public void Dehighlight()
    {
        // material.SetFloat("_OutlineExtrusion", 0f);
    }
}
