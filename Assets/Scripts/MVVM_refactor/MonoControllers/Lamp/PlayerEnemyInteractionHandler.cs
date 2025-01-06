using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyInteractionHandler : MonoBehaviour, IInitializable
{
    [SerializeField] private LampCollisionHandler _lampCollisionHandler;
    [SerializeField] private LampAttackExitZoneCollisionHandler _lampAttackExitZoneCollisionHandler;
    [SerializeField] private LampStickZoneCollisionHandler _lampStickZoneCollisionHandler;
    private bool _isAssessingDamage = false;
    private List<EnemyBase> _stickyEnemies; // TODO: replace Enemy with ISticky Interface
    
    public event Action<bool, EnemyBase> EnemyAttackBounced;  // TODO: Use Interface
    public event Action<bool, EnemyBase> LampBlockedStarted; // TODO: Use Interface

    public void Initialize()
    {
        _lampCollisionHandler.LampCollidedWithEnemy += OnLampCollidedWithEnemy;
        _lampCollisionHandler.EnemyExitedLampCollision += OnEnemyExitedLampCollision;
        _lampAttackExitZoneCollisionHandler.EnemyExitedAttackExitZone += OnEnemyExitedAttackExitZone;
        _lampStickZoneCollisionHandler.CollidedWithStickyEnemy += OnCollidedWithStickyEnemy;
        
        if (_stickyEnemies == null)
            _stickyEnemies = new List<EnemyBase>();
        else
            _stickyEnemies.Clear();
    }

    private void OnDestroy()
    {
        _lampCollisionHandler.LampCollidedWithEnemy -= OnLampCollidedWithEnemy;
        _lampCollisionHandler.EnemyExitedLampCollision -= OnEnemyExitedLampCollision;
        _lampAttackExitZoneCollisionHandler.EnemyExitedAttackExitZone -= OnEnemyExitedAttackExitZone;
        _lampStickZoneCollisionHandler.CollidedWithStickyEnemy -= OnCollidedWithStickyEnemy;
    }

    public void LampAttack()
    {
        if (_isAssessingDamage)
        {
            _isAssessingDamage = false;
        }
    }

    public void Reset()
    {
        Debug.Log("Resetting PlayerEnemyInteractionHandler - _isAssessingDamage disabled");
        _isAssessingDamage = false;
    }
    
    /// <summary>
    /// Register potential Damage
    /// </summary>
    /// <param name="enemy"></param>
    private void OnLampCollidedWithEnemy(EnemyBase enemy)
    {
        if (!_isAssessingDamage)
        {
            _isAssessingDamage = true;
        }
    }

    /// <summary>
    /// Assess if Damage was made
    /// </summary>
    /// <param name="enemy"></param>
    private void OnEnemyExitedAttackExitZone(EnemyBase enemy)
    {
        Debug.Log("Trying to Assess Damage");
        if (_isAssessingDamage)
        {
            Debug.Log("Assessing Damage");
            if (enemy.ReceivedLampAttack && (enemy.EnemyType != EnemyType.Ladybug || enemy.EnemyType != EnemyType.Megabeetle)) // TODO: replace with ISticky interface
            {
                _isAssessingDamage = false;
                EnemyAttackBounced?.Invoke(true, enemy);
            }
            else
            {
                _isAssessingDamage = false;
                EnemyAttackBounced?.Invoke(false, enemy);
            }
        }
    }

    private void OnCollidedWithStickyEnemy(EnemyBase enemy)
    {
        enemy.transform.parent = transform;
        enemy.HandleCollisionWithStickZone();
        if (!_stickyEnemies.Contains(enemy))
        {
            _stickyEnemies.Add(enemy);
        }
        LampBlockedStarted?.Invoke(true, enemy);
    }
    
    // TODO: refactor this
    private void OnEnemyExitedLampCollision(EnemyBase enemy)
    {
        if (enemy.EnemyType == EnemyType.Ladybug || enemy.EnemyType == EnemyType.Megabeetle) // Use ISticky interface
        {
            if (_stickyEnemies.Contains(enemy))
            {
                _stickyEnemies.Remove(enemy);
                if( _stickyEnemies.Count <= 0)
                {
                    LampBlockedStarted?.Invoke(false, enemy);
                }    
            }
            if (enemy.gameObject.activeInHierarchy)
            {
                enemy.transform.parent = null;
            }
        }
    }
}


