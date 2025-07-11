using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject AchievementsScreen;
    public GameObject WalletScreen;

    private void Start()
    {
        AchievementsScreen.SetActive(false);
        WalletScreen.SetActive(false);
    }
    private void Update()
    {

    }

    public void ToggleAchievementsScreen()
    {
        bool isActive = AchievementsScreen.activeSelf;
        AchievementsScreen.SetActive(!isActive);
    }

    public void ToggleWalletScreen()
    {
        bool isActive = WalletScreen.activeSelf;
        WalletScreen.SetActive(!isActive);
    }
}
