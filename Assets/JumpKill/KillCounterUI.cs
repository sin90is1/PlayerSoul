using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCounterUI : MonoBehaviour
{
    public Text killCountText; // Reference to the UI Text element
    private int killCount = 0;

    public AchievementManager achievementManager;
    void Start()
    {
        // Initialize the kill count text
        UpdateKillCountText();
    }

    public void IncrementKillCount()
    {
        killCount++;
        //Debug.Log($"kill Count 2: {killCount}");
        UpdateKillCountText();
        if (achievementManager != null)
        {
            achievementManager.TryMintAchievement(killCount);
        }
    }

    private void UpdateKillCountText()
    {
        if (killCountText != null)
        {
            killCountText.text = "Kills: " + killCount.ToString();
            //Debug.Log($"kill Count: {killCount}");

        }
    }
}