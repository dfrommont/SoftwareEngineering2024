using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Overlay_Script : MonoBehaviour
{

    public UI_Manager manager;
    public GameObject overlay;
    public TMP_Text timerText;
    public TMP_Text draftSign;
    public TMP_Text attackSign;
    public TMP_Text fortifySign;
    private float start = Time.time;
    private int highlighted = 0;
    private bool timerPaused = false;
    private float pausedTime = -1;

    private void Start()
    {
        InvokeRepeating("Timer", 0f, 1f);
    }

    private void Timer()
    {
        float elapsedTime = Time.time - start;
        timerText.SetText(FormatTime(elapsedTime));
    }

    private string FormatTime(float timeInSeconds)
    {
        float hours = Mathf.Floor(timeInSeconds / 3600);
        float minutes = Mathf.Floor(timeInSeconds / 60) % 60;
        float seconds = timeInSeconds % 60;

        string timeString = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        return timeString;
    }

    public void pauseTimer()
    {
        if (!timerPaused)
        {
            // If the timer is running, pause it and record elapsed time
            CancelInvoke("Timer");
            timerPaused = true;
            pausedTime = Time.time;
        }
        else
        {
            // If the timer is paused, resume it from the elapsed time
            if (pausedTime > 0)
            {
                start = pausedTime;
            }
            InvokeRepeating("Timer", 0f, 1f);
            timerPaused = false;
        }
    }

    public void highlight()
    {
        draftSign.fontSize = 18;
        attackSign.fontSize = 18;
        fortifySign.fontSize = 18;
        highlighted = highlighted + 1 & 3;
        if (highlighted == 1) {
            draftSign.fontSize = 25;
        } else if (highlighted == 2)
        {
            attackSign.fontSize = 25;
        } else if(highlighted == 3)
        {
            fortifySign.fontSize = 25;
        }
    }
}
