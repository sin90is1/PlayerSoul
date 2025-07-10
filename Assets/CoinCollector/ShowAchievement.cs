using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AchievementDisplay : MonoBehaviour
{
    public string metadataCID = "bafkreihvks5kck3lzzxwymm57edkn3yu7bzr34izjculv33bojczvxzgpy";
    public RawImage achievementImage;
    public TMP_Text achievementName;


    [System.Serializable]
    public class AchievementData
    {
        public string name;
        public string description;
        public string image;
    }

    private void Start()
    {
        StartCoroutine(LoadAchievementFromIPFS());
    }

    IEnumerator LoadAchievementFromIPFS()
    {
        string metadataUrl = "https://ipfs.io/ipfs/" + metadataCID;

        UnityWebRequest request = UnityWebRequest.Get(metadataUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            AchievementData data = JsonUtility.FromJson<AchievementData>(json);
            achievementName.text = data.name;

            // Load image from IPFS
            string imageUrl = data.image.Replace("ipfs://", "https://ipfs.io/ipfs/");
            UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(imageUrl);
            yield return textureRequest.SendWebRequest();

            if (textureRequest.result == UnityWebRequest.Result.Success)
            {
                achievementImage.texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
            }
            else
            {
                Debug.LogError("Failed to load image: " + textureRequest.error);
            }
        }
        else
        {
            Debug.LogError("Failed to load metadata: " + request.error);
        }
    }

}