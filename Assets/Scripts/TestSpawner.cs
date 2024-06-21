using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    [SerializeField] private EnemyBase _enemyPrefab;

    private void OnEnable()
    {
        BossBase.OnDeath += HandleBossEnd;
    }
    
    private void OnDisable()
    {
        BossBase.OnDeath -= HandleBossEnd;
    }

    private void Start()
    {
        _enemyPrefab.Initialize();
    }

    private void HandleBossEnd()
    {
        // _enemyPrefab.Reset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _enemyPrefab.Initialize();
            // _enemyPrefab.Play();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            // _enemyPrefab.Reset();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _enemyPrefab.UpdateAttackAvailability();
            if (_enemyPrefab.ReadyToAttack)
            {
                _enemyPrefab.AttackStart();    
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _enemyPrefab.ReceiveDamage(2);
        }
        
    }
}
