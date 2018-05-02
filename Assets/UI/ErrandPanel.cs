using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrandPanel : MonoBehaviour
{
    public float messageDurationInSeconds;
    public Text text;
    public Image image;
    public Image textBg;

    Queue<QueueData> queue = new Queue<QueueData>();

    bool bussy;
    GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();

        text.color = new Color(0f, 10f, 0f, 0f);
        image.color = new Color(1f, 1f, 1f, 0f);
        textBg.color = new Color(1f, 1f, 1f, 0f);
    }

    private void Update()
    {
        if (!bussy && queue.Count > 0)
        {
            QueueData qData = queue.Dequeue();
            gm.ResolveJob(qData.errors, qData.dogs);
            string msg = gm.GetRandomJobMessage(qData.errors);

            SetMessage(msg, qData.dogs[0].owner.sprite, qData.duration, false);
        }
    }

    public void ClearMessages()
    {
        queue.Clear();
        text.color = new Color(0f, 0f, 0f, 0f);
        textBg.color = new Color(1f, 1f, 1f, 0f);
        image.color = new Color(1f, 1f, 1f, 0f);
        bussy = false;
    }

    public void SetMessage(string newMessage, Sprite sprite, float duration, bool clearMessages = true)
    {
        if (clearMessages)
        {
            queue.Clear();
        }

        image.sprite = sprite;
        text.text = newMessage;

        if (HideCoroutine != null)
        {
            StopCoroutine(HideCoroutine);
        }

        text.color = new Color(0f, 0f, 0f, 1f);
        textBg.color = new Color(1f, 1f, 1f, 1f);
        image.color = new Color(1f, 1f, 1f, 1f);

        HideCoroutine = StartCoroutine(HideAutomatically(duration));
    }

    Coroutine HideCoroutine;

    public void Enqueue(int errors, Doggo[] dogs, float duration)
    {
        QueueData qData = new QueueData(duration, errors, dogs);
        queue.Enqueue(qData);
    }

    IEnumerator HideAutomatically(float duration)
    {
        bussy = true;

        yield return new WaitForSeconds(duration);
        text.color = new Color(0f, 0f, 0f, 0f);
        textBg.color = new Color(1f, 1f, 1f, 0f);
        image.color = new Color(1f, 1f, 1f, 0f);
        bussy = false;
    }

    struct QueueData
    {
        public float duration;
        public int errors;
        public Doggo[] dogs;

        public QueueData(float duration, int errors, Doggo[] dogs)
        {
            this.duration = duration;
            this.errors = errors;
            this.dogs = dogs;
        }
    }
}
