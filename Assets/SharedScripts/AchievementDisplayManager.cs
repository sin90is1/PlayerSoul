using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Thirdweb;
using Thirdweb.Unity;
using UnityEngine.Networking;

public class AchievementDisplayManager : MonoBehaviour
{
    [Header("UI Prefab Setup")]
    public GameObject achievementPrefab; // Prefab with RawImage + TMP_Text
    public Transform achievementContainer;

    [Header("Blockchain Info")]
    public string contractAddress = "YOUR_CONTRACT_ADDRESS";
    public ulong chainId = 11155111; // Sepolia
    public TextAsset contractABI;
    public string playerAddress = "PLAYER_WALLET_ADDRESS"; // Set manually

    private ThirdwebContract contract;

    [System.Serializable]
    public class AchievementData
    {
        public string name;
        public string description;
        public string image;
    }

    public void OnLoadAchievementsClicked()
    {

        StartCoroutine(LoadAndDisplayAchievements());
        
    }

    private IEnumerator LoadAndDisplayAchievements()
    {
        yield return LoadContract();

        var resultTask = ThirdwebContract.Read<List<string>>(
            contract,
            "getPlayerAchievementsTokenURI",
            new object[] { playerAddress }
        );

        yield return new WaitUntil(() => resultTask.IsCompleted);

        var result = resultTask.Result;

        foreach (Transform child in achievementContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (string uri in result)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                Debug.Log($"Parsed URI: {uri}");
                yield return StartCoroutine(InstantiateAchievementFromIPFS(uri));
            }
        }
    }



    private IEnumerator LoadContract()
    {
        if (contract == null)
        {
            var task = ThirdwebContract.Create(
                ThirdwebManager.Instance.Client,
                contractAddress,
                chainId,
                contractABI.text
            );
            yield return new WaitUntil(() => task.IsCompleted);
            contract = task.Result;
        }
    }

    private IEnumerator InstantiateAchievementFromIPFS(string ipfsUri)
    {
        string metadataUrl = ipfsUri.Replace("ipfs://", "https://ipfs.io/ipfs/");
        UnityWebRequest request = UnityWebRequest.Get(metadataUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load metadata from IPFS: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
        AchievementData data = JsonUtility.FromJson<AchievementData>(json);

        GameObject instance = Instantiate(achievementPrefab, achievementContainer);
        RawImage image = instance.GetComponentInChildren<RawImage>();
        TMP_Text text = instance.GetComponentInChildren<TMP_Text>();

        text.text = data.name;

        string imageUrl = data.image.Replace("ipfs://", "https://ipfs.io/ipfs/");
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result == UnityWebRequest.Result.Success)
        {
            image.texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
        }
        else
        {
            Debug.LogError("Failed to load image: " + textureRequest.error);
        }
    }
}
