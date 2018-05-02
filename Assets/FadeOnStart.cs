using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOnStart : MonoBehaviour
{
    Image fadePanel;
    public float fadeTime;

    void Start()
    {
        fadePanel = GetComponent<Image>();
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        StartCoroutine(HidePanelCoroutine());
    }

    public void ShowPanel()
    {
        StartCoroutine(ShowPanelCoroutine());
    }

    IEnumerator HidePanelCoroutine()
    {
        float alpha = fadePanel.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = new Color(0f, 0f, 0f, Mathf.Lerp(alpha, 0f, t));
            fadePanel.color = newColor;
            yield return null;
        }
        fadePanel.color = new Color(0f, 0f, 0f, 0f);
    }

    IEnumerator ShowPanelCoroutine()
    {
        float alpha = fadePanel.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = new Color(0f, 0f, 0f, Mathf.Lerp(alpha, 1f, t));
            fadePanel.color = newColor;
            yield return null;
        }
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
    }
}
