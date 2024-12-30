using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyInteractionHandler : MonoBehaviour, IInitializable
{
    [SerializeField] private LampCollisionHandler _lampCollisionHandler;
    [SerializeField] private LampAttackExitZoneCollisionHandler _lampAttackExitZoneCollisionHandler;
    [SerializeField] private LampStickZoneCollisionHandler _lampStickZoneCollisionHandler;
    
    public event Action<bool, EnemyBase> OnEnemyAttackDeflectedEvent;  // TODO: Use Interface
    public event Action<bool, EnemyBase> OnLampBlockedSetEvent; // TODO: Use Interface
    
    private bool _isAssessingDamage = false;
    
    private List<EnemyBase> _stickyEnemies; // TODO: replace Enemy with ISticky Interface
    
    public void Initialize()
    {
        _lampCollisionHandler.OnLampCollidedEnemyEvent += RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemyEvent += EnemyExitCollisionHandle;
        _lampAttackExitZoneCollisionHandler.OnExitAttackExitZoneEvent += AssessDamage;
        _lampStickZoneCollisionHandler.OnCollidedWithStickyEnemyEvent += StickyEnemyEnterCollisionHandle;
        
        if (_stickyEnemies == null)
            _stickyEnemies = new List<EnemyBase>();
        else
            _stickyEnemies.Clear();
    }

    private void OnDestroy()
    {
        _lampCollisionHandler.OnLampCollidedEnemyEvent -= RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemyEvent -= EnemyExitCollisionHandle;
        _lampAttackExitZoneCollisionHandler.OnExitAttackExitZoneEvent -= AssessDamage;
        _lampStickZoneCollisionHandler.OnCollidedWithStickyEnemyEvent -= StickyEnemyEnterCollisionHandle;
    }

    public void LampAttack()
    {
        if (_isAssessingDamage)
        {
            _isAssessingDamage = false;
        }
    }

    private void RegisterPotentialDamage(EnemyBase enemy)
    {
        if (!_isAssessingDamage)
        {
            _isAssessingDamage = true;
        }
    }

    private void AssessDamage(EnemyBase enemy)
    {
        if (_isAssessingDamage)
        {
            if (enemy.ReceivedLampAttack && (enemy.EnemyType != EnemyType.Ladybug || enemy.EnemyType != EnemyType.Megabeetle)) // TODO: replace with ISticky interface
            {
                _isAssessingDamage = false;
                OnEnemyAttackDeflectedEvent?.Invoke(true, enemy);
            }
            else
            {
                _isAssessingDamage = false;
                OnEnemyAttackDeflectedEvent?.Invoke(false, enemy);
            }
        }
    }

    private void StickyEnemyEnterCollisionHandle(EnemyBase enemy)
    {
        enemy.transform.parent = transform;
        enemy.HandleCollisionWithStickZone();
        if (!_stickyEnemies.Contains(enemy))
        {
            _stickyEnemies.Add(enemy);
        }
        OnLampBlockedSetEvent?.Invoke(true, enemy);
    }
    
    // TODO: refactor this
    private void EnemyExitCollisionHandle(EnemyBase enemy)
    {
        if (enemy.EnemyType == EnemyType.Ladybug || enemy.EnemyType == EnemyType.Megabeetle) // Use ISticky interface
        {
            if (_stickyEnemies.Contains(enemy))
            {
                _stickyEnemies.Remove(enemy);
                if( _stickyEnemies.Count <= 0)
                {
                    OnLampBlockedSetEvent?.Invoke(false, enemy);
                }    
            }
            enemy.transform.parent = null;
        }
    }
}


