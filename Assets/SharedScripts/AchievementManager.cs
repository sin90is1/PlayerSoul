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
        StartCoroutine(FetchPlayerAchievementsCoroutine());
    }

         public IEnumerator FetchPlayerAchievementsCoroutine()
         {
             yield return new WaitUntil(() => soulMinter != null && soulMinter.IsReady());
     
             var achievementsTask = soulMinter.GetAchievementsForPlayer(playerAddress);
             yield return new WaitUntil(() => achievementsTask.IsCompleted);
    
             if (achievementsTask.Result != null)
             {
                 foreach (var achievement in achievementsTask.Result)
                 {
                     Debug.Log($"Issuer: {achievement.issuer}, URI: {achievement.uri}");
                 }
             }
         }

    public IEnumerator FetchPlayerAchievementsCoroutineWithCallback(Action<List<(string issuer, string uri)>> callback)
    {
        yield return new WaitUntil(() => soulMinter != null && soulMinter.IsReady());

        var achievementsTask = soulMinter.GetAchievementsForPlayer(playerAddress);
        yield return new WaitUntil(() => achievementsTask.IsCompleted);

        if (achievementsTask.Result != null)
        {
            callback?.Invoke(achievementsTask.Result);
        }
        else
        {
            callback?.Invoke(new List<(string, string)>());
        }
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
