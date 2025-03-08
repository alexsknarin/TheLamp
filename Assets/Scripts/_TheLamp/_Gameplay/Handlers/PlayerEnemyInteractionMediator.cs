using System;
using UnityEngine;

public class PlayerEnemyInteractionMediator: IInitializable, IDisposable
{
    private WaveEnemyDirector _waveEnemyDirector; // TODO: interface
    private LampCollisionDetectionService _lampCollisionDetectionService;
    private LampStickyDetectionService _lampStickyDetectionService;
    
    public PlayerEnemyInteractionMediator(
        WaveEnemyDirector waveEnemyDirector, 
        LampCollisionDetectionService lampCollisionDetectionService, 
        LampStickyDetectionService lampStickyDetectionService
        )
    {
        _waveEnemyDirector = waveEnemyDirector;
        _lampCollisionDetectionService = lampCollisionDetectionService;
        _lampStickyDetectionService = lampStickyDetectionService;
    }
    
    public event Action<Vector3, bool, string> EnemyAttackEnded;
    public event Action<IStickableWithLamp> EnemySticked;
    public event Action<IStickableWithLamp> EnemyUnSticked;

    public void Initialize()
    {
        _waveEnemyDirector.EnemyAttackStarted += OnEnemyAttackStarted;
        _waveEnemyDirector.StickyEnemyRedyToStick += OnStickyEnemyRedyToStick;
        
        _lampCollisionDetectionService.EnemyAttackEnded += OnEnemyAttackEnded;

        _lampStickyDetectionService.AttackBlocked += _waveEnemyDirector.BlockAttackCooldown;
        _lampStickyDetectionService.AttackUnblocked += _waveEnemyDirector.UnblockAttackCooldown;
        _lampStickyDetectionService.EnemySticked += OnEnemySticked;
        _lampStickyDetectionService.EnemyUnSticked += OnEnemyUnSticked;
    }

    public void Dispose()
    {
        _waveEnemyDirector.EnemyAttackStarted -= OnEnemyAttackStarted;
        _waveEnemyDirector.StickyEnemyRedyToStick -= OnStickyEnemyRedyToStick;
        
        _lampCollisionDetectionService.EnemyAttackEnded -= OnEnemyAttackEnded;
        
        _lampStickyDetectionService.AttackBlocked -= _waveEnemyDirector.BlockAttackCooldown;
        _lampStickyDetectionService.AttackUnblocked -= _waveEnemyDirector.UnblockAttackCooldown;
        _lampStickyDetectionService.EnemySticked -= OnEnemySticked;
        _lampStickyDetectionService.EnemyUnSticked -= OnEnemyUnSticked;
    }
    
    public void SetAttackZoneRadius(float radius)
    {
        _lampCollisionDetectionService.SetAttackZoneRadius(radius);
        _lampStickyDetectionService.SetAttackZoneRadius(radius);
    }
    
    public void Reset()
    {
        _lampCollisionDetectionService.Reset();
        _lampStickyDetectionService.Reset();
    }

    private void OnEnemyAttackStarted(CollidableEnemy enemy)
    {
        _lampCollisionDetectionService.AddCollidable(enemy);
    }

    private void OnStickyEnemyRedyToStick(IStickableWithLamp enemy)
    {
        _lampStickyDetectionService.AddStickable(enemy);
    }
    
    private void OnEnemyAttackEnded(Vector3 impactPoint, bool isEnemyDamaged, string enemyTypeName)
    {
        EnemyAttackEnded?.Invoke(impactPoint, isEnemyDamaged, enemyTypeName);
    }
    
    private void OnEnemySticked(IStickableWithLamp enemy)
    {
        _waveEnemyDirector.AddStickyEnemy(enemy);
        EnemySticked?.Invoke(enemy);
    }
    
    private void OnEnemyUnSticked(IStickableWithLamp enemy)
    {
        _waveEnemyDirector.RemoveStickyEnemy(enemy);
        EnemyUnSticked?.Invoke(enemy);
    }
}
