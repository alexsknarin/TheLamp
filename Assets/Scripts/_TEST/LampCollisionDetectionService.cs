using System;
using System.Collections.Generic;
using UnityEngine;

public class LampCollisionDetectionService : MonoBehaviour
{
    [SerializeField] private float _collisionRadius = 0.5f;
    [SerializeField] private float _attackZoneRadius = 0.62f;
    [SerializeField] private float _attackExitZoneRadius = 0.55f;
    [SerializeField] private int _collidableCount = 0;
    private List<ICollidable> _collidables = new();
    private List<ICollidable> _collidablesToRemove = new();
    private Vector2 _position;
    private float _collisionThreshold = 0.0001f;

    public void AddCollidable(ICollidable collidable)
    {
        // Need to check if the collidable is already in the list
        if (!_collidables.Contains(collidable))
        {
            enabled = true;
            _collidables.Add(collidable);
            _collidableCount++;
        }
    }
    
    public void RemoveCollidable(ICollidable collidable)
    {
        if (_collidables.Contains(collidable))
        {
            _collidables.Remove(collidable);
            _collidableCount--;
        }
    }

    private void Update()
    {
        _position = transform.position;
        if (_collidables.Count != 0)
        {
            foreach (var collidable in _collidables)
            {
                Vector2 targetPosition = collidable.Position;
                Vector2 directionRaw = targetPosition - _position;
                float distance = directionRaw.magnitude;
            
                // Attack zone Enter detection
                
                // Collision detection
                if (distance < _collisionRadius + collidable.Radius + _collisionThreshold)
                {
                    Debug.Log("Collision detected");
                    Debug.Log($"EnemyType: {collidable.GetType()}");
                    Vector2 newCollidablePosition = 
                        directionRaw.normalized * (_collisionRadius + collidable.Radius + _collisionThreshold) + _position;
                    collidable.HandleCollision(newCollidablePosition);
                    _collidablesToRemove.Add(collidable);
                }
            }    
        }
    }
    
    private void LateUpdate()
    {
        if (_collidablesToRemove.Count != 0)
        {
            foreach (var collidable in _collidablesToRemove)
            {
                RemoveCollidable(collidable);
            }
            _collidablesToRemove.Clear();
            
            if (_collidables.Count == 0)
            {
                enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _collisionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackZoneRadius);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, _attackExitZoneRadius);
    }
}
