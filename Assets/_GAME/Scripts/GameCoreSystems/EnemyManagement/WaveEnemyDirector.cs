using System;
using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.GameCoreSystems.DataManagement.GameDataHandlers;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems.EnemyManagement
{
    public class WaveEnemyDirector : MonoBehaviour, IInitializable, IProjectileDeactivatedProvider
    {
        [SerializeField] private Transform _cameraTransform; 
        private SpawnQueueGenerator _spawnQueueGenerator;
        private SpawnQueue _spawnQueue;
        private EnemyQueue _currentWaveEnemyQueue;
        private List<FEnemy> _enemies = new ();
        private List<IStickableWithLamp> _stickedEnemies = new ();
        private int _enemiesKilledCount = 0;
        private LampAttacker _lampAttacker;
        private FireflyExplosionEnemyDamager _fireflyExplosionEnemyDamager;
        [SerializeField] private bool _lampBlocked = false;
        private EnemyAttacker _enemyAttacker;
        private WaitForSeconds _waitAfterGameOver = new (1f);
        private WaitForSeconds _waitToDeactivateEnemies;

        // Dependencies
        private IGameConfigService _gameConfigService;
        private EnemySpawner _enemySpawner;
    
        public void Construct(
            IGameConfigService gameConfigService, 
            EnemySpawner enemySpawner
        )
        {
            _gameConfigService = gameConfigService;
            _enemySpawner = enemySpawner;
        }
    
        public event Action WaveEnded;
        public event Action<CollidableEnemy> EnemyAttackStarted;
        public event Action<IStickableWithLamp> StickyEnemyReadyToStick;
        public event Action LampBlocked;
        public event Action LampUnblocked;
        public event Action ExplodableEnemySpawned;
        public event Action<FEnemy> ExplodableEnemyDeactivated;
        public event Action FireflyExplosionStarted;
        public event Action<FEnemy> BossSpawned;
        public event Action BossDied;
        public event Action<Vector3, bool, string> StickyAttackEnded;
        public event Action<FEnemy> ProjectileDestroyed;
    
        public void Initialize()
        {
            Debug.Log("WaveEnemyDirector: Initializing");
            _spawnQueueGenerator = new SpawnQueueGenerator(_gameConfigService.SpawnQueueConfig.Data);
            _spawnQueue = _spawnQueueGenerator.Generate();
            _lampAttacker = new LampAttacker();
            _enemyAttacker = new EnemyAttacker(_gameConfigService.GameConfig.MaxAggressionLevel);
            _fireflyExplosionEnemyDamager = new FireflyExplosionEnemyDamager(_gameConfigService, _cameraTransform);
            _fireflyExplosionEnemyDamager.Initialize();
        
            _waitToDeactivateEnemies = new WaitForSeconds(_gameConfigService.GameConfig.GameoverInStageDuration);
        
            _enemySpawner.EnemySpawned += OnEnemySpawned;
            _enemySpawner.EnemyReturnedToPool += OnEnemyDead;
            _enemyAttacker.EnemyAttackStarted += OnEnemyAttackStarted;
        
            enabled = false;
        
            // Debug Spawn Queue
            Debug.Log("++++ ----- Spawn Queue Generated:");
            for(int i=0; i<_spawnQueue.Count(); i++)
            {
                Debug.Log($"Wave : {i} --- Count: {_spawnQueue.Get(i).Count()}");
                string waveData = "";
                for(int j=0; j<_spawnQueue.Get(i).Count(); j++)
                {
                    waveData = waveData + " - " + _spawnQueue.Get(i).Get(j).ToString();
                }
                Debug.Log(waveData);
            }
        }

        private void OnDestroy()
        {
            _enemySpawner.EnemySpawned -= OnEnemySpawned;
            _enemySpawner.EnemyReturnedToPool -= OnEnemyDead;
            _enemyAttacker.EnemyAttackStarted -= OnEnemyAttackStarted;
        }

        public void PrepareWave(int waveIndex)
        {
            _currentWaveEnemyQueue = _spawnQueue.Get(waveIndex);
            Debug.Log($"WaveEnemyDirector: Preparing Wave {waveIndex}");
            _enemySpawner.PrepareWave(_currentWaveEnemyQueue, _enemies);
            _enemyAttacker.PrepareWave(_currentWaveEnemyQueue.AggressionLevel, _enemies);
            _stickedEnemies.Clear();
            _lampAttacker.SetUnBlockedMode();
        
            // TODO: wave prepared switch on ????
        
        }

        public void StartWave()
        {
            _lampBlocked = false;
            _enemiesKilledCount = 0;
            _enemySpawner.StartWave();
            _enemyAttacker.StartWave();
            enabled = true;
        }

        public void Reset()
        {
            ReturnAllActiveEnemiesToPool();
            _enemies.Clear();
            _stickedEnemies.Clear();
            _enemySpawner.StopWave();
            _spawnQueue = _spawnQueueGenerator.Generate();
            _enemyAttacker.StopWave();
        
        }

        private void StopWave()
        {
            _enemyAttacker.StopWave();
            _enemySpawner.StopWave();
            enabled = false;
            WaveEnded?.Invoke();
        }

        public void HandleAttackButtonClicked(float power)
        {
            _lampAttacker.Attack(power, _enemies);
        }
    
        public void HandleLampDestroyed()
        {
            _enemyAttacker.StopWave();
            _enemySpawner.StopWave();

            foreach (var enemy in _enemies)
            {
                Debug.Log("Lamp Destroyed. Enemies left: " + enemy.gameObject.name);    
                if (enemy is IStickableWithLamp)
                {
                    ((IStickableWithLamp)enemy).HandleLampDestroyed();
                }
                if (enemy is ILampDestroyedDependable)
                {
                    ((ILampDestroyedDependable)enemy).HandleLampDestroyed();
                }
                
                enemy.IsGameOver = true;
            }
            StartCoroutine(SpreadEnemiesAfterGameOver());
            StartCoroutine(DeactivateEnemiesAfterGameOver());
            enabled = false;
        }

        // Calls from PlayerEnemyInteractionMediator
        public void BlockEnemyAttackCooldown()
        {
            _enemyAttacker.BlockAttackCooldown();
        }

        public void UnblockEnemyAttackCooldown()
        {
            _enemyAttacker.UnblockAttackCooldown();
        }

        public void AddStickyEnemy(IStickableWithLamp enemy)
        {
            if (!_stickedEnemies.Contains(enemy))
            {
                _stickedEnemies.Add(enemy);
                if (!_lampBlocked)
                {
                    _lampBlocked = true;
                    LampBlocked?.Invoke();
                    _lampAttacker.SetBlockedMode();
                }
            }
        }

        public void RemoveStickyEnemy(IStickableWithLamp enemy)
        {
            if (_stickedEnemies.Contains(enemy))
            {
                _stickedEnemies.Remove(enemy);
            }
            if (_stickedEnemies.Count == 0)
            {
                _lampBlocked = false;
                _lampAttacker.SetUnBlockedMode();
                LampUnblocked?.Invoke();
            }
        }
        //


        private IEnumerator SpreadEnemiesAfterGameOver()
        {
            yield return _waitAfterGameOver;
            SpreadEnemies();
        }
    
        private void SpreadEnemies()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy is ISpreadable)
                {
                    ((ISpreadable)enemy).Spread();
                }
            }
        }
    
        private IEnumerator DeactivateEnemiesAfterGameOver()
        {
            yield return _waitToDeactivateEnemies;
            ReturnAllActiveEnemiesToPool();
        }
    
        private void ReturnAllActiveEnemiesToPool()
        {
            enabled = false;
            Debug.Log("Return all enemies to pool CALLED");
            // Enemies
            if (_enemies.Count > 0)
            {
                foreach (var enemy in _enemies)
                {
                    Debug.Log("Return enemy to pool:" + enemy.gameObject.name);
                    if (enemy.gameObject.activeInHierarchy)
                    {
                        enemy.ReturnToPool();                        
                    }
                }            
            }
            _enemies.Clear();
        }

        private void OnEnemySpawned(FEnemy enemy)
        {
            if (enemy is IStickableWithLamp)
            {
                ((IStickableWithLamp)enemy).StickReadyStarted += OnStickReadyStarted;
            }
            if (enemy is IExplodable)
            {
                ExplodableEnemySpawned?.Invoke();
            }
            if (enemy is IBoss)
            {
                BossSpawned?.Invoke(enemy);
                ((IBoss)enemy).SpreadRequested += OnBossRequestedSpread;
            }
            if (enemy is IAnimatedEnemy)
            {
                ((IAnimatedEnemy)enemy).AnimatedAttackStarted += OnEnemyAttackStarted;
            }
            if (enemy is IStickyAttacker)
            {
                ((IStickyAttacker)enemy).StickyAttackEnded += OnStickyAttackEnded;
            }
            if (enemy is IProjectileShooter)
            {
                ((IProjectileShooter)enemy).ProjectileShot += OnProjectileShot;
                ((IProjectileShooter)enemy).ProjectileDeactivated += OnProjectileDeactivated;
            }
        }

        private void OnEnemyDead(FEnemy enemy)
        {
            // TODO: find better way to return enemies to pool that will work better with gameover one
            if (enabled)
            {
                if (_enemies.Contains(enemy))
                {
                    _enemies.Remove(enemy);
                    _enemiesKilledCount++;

                    if (_enemiesKilledCount == _currentWaveEnemyQueue.Count())
                    {
                        StopWave();
                    }
                }
                else
                {
                    Debug.LogError("WaveEnemyDirector: Enemy not found in list");
                }

                if (enemy is IExplodable)
                {
                    ExplodableEnemyDeactivated?.Invoke(enemy);
                    FireflyExplosionStarted?.Invoke();
                    _fireflyExplosionEnemyDamager.StartExplosion(enemy.transform.position, _enemies);
                }
                if (enemy is IBoss)
                {
                    BossDied?.Invoke();
                    ((IBoss)enemy).SpreadRequested -= OnBossRequestedSpread;
                }
                if (enemy is IAnimatedEnemy)
                {
                    ((IAnimatedEnemy)enemy).AnimatedAttackStarted -= OnEnemyAttackStarted;
                }
                if (enemy is IStickableWithLamp)
                {
                    ((IStickableWithLamp)enemy).StickReadyStarted -= OnStickReadyStarted;
                }
                if (enemy is IStickyAttacker)
                {
                    ((IStickyAttacker)enemy).StickyAttackEnded -= OnStickyAttackEnded;
                }

                if (enemy is IProjectileShooter)
                {
                    ((IProjectileShooter)enemy).ProjectileShot -= OnProjectileShot;
                    ((IProjectileShooter)enemy).ProjectileDeactivated -= OnProjectileDeactivated;
                }
            }
        }


        private void OnStickReadyStarted(IStickableWithLamp stickable)
        {
            StickyEnemyReadyToStick?.Invoke(stickable);
        }

        private void OnBossRequestedSpread()
        {
            SpreadEnemies();
        }

        private void OnEnemyAttackStarted(CollidableEnemy enemy)
        {
            EnemyAttackStarted?.Invoke(enemy);
        }

        private void OnStickyAttackEnded(Vector3 impactPoint, bool isEnemyDamaged, string enemyTypeName)
        {
            StickyAttackEnded?.Invoke(transform.position, false, "Megabeetle");
        }
    
        private void OnProjectileShot(CollidableEnemy enemy)
        {
            EnemyAttackStarted?.Invoke(enemy);
            _enemies.Add(enemy);
        }
    
        private void OnProjectileDeactivated(FEnemy enemy, bool damaged)
        {
            if (damaged)
            {
                ProjectileDestroyed?.Invoke(enemy);
            }
        
            if (_enemies.Contains(enemy))
            {
                _enemies.Remove(enemy);
            }
        }

        private void Update()
        {
            _enemyAttacker.Tick(Time.deltaTime);
            _fireflyExplosionEnemyDamager.Tick(Time.deltaTime);
        }


    
    }
}
