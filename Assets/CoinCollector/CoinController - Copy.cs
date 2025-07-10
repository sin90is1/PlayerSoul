using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class CoinController : MonoBehaviour
{
    public event Action<GameObject> OnCoinCollected;

    private CircleCollider2D[] _colliders;

    private void Start()
    {
        _colliders = GetComponents<CircleCollider2D>();
    }

    public void DestroyCoin()
    {
        OnCoinCollected?.Invoke(gameObject);
        Destroy(gameObject);
    }
}