using System;
using System.Collections.Generic;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.GameCoreSystems.DataManagement.GameDataHandlers;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using IDisposable = _GAME.Scripts.Lib.Interfaces.IDisposable;

namespace _GAME.Scripts.GameCoreSystems.EnemyManagement
{
    public class EnemySpawner: ITickable, IDisposable
    {
        // Dependencies
        private readonly EnemyPool _enemyPool;
        private readonly float _firstEnemySpawnDelay;
        private ISpiderSpawnAvailablityProvider _spiderSpawnAvailablityProvider;
        
        private EnemyQueue _enemyQueue;
        private List<Enemy> _activeEnemies; 
        private int _currentEnemyIndex;
        private float _spawnCooldown;
        private float _localTime;
        private bool _isWaveActive = false;
        
        public EnemySpawner(
            EnemyPool enemyPool,
            float firstEnemySpawnDelay,
            ISpiderSpawnAvailablityProvider spiderSpawnAvailablityProvider
            )
        {
            _enemyPool = enemyPool;
            _firstEnemySpawnDelay = firstEnemySpawnDelay;
            _spiderSpawnAvailablityProvider = spiderSpawnAvailablityProvider;
        }
    
        public event Action<Enemy> EnemySpawned;
        public event Action<Enemy> EnemyReturnedToPool;

        public void Initialize()
        {
            _enemyPool.EnemyReleasedToPool += OnEnemyReleasedToPool;
        }

        public void Dispose()
        {
            _enemyPool.EnemyReleasedToPool -= OnEnemyReleasedToPool;
        }

        private void OnEnemyReleasedToPool(Enemy enemy)
        {
            EnemyReturnedToPool?.Invoke(enemy);
        }

        public void PrepareWave(EnemyQueue enemyQueue, List<Enemy> enemies)
        {
            _enemyQueue = enemyQueue;
            _activeEnemies = enemies;
            _spiderSpawnAvailablityProvider.Reset();
        
            LogCurrentWaveData();
        }

        public void StartWave()
        {
            _currentEnemyIndex = 0;
            _localTime = 0;
            _spawnCooldown = _firstEnemySpawnDelay;
            _isWaveActive = true;
            
        }

        public void StopWave()
        {
            _isWaveActive = false;
        }

        public void Tick(float deltaTime)
        {
            if (_isWaveActive)
            {
                WaitForCooldown(deltaTime);
            }
        }

        private void WaitForCooldown(float deltaTime)
        {
            if (_localTime >= _spawnCooldown)
            {
                _localTime = 0;
                SpawnEnemies();
                _spawnCooldown = UpdateSpawnCooldown();
            }
            else
            {
                _localTime += deltaTime;
            }
        }

        private void SpawnEnemies()
        {
            if (_currentEnemyIndex < _enemyQueue.Count())
            {
                if (_activeEnemies.Count < _enemyQueue.MaxEnemiesOnScreen)
                {
                    // TODO: refactor
                    EnemyType enemyType = _enemyQueue.Get(_currentEnemyIndex);
                    
                    if (enemyType == EnemyType.Spider && !_spiderSpawnAvailablityProvider.CheckPointAvailability())
                    {
                        bool hasReplacement = _enemyQueue.PushEnemyForward(_currentEnemyIndex, enemyType);
                        if (!hasReplacement)
                        {
                            return;
                        }
                        enemyType = _enemyQueue.Get(_currentEnemyIndex);
                    }
                    
                    Enemy enemy = SpawnEnemy(enemyType);
                    _activeEnemies.Add(enemy);
                    EnemySpawned?.Invoke(enemy);
                    _currentEnemyIndex++;
                }
                else
                {
                    _localTime = 0; 
                }
            }
            else
            {
                // Stop Spawning
                _isWaveActive = false;
            }
        }

        private Enemy SpawnEnemy(EnemyType enemyType)
        {
            var enemy = _enemyPool.Get(EnemyTypeLibrary.EnemyTypeDictionary[enemyType]);
            enemy.Play();
            return enemy;
        }

        private float UpdateSpawnCooldown()
        {
            float spawnDelayPhase = (float)(_currentEnemyIndex-1) / (_enemyQueue.Count()-1);
            float spawnDelayAcceleration = 1f/Mathf.Lerp(1, _enemyQueue.SpawnDelayAcceleration, spawnDelayPhase);
            return _enemyQueue.SpawnDelay * spawnDelayAcceleration;
        }

        private void LogCurrentWaveData()
        {
            string waveData = "";
            for (int i = 0; i < _enemyQueue.Count(); i++)
            {
                _enemyPool.PreloadEnemy(EnemyTypeLibrary.EnemyTypeDictionary[_enemyQueue.Get(i)]);
                waveData = waveData + " - " + _enemyQueue.Get(i).ToString();
            }
            Debug.Log(waveData);
        }
    }
}
