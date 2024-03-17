using System;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    [SerializeField] private float _enemyDamagePauseTime = 0.1f;
    private float _localTime;
    private bool _isPausedOnEnemyDamage = false;
    
    private void OnEnable()
    {
        EnemyManager.OnEnemyDamaged += OnDamagedEnemyPauseStart;
    }
    
    private void OnDisable()
    {
        EnemyManager.OnEnemyDamaged -= OnDamagedEnemyPauseStart;
    }

    private void OnDamagedEnemyPauseStart()
    {
        Time.timeScale = 0.2f;
        _localTime = 0;
        _isPausedOnEnemyDamage = true;
    }
   
    private void Update()
    {
        if (_isPausedOnEnemyDamage)
        {
            float phase = _localTime / _enemyDamagePauseTime;
            if(phase > 1)
            {
                Time.timeScale = 1f;
                _isPausedOnEnemyDamage = false;
            }

            _localTime += Time.unscaledDeltaTime;
        }
    }
}
