using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class EnemyController : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath; // Event to notify when the enemy dies

    private BoxCollider2D[] _colliders;

    private void Start()
    {
        _colliders = GetComponents<BoxCollider2D>();
    }

    public void Kill()
    {
        // Notify subscribers that this enemy has died
        OnEnemyDeath?.Invoke(gameObject);

        // Destroy the enemy
        Destroy(gameObject);
    }
}