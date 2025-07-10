using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject swarmerPrefab;

    [SerializeField]
    private float swarmerInterval = 3.5f;

    private List<GameObject> activecoins = new List<GameObject>(); 


    void Start()
    {
        StartCoroutine(spawnEnemy(swarmerInterval, swarmerPrefab));
    }

    private IEnumerator spawnEnemy(float interval, GameObject Coin)
    {
        yield return new WaitForSeconds(interval);


        if (activecoins.Count < 5)
        {

            GameObject newCoin = Instantiate(Coin, new Vector3(Random.Range(-4f, 4f), Random.Range(-1f, 1f), 0), Quaternion.identity);
            activecoins.Add(newCoin); 

            var CoinController = newCoin.GetComponent<CoinController>();
            if (CoinController != null)
            {
                CoinController.OnCoinCollected += HandleCoinCollect;
            }
        }

        StartCoroutine(spawnEnemy(interval, Coin));
    }


    private void HandleCoinCollect(GameObject coin)
    {
        activecoins.Remove(coin); 
    }
}