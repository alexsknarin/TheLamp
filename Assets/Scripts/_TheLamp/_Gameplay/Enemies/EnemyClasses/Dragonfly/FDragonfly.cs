using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FDragonfly : CollidableEnemy
{
    [Header("-- Attributes --")]
    [SerializeField] private int _maxHealth = 24;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _collisionRadius = 0.22f;
    [Header("-- Movement --")]
    [SerializeField] private FDragonflyMovement _movement;

    public override void Initialize()
    {
        _movement.Initialize();
    }

    public override void Play()
    {
        _currentHealth = _maxHealth;
        var enterType = (DragonflyEnterType)Random.Range(0, 2);
        int sideDirection = RandomDirection.Generate();
        _movement.Play(enterType, sideDirection);
    }

    public override void ReceiveDamage(int damageAmount)
    {
        throw new NotImplementedException();
    }

    public override void Attack()
    {
        throw new NotImplementedException();
    }

    public override void DoDeath()
    {
        throw new NotImplementedException();
    }

    public override void HandleCollision()
    {
        throw new NotImplementedException();
    }
}
