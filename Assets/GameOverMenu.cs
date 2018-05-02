using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public Text nmbrOfDoggosText;

    public AudioClip clipPlayAgain;
    public AudioClip clipExit;

    FadeOnStart fade;

    private void Start()
    {
        fade = FindObjectOfType<FadeOnStart>();
    }

    public void GameOver()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gameObject.SetActive(true);
        nmbrOfDoggosText.text = string.Format("You took care of {0} doggos today", 
            gm.totalNumberOfDoggos);
    }

    bool actionsEnabled = true;

    public void Quit()
    {
        if (!actionsEnabled) { return; }
        actionsEnabled = false;

        AudioSource.PlayClipAtPoint(clipExit, Camera.main.transform.position);
        StartCoroutine(QuitCoroutine());
    }

    IEnumerator QuitCoroutine()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    public void PlayAgain()
    {
        if (!actionsEnabled) { return; }

        AudioSource.PlayClipAtPoint(clipPlayAgain, Camera.main.transform.position);
        fade.ShowPanel();
        StartCoroutine(PlayAgainCoroutine());
    }

    IEnumerator PlayAgainCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
