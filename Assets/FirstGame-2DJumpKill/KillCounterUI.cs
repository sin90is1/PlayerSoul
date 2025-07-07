using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCounterUI : MonoBehaviour
{
    public Text killCountText; // Reference to the UI Text element
    private int killCount = 0;

    void Start()
    {
        // Initialize the kill count text
        UpdateKillCountText();
    }

    public void IncrementKillCount()
    {
        killCount++;
        UpdateKillCountText();
    }

    private void UpdateKillCountText()
    {
        if (killCountText != null)
        {
            killCountText.text = "Kills: " + killCount.ToString();
        }
    }
}