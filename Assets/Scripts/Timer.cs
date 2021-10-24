using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] Text timerText;

    float currentTime;
    bool completed;

    private void Start()
    {
        currentTime = 0;
        completed = false;
    }

    private void FixedUpdate()
    {
        if (!completed)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);

            timerText.text = string.Format("Timer\n{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            currentTime += Time.deltaTime;
        }
    }

    public void StopCount()
    {
        completed = true;
    }
}
