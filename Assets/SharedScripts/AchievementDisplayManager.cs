using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class AchievementDisplayManager : MonoBehaviour
{
    [Header("Dependencies")]
    public AchievementManager achievementManager; // Drag in Inspector
    public Button loadAchievementsButton;

    [Header("UI")]
    public GameObject achievementPrefab; // Must have RawImage + TMP_Text children
    public Transform container;

    [System.Serializable]
    public class AchievementData
    {
        public string name;
        public string description;
        public string image;
    }

    void Start()
    {
        loadAchievementsButton.onClick.AddListener(() => StartCoroutine(DisplayPlayerAchievements()));
    }

    private IEnumerator DisplayPlayerAchievements()
    {
        List<(string issuer, string uri)> achievements = null;

        yield return StartCoroutine(achievementManager.FetchPlayerAchievementsCoroutineWithCallback((result) =>
        {
            achievements = result;
        }));

        if (achievements == null || achievements.Count == 0)
        {
            Debug.Log("No achievements found.");
            yield break;
        }

        foreach (var (_, uri) in achievements)
        {
            yield return StartCoroutine(InstantiateAchievementFromIPFS(uri));
        }
    }

    private IEnumerator InstantiateAchievementFromIPFS(string ipfsUri)
    {
        string metadataUrl = ipfsUri.Replace("ipfs://", "https://ipfs.io/ipfs/");
        UnityWebRequest request = UnityWebRequest.Get(metadataUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Metadata load failed: " + request.error);
            yield break;
        }

        AchievementData data = JsonUtility.FromJson<AchievementData>(request.downloadHandler.text);
        GameObject item = Instantiate(achievementPrefab, container);

        var text = item.GetComponentInChildren<TMP_Text>();
        var image = item.GetComponentInChildren<RawImage>();

        if (text != null) text.text = data.name;

        string imageUrl = data.image.Replace("ipfs://", "https://ipfs.io/ipfs/");
        UnityWebRequest imgRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imgRequest.SendWebRequest();

        if (imgRequest.result == UnityWebRequest.Result.Success)
        {
            image.texture = ((DownloadHandlerTexture)imgRequest.downloadHandler).texture;
        }
        else
        {
            Debug.LogError("Image load failed: " + imgRequest.error);
        }
    }
}
