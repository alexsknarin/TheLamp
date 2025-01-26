using System;
using UnityEngine;

[RequireComponent(typeof(FMothlingMovement))]
public class FMothling: MonoBehaviour, ICollidable, IInitializable
{
    [SerializeField] private float _collisionRadius = 0.075f;
    [SerializeField] private FMothlingMovement _movement;
    
    public float Radius => _collisionRadius;
    public Vector2 Position => transform.position;

    public void Initialize()
    {
        _movement.Initialize();
    }

    public void Play()
    {
        _movement.Play();
    }

    public void Attack()
    {
        _movement.TriggerAttack();
    }

    public void HandleCollision(Vector2 newPosition)
    {
        Vector3 newPosition3d = transform.position;
        newPosition3d.x = newPosition.x;
        newPosition3d.y = newPosition.y;
        transform.position = newPosition;
        
        _movement.TriggerFall();
    }
}
