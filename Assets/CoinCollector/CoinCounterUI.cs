using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCounterUI : MonoBehaviour
{
    public Text CoinCountText; // Reference to the UI Text element
    private int CoinCount = 0;

    public AchievementManager achievementManager;
    void Start()
    {
        // Initialize the kill count text
        UpdateCoinCountText();
    }

    public void IncrementKillCount()
    {
        CoinCount++;
        UpdateCoinCountText();
        if (achievementManager != null)
        {
            achievementManager.TryMintAchievement(CoinCount);
        }
    }

    private void UpdateCoinCountText()
    {
        if (CoinCountText != null)
        {
            CoinCountText.text = "Collected Coins: " + CoinCount.ToString();
        }
    }
}