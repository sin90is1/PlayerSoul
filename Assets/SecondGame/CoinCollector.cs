using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public CoinCounterUI coinCounterUI; // Reference to the KillCounterUI script

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            var enemy = other.gameObject.GetComponent<CoinController>();
            enemy.DestroyCoin();

            // Notify the KillCounterUI to increment the kill count
            if (coinCounterUI != null)
            {
                coinCounterUI.IncrementKillCount();
            }
        }
    }
}