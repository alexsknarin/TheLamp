using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class LampCollisionDetectionService : MonoBehaviour, IInitializable
{
    [SerializeField] private float _collisionRadius = 0.5f;
    [SerializeField] private float _attackZoneRadius = 0.62f; // TODO: control from the single source
    [SerializeField] private float _attackExitZoneRadius = 0.55f;
    [SerializeField] private int _collidableCount = 0;
    [SerializeField] private string _collidableItems;
    private List<ICollidableWithLamp> _collidables = new();
    private List<ICollidableWithLamp> _collidablesToRemove = new();
    private Vector2 _position;
    private float _collisionThreshold = 0.0001f; // TODO: control from the config
    private float _combinedCollisionRadius;
    
    // TODO: add event for enemy collision
    public event Action<Vector3, bool, string> EnemyAttackEnded;
    
    public void Initialize()
    {
        _combinedCollisionRadius = _collisionRadius + _collisionThreshold;
        enabled = false;
    }
    
    public void Reset()
    {
        _collidables.Clear();
        _collidablesToRemove.Clear();
        _collidableCount = 0;
        enabled = false;
    }
    
    public void SetAttackZoneRadius(float attackZoneRadius)
    {
        _attackZoneRadius = attackZoneRadius;
    }

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
        
        // Debug
        _collidableItems = "";
        foreach (var collidable in _collidables)
        {
            _collidableItems = _collidableItems + ", " + collidable.GetType().ToString();
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
            float attackZoneCombinedRadius = _attackZoneRadius + collidable.Radius;
            
            // Entering Attack Zone
            if (collidable.CollisionState == CollidableState.Outside && distance < attackZoneCombinedRadius)
            {
                collidable.HandleEnterAttackZone();
                Debug.Log(((FEnemy)collidable).gameObject.name + " --- Entered Attack Zone.");
                Debug.Log("Position: " + collidable.Position);
                Debug.DrawLine(Vector3.zero, collidable.Position, Color.yellow, 1f);
            }
            
            // Exiting Attack Zone Before Collision
            if (collidable.CollisionState == CollidableState.InAttackZone && distance > attackZoneCombinedRadius)
            {
                _collidablesToRemove.Add(collidable);
                collidable.HandleExitAttackZone();
                Debug.Log(((FEnemy)collidable).gameObject.name + " --- Exited Attack Attack Zone Before Collision.");
                Debug.Log("Position: " + collidable.Position);
                Debug.DrawLine(Vector3.zero, collidable.Position, Color.yellow, 1f);
            }
            
            // Collision detection
            if (collidable.CollisionState == CollidableState.InAttackZone &&  distance < _combinedCollisionRadius + collidable.Radius)
            {
                collidable.HandleCollision();
                Debug.Log(((FEnemy)collidable).gameObject.name + " --- Collided.");
                Debug.Log("Position: " + collidable.Position);
                Debug.DrawLine(Vector3.zero, collidable.Position, Color.white, 1f);
            }
            
            // Exiting Attack Zone After Collision
            if (collidable.CollisionState == CollidableState.AfterCollision && distance > _attackExitZoneRadius + collidable.Radius)
            {
                _collidablesToRemove.Add(collidable);
                collidable.HandleExitAttackZone();
                EnemyAttackEnded?.Invoke(collidable.ProvideImpactPoint(), collidable.IsReceivedLampAttackDamage, collidable.GetType().ToString());
                Debug.Log($"Enemy {collidable.GetType().ToString()} has just attacked.");
                Debug.Log(((FEnemy)collidable).gameObject.name + " --- Exited Attack Zone after collision.");
                Debug.Log("Position: " + collidable.Position);
                Debug.DrawLine(Vector3.zero, collidable.Position, Color.green, 1f);
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
