using System;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Required for TextMeshPro
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public PlayerSoulMinter soulMinter;
    public string playerAddress; // Set this manually in Unity Inspector
    public string achievementURI; // Set IPFS URI for the achievement

    private bool hasMinted = false;

    void Start()
    {
    }


    public void TryMintAchievement(int count)
    {
        if (!hasMinted && count >= 3)
        {
            hasMinted = true;
           StartCoroutine(MintAchievementCoroutine());

        }
    }

    private IEnumerator MintAchievementCoroutine()
    {
        yield return new WaitUntil(() => soulMinter != null && soulMinter.IsReady());
        yield return soulMinter.MintAchievement(playerAddress, achievementURI);
    }
}
