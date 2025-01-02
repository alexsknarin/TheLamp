using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    [SerializeField] private EnemyBase _enemyPrefab;

    private void OnEnable()
    {
        BossBase.BossDied += HandleBossEnd;
    }
    
    private void OnDisable()
    {
        BossBase.BossDied -= HandleBossEnd;
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
                _enemyPrefab.StartAttack();    
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _enemyPrefab.ReceiveDamage(2);
        }
        
    }
}
