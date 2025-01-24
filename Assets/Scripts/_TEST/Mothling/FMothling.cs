using System;
using UnityEngine;

public class FMothling: MonoBehaviour, ICollidable
{
    [SerializeField] private float _collisionRadius = 0.075f;
    [SerializeField] private FMothlingMovement _movement;
    
    public float Radius => _collisionRadius;
    public Vector2 Position => transform.position;

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

    private void Start()
    {
        _movement.Initialize();
        
        // TODO: control from enemy manager
        _movement.Play();
    }
}
