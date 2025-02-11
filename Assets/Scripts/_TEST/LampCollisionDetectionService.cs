using System;
using System.Collections.Generic;
using UnityEngine;

public class LampCollisionDetectionService : MonoBehaviour
{
    [SerializeField] private float _collisionRadius = 0.5f;
    [SerializeField] private float _attackZoneRadius = 0.62f;
    [SerializeField] private float _attackExitZoneRadius = 0.55f;
    [SerializeField] private int _collidableCount = 0;
    private List<ICollidableWithLamp> _collidables = new();
    private List<ICollidableWithLamp> _collidablesToRemove = new();
    
    private Vector2 _position;
    private float _collisionThreshold = 0.0001f;

    public void AddCollidable(ICollidableWithLamp collidableWithLamp)
    {
        if (!_collidables.Contains(collidableWithLamp))
        {
            enabled = true;
            _collidables.Add(collidableWithLamp);
            _collidableCount++;
        }
    }

    public void RemoveCollidable(ICollidableWithLamp collidable)
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
            CheckCollidables();
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

    private void CheckCollidables()
    {
        foreach (var collidable in _collidables)
        {
            // Get Current distance
            Vector2 targetPosition = collidable.Position;
            Vector2 directionRaw = targetPosition - _position;
            float distance = directionRaw.magnitude;
            
            // Collision Measurements
            float combinedRadius = collidable.Radius + _collisionThreshold;
            float attackZoneCombinedRadius = _attackZoneRadius + combinedRadius;
           
            
            // Entering Attack Zone
            if (collidable.CollisionState == CollidableState.Outside &&
                distance < attackZoneCombinedRadius)
            {
                Debug.Log("Attack zone enter Detected");
                Debug.Log($"EnemyType: {collidable.GetType()}");
                collidable.HandleEnterAttackZone();
            }
            
            // Exiting Attack Zone Before Collision
            if (collidable.CollisionState == CollidableState.InAttackZone &&
                distance > attackZoneCombinedRadius)
            {
                Debug.Log("Attack zone exit before collision Detected");
                Debug.Log($"EnemyType: {collidable.GetType()}");
                _collidablesToRemove.Add(collidable);
                collidable.HandleExitAttackZone();
            }
            
            // Collision detection
            if (collidable.CollisionState == CollidableState.InAttackZone &&
                distance < _collisionRadius + combinedRadius)
            {
                Debug.Log("Collision detected");
                Debug.Log($"EnemyType: {collidable.GetType()}");
                collidable.HandleCollision();
            }
            
            // Exiting Attack Zone After Collision
            if (collidable.CollisionState == CollidableState.AfterCollision &&
                distance > _attackExitZoneRadius + combinedRadius)
            {
                Debug.Log("Attack zone exit after collision Detected");
                Debug.Log($"EnemyType: {collidable.GetType()}");
                _collidablesToRemove.Add(collidable);
                collidable.HandleExitAttackZone();
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
