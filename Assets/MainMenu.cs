using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public float waitTimeBeforeFading;
    public float fadeTime;
    public float waitTimeAfterFade;

    public Image fadePanel;

    bool actionsEnabled = true;
    FadeAudioSource fade;
    AudioSource source;

    public AudioClip exitGame;
    public AudioClip startGame;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        fade = GetComponent<FadeAudioSource>();
    }

    public void StartGame()
    {
        if (!actionsEnabled) { return; }
        actionsEnabled = false;

        AudioSource.PlayClipAtPoint(startGame, Camera.main.transform.position);

        StartCoroutine(StartGameCoroutine());
        fade.FadeOut(source, 3f);
    }

    public void QuitGame()
    {
        if (!actionsEnabled) { return; }
        actionsEnabled = true;

        AudioSource.PlayClipAtPoint(exitGame, Camera.main.transform.position);

        StartCoroutine(EndGameCoroutine());
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(waitTimeBeforeFading);

        float alpha = fadePanel.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = new Color(0f, 0f, 0f, Mathf.Lerp(alpha, 1f, t));
            fadePanel.color = newColor;
            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 1f);

        yield return new WaitForSeconds(waitTimeAfterFade);

        SceneManager.LoadScene(1);
    }
}
