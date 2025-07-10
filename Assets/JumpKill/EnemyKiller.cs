using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKiller : MonoBehaviour
{
    public KillCounterUI killCounterUI; // Reference to the KillCounterUI script

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemyController>();
            enemy.Kill();

            // Notify the KillCounterUI to increment the kill count
            if (killCounterUI != null)
            {
                killCounterUI.IncrementKillCount();
            }
        }
    }
}