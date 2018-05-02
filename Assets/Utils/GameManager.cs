using System.IO;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour
{
    public Character[] characters;
    public string[] phoneCallMessages;
    public string dogNamesFilename;

    public string[] badJobMessages;
    public string[] averageJobMessages;
    public string[] awesomeJobMessages;

    public int maxErrorsPerDoggo;

    [Header("SFX")]
    public AudioClip awesomeJobClip;
    public AudioClip averageJobClip;
    public AudioClip badJobClip;
    public AudioClip[] barkClips;
    public float barkPitchMin;
    public float barkPitchMax;

    [Header("Points")]
    public int pointsPerAwesomeJob;
    public int pointsPerAverageJob;
    int points;
    public int Points { get { return points; } }

    [Header("Hearts")]
    public int maxHearts;
    HealthDisplay healthDisplay;
    MoneyDisplay moneyDisplay;

    int hearts;
    public int Hearts { get { return hearts; } }

    string[] dogNames;

    public bool gamePaused = false;

    public PauseMenu pauseMenu;
    public GameOverMenu gameOverMenu;

    public int totalNumberOfDoggos;

    public bool gameOver = false;

    TimerManager tm;

    private void Start()
    {
        healthDisplay = FindObjectOfType<HealthDisplay>();
        moneyDisplay = FindObjectOfType<MoneyDisplay>();
        tm = FindObjectOfType<TimerManager>();

        hearts = maxHearts;
        LoadDogNames();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        pauseMenu.gameObject.SetActive(false);
        gameOverMenu.gameObject.SetActive(false);
    }

    void LoadDogNames()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, dogNamesFilename + ".csv");
        dogNames = File.ReadAllLines(filePath);
    }

    public string GetRandomTelephoneMessage()
    {
        return phoneCallMessages[Random.Range(0, phoneCallMessages.Length)];
    }

    public Character GetRandomCharacter()
    {
        return characters[Random.Range(0, characters.Length)];
    }

    public string GetRandomDogName()
    {
        return dogNames[Random.Range(0, dogNames.Length)];
    }

    public string[] GetRandomDogNames(int count)
    {
        string[] names = new string[count];
        for (int i = 0; i < count; i++)
        {
            names[i] = dogNames[Random.Range(0, dogNames.Length)];
        }
        return names;
    }

    public string GetRandomBadJobMessage()
    {
        return badJobMessages[Random.Range(0, badJobMessages.Length)];
    }

    public string GetRandomJobMessage(int errors)
    {
        if (errors > maxErrorsPerDoggo)
        {
            return GetRandomBadJobMessage();
        }
        else if (errors > 0)
        {
            return GetRandomAverageJobMessage();
        }
        else
        {
            return GetRandomAwesomeJobMessage();
        }
    }

    public string GetRandomAverageJobMessage()
    {
        return averageJobMessages[Random.Range(0, averageJobMessages.Length)];
    }

    public string GetRandomAwesomeJobMessage()
    {
        return awesomeJobMessages[Random.Range(0, awesomeJobMessages.Length)];
    }

    public void ResolveJob(int errors, Doggo[] dogs)
    {
        if (errors > maxErrorsPerDoggo)
        {
            RemoveHeart();
        }
        else if (errors > 0)
        {
            AudioClip clip = averageJobClip;
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
            AddPoints(pointsPerAverageJob);
        }
        else
        {
            AudioClip clip = awesomeJobClip;
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
            AddPoints(pointsPerAwesomeJob);
        }

        for (int i = 0; i < dogs.Length; i++)
        {
            dogs[i].PlayDespawnEffectAndDestroy();
        }
    }

    public AudioClip GetRandomBarkClip()
    {
        return barkClips[Random.Range(0, barkClips.Length)];
    }

    public float GetRandomBarkPitch()
    {
        return Random.Range(barkPitchMin, barkPitchMax);
    }

    public void RemoveHeart()
    {
        hearts--;
        healthDisplay.UpdateDisplay();
        AudioSource.PlayClipAtPoint(badJobClip, Camera.main.transform.position);

        if (hearts <= 0)
        {
            gameOver = true;
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Player p = FindObjectOfType<Player>();
            p.GetComponent<RigidbodyFirstPersonController>().enabled = false;
            gameOverMenu.GameOver();
        }
    }

    void AddPoints(int addPoints)
    {
        points += addPoints;
        moneyDisplay.UpdateDisplay();
    }

    public void SwapPauseState()
    {
        gamePaused = !gamePaused;
        if (gamePaused)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.ShowPauseMenu();
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            pauseMenu.HidePauseMenu();
        }
    }

    public int GetRandomNumberOfSpawnedDogs()
    {
        float r = Random.Range(0f, tm.maxSpawnedDogsAtOnce);
        return Mathf.Max(1, (int)Mathf.Floor(r));
    }
}
