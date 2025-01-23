using System.Collections.Generic;
using UnityEngine;

public class EnemiesFireflyExploder : ITickable
{
    private EnemyBase _explosionSource;
    private Vector3 _explosionPosition;
    private bool _isExploding = false;
    private float _localTime;
    // Dependencies
    private List<EnemyBase> _enemies;
    private FireflyExplosion _fireflyExplosion;
    private float _fireflyExplosionRadius;
    private float _duration;

    public EnemiesFireflyExploder(List<EnemyBase> enemies, FireflyExplosion fireflyExplosion, float fireflyExplosionRadius, float duration)
    {
        _enemies = enemies;
        _fireflyExplosion = fireflyExplosion;
        _fireflyExplosionRadius = fireflyExplosionRadius;
        _duration = duration;
    }
    
    public void StartExplosion(EnemyBase explosionSource)
    {
        _explosionSource = explosionSource;
        _explosionPosition = explosionSource.transform.position;
        _fireflyExplosion.Play(_explosionPosition, _fireflyExplosionRadius * 2, _duration); 
        _isExploding = true;
        _localTime = 0;
    }

    public void Tick(float deltaTime)
    {
        if (_isExploding)
        {
            if (_localTime < _duration)
            {
                PerformExplosion();
                _localTime += deltaTime;
            }
            else
            {
                _isExploding = false;
            }
        }
    }

    private void PerformExplosion()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy == _explosionSource)
            {
                continue;
            }
            Vector3 enemyPosition2d = enemy.transform.position;
            enemyPosition2d.z = 0; // TODO: take camera projection into account
            Vector3 explosionPosition2d = _explosionPosition;
            explosionPosition2d.z = 0;
            if((explosionPosition2d - enemyPosition2d).magnitude < _fireflyExplosionRadius)
            {
                enemy.ReceiveDamage(100);
            }
        }
    }
}
