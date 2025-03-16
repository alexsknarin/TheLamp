using System;
using System.Collections.Generic;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.GameCoreSystems.EnemyManagement
{
    public class EnemyAttacker: ITickable
    {
        // Dependencies
        private readonly float _maxAggressionLevel;
    
        private float _attackCooldown;
        private float _aggressionLevelNormalized;
        private bool _isWaveActive = false;
        private float _localTime;
        private bool _isCooldownActive = false;
    
        private List<FEnemy> _enemies; // TODO: find a way to remove this dependency and to not keep the list of enemies
        private List<CollidableEnemy> _enemiesReadyToAttack = new ();

        public EnemyAttacker(float maxAggressionLevel)
        {
            _maxAggressionLevel = maxAggressionLevel;
        }
    
        public event Action<CollidableEnemy> EnemyAttackStarted;
    
        public void PrepareWave(int aggressionLevel, List<FEnemy> enemies)
        {
            _enemies = enemies;
            _enemiesReadyToAttack.Clear();
            _aggressionLevelNormalized = aggressionLevel / _maxAggressionLevel;
        }
    
        public void StartWave()
        {
            // TODO: fix magic numbers
            _attackCooldown = GetRandomAttackDelay(4.5f, 1.8f, 6.5f, 2.8f, _aggressionLevelNormalized);
            _isWaveActive = true;
            _isCooldownActive = true;
            _localTime = 0;
        }
    
        public void StopWave()
        {
            _isWaveActive = false;
        }
    
        public void BlockAttackCooldown()
        {
            Debug.Log("Cooldown blocked");
            _isCooldownActive = false;
        }
    
        public void UnblockAttackCooldown()
        {
            Debug.Log("Cooldown unblocked");
            _isCooldownActive = true;
        }
    

    
        public void Tick(float deltaTime)
        {
            if (_isWaveActive)
            {
                UpdateEnemiesReadyToAttack();
                WaitForCooldown();    
            }
        }
    
        private void UpdateEnemiesReadyToAttack()
        {
            // TODO: this method is the only reason to keep the list of enemies
            // Need to find out how to update ready to attack without keeping the list ????
        
            _enemiesReadyToAttack.Clear();
       
            foreach (var enemy in _enemies)
            {
                if (enemy is CollidableEnemy && enemy.IsReadyToAttack)
                {
                    _enemiesReadyToAttack.Add((CollidableEnemy)enemy);
                }
            }
        }
    
        private void WaitForCooldown()
        {
            if (_localTime >= _attackCooldown)
            {
                _localTime = 0;
                Attack();
            }
            else
            {
                if (_isCooldownActive)
                {
                    // TODO: add exception for Megamothling and Megabeetle (BOSSES)
                    _localTime += Time.deltaTime;   
                }
            }
        }
    
        private void Attack()
        {
            // Select random Enemy from available
            int enemyIndex;
            if (_enemiesReadyToAttack.Count > 0)
            {
                enemyIndex = Random.Range(0, _enemiesReadyToAttack.Count);    
            }
            else
            {
                return;
            }
        
            var attackingEnemy = _enemiesReadyToAttack[enemyIndex];
        

            // MEGAMOTHLING: TODO: add later
            // It attacks alongside other enemies, but it should be
            // Prioritized to attack more often
            // In this case twice as often
        
            // if (_isBossActive && (_boss.EnemyType == EnemyType.Megamothling) && _boss.ReadyToAttack)
            // {
            //     int megamothlingAttackChance = Random.Range(0, 2);
            //     if (megamothlingAttackChance == 0)
            //     {
            //         attackingEnemy = _boss;
            //     }
            // }
       
            attackingEnemy.Attack();
            EnemyAttackStarted?.Invoke(attackingEnemy);
        
        
            _localTime = 0;
            // TODO: fix magic numbers
            _attackCooldown = GetRandomAttackDelay(2.5f, 0.8f, 6.1f, 1.8f, _aggressionLevelNormalized); 
        }
    
        private float GetRandomAttackDelay(float minMin, float minMax, float maxMin, float maxMax, float aggressionLevel)
        {
            return Random.Range(
                Mathf.Lerp(minMin, minMax, aggressionLevel),
                Mathf.Lerp(maxMin, maxMax, aggressionLevel)
            );
        }

    }
}
