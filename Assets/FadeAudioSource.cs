using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAudioSource : MonoBehaviour
{
    public void FadeOut(AudioSource source, float fadeTime)
    {
        StartCoroutine(FadeOutE(source, fadeTime));
    }

    public void FadeIn(AudioSource source, float fadeTime)
    {
        StartCoroutine(FadeInE(source, fadeTime));
    }

    IEnumerator FadeInE(AudioSource source, float fadeTime)
    {
        float volume = source.volume;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            float v = Mathf.Lerp(volume, 1f, t);
            source.volume = v;
            yield return null;
        }
        source.volume = 1f;
    }

    IEnumerator FadeOutE(AudioSource source, float fadeTime)
    {
        float volume = source.volume;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            float v = Mathf.Lerp(volume, 0f, t);
            source.volume = v;
            yield return null;
        }
        source.volume = 0f;
    }
}
