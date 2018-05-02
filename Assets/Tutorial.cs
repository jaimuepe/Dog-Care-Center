using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Text text;
    public Image panel;

    public string[] messages;

    private void Start()
    {
        panel.color = new Color(1f, 1f, 1f, 0f);
        text.color = new Color(0f, 0f, 0f, 0f);
        StartCoroutine(ShowMessages());
    }

    IEnumerator ShowMessages()
    {
        yield return new WaitForSeconds(1f);

        int i = 0;
        while (i < messages.Length)
        {
            StartCoroutine(FadeOutFadeIn(messages[i++]));
            yield return new WaitForSeconds(7f);
        }

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOutFadeIn(string newText)
    {
        float fadeTime = 0.25f;

        float alpha = text.color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = new Color(0f, 0f, 0f, Mathf.Lerp(alpha, 0f, t));
            text.color = newColor;

            newColor = new Color(1f, 1f, 1f, Mathf.Lerp(alpha, 0f, t));
            panel.color = newColor;

            yield return null;
        }

        text.color = new Color(0f, 0f, 0f, 0f);
        panel.color = new Color(1f, 1f, 1f, 0f);

        text.text = newText;

        alpha = text.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = new Color(0f, 0f, 0f, Mathf.Lerp(alpha, 1f, t));
            text.color = newColor;

            newColor = new Color(1f, 1f, 1f, Mathf.Lerp(alpha, 1f, t));
            panel.color = newColor;

            yield return null;
        }

        text.color = new Color(0f, 0f, 0f, 1f);
        panel.color = new Color(1f, 1f, 1f, 1f);
    }

    IEnumerator FadeOut()
    {
        float fadeTime = 0.25f;

        float alpha = text.color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = new Color(0f, 0f, 0f, Mathf.Lerp(alpha, 0f, t));
            text.color = newColor;

            newColor = new Color(1f, 1f, 1f, Mathf.Lerp(alpha, 0f, t));
            panel.color = newColor;

            yield return null;
        }

        text.color = new Color(0f, 0f, 0f, 0f);
        panel.color = new Color(1f, 1f, 1f, 0f);
    }
}
