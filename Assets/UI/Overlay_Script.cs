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
    private float start;
    private int highlighted = 0;
    private bool timerPaused = false;
    private float pausedTime = -1;
    public GameInterface gameInterface;

    private void Start()
    {
        start = Time.time;
        InvokeRepeating("Timer", 0f, 1f);
        gameInterface.DraftCountChanged += updateArmiesCount;
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

    public void updateTurnPhaseIndicator(TurnPhase currentPhase)
    {
        switch (currentPhase) {
            case TurnPhase.Draft:
                attackSign.SetText("Draft");
                break;
            case TurnPhase.Attack:
                attackSign.SetText("Attack");
                fortifySign.SetText("");
                break;
            case TurnPhase.Fortify:
                attackSign.SetText("Fortify");
                fortifySign.SetText("");
                break;
            case TurnPhase.Deploy:
                attackSign.SetText("Deploy");
                fortifySign.SetText("");
                break;
        };
    }

    void updateArmiesCount(int newValue)
    {
        Debug.Log(newValue);
        fortifySign.SetText(newValue.ToString());
    }
}
