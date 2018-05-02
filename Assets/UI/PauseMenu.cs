using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button btnContinue;
    GameManager gm;

    public AudioClip exitClip;
    public AudioClip continueClip;

    bool actionsEnabled = true;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void ShowPauseMenu()
    {
        actionsEnabled = true;
        gameObject.SetActive(true);
        // EventSystem.current.SetSelectedGameObject(btnContinue.gameObject);
    }

    public void HidePauseMenu()
    {
        gameObject.SetActive(false);
    }

    public void Continue()
    {
        if (!actionsEnabled) { return; }

        Time.timeScale = 1f;
        AudioSource.PlayClipAtPoint(continueClip, Camera.main.transform.position);
        Time.timeScale = 0f;

        gm.SwapPauseState();
    }

    public void QuitGame()
    {
        if (!actionsEnabled) { return; }
        actionsEnabled = true;

        Time.timeScale = 1f;
        AudioSource.PlayClipAtPoint(exitClip, Camera.main.transform.position);
        Time.timeScale = 0f;

        StartCoroutine(EndGameCoroutine());
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSecondsRealtime(1);
        Application.Quit();
    }
}
