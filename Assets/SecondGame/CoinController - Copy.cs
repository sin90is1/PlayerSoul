using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class CoinController : MonoBehaviour
{
    public event Action<GameObject> OnCoinCollected; // Event to notify when the enemy dies

    private CircleCollider2D[] _colliders;

    private void Start()
    {
        _colliders = GetComponents<CircleCollider2D>();
    }

    public void DestroyCoin()
    {
        // Notify subscribers that this enemy has died
        OnCoinCollected?.Invoke(gameObject);

        // Destroy the enemy
        Destroy(gameObject);
    }
}