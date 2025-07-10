using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject swarmerPrefab;

    [SerializeField]
    private float swarmerInterval = 3.5f;

    private List<GameObject> activeEnemies = new List<GameObject>(); // Track active enemies

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemy(swarmerInterval, swarmerPrefab));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        yield return new WaitForSeconds(interval);

        // Check if there are fewer than 5 enemies alive
        if (activeEnemies.Count < 5)
        {
            // Spawn a new enemy
            GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-1f, 3), Random.Range(-1f, 4f), 0), Quaternion.identity);
            activeEnemies.Add(newEnemy); // Add the new enemy to the list

            // Subscribe to the enemy's death event (if applicable)
            var enemyController = newEnemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.OnEnemyDeath += HandleEnemyDeath;
            }
        }

        // Continue spawning enemies
        StartCoroutine(spawnEnemy(interval, enemy));
    }

    // Handle enemy death
    private void HandleEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy); // Remove the dead enemy from the list
    }
}