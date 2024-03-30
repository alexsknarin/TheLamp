using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour,IInitializable
{
    [SerializeField] private SpawnQueueData _spawnQueueDataCache;
    private SpawnQueueGenerator _spawnQueueGenerator;
    private SpawnQueue _spawnQueue;
    private EnemyQueue _enemyQueue;
    
    [Header("------ Enemy Prefabs -------")]
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] private BossBase _waspBoss;
    private BossBase _currentBoss;
    
    [Header("------ Explosions -------")]
    [SerializeField] private FireflyExplosion _fireflyExplosion;
    [SerializeField] private float _fireflyExplosionRadius;
    [SerializeField] private float _explosionDuration;
    private EnemyBase _explosionSource;
    private Vector3 _explosionPosition;
    private bool _isExplosionActive = false;
    
    [Header("---- Waves Generation ------")]
    [SerializeField] private int _maxEnemiesOnScreen;
    [SerializeField] private int _aggresionLevel;
    [SerializeField] private float _maxAggressionLevel;
    private float _aggressionLevelNormalized;
    
    [Header("")]
    [SerializeField] private int _startAtWave = 0;
    [SerializeField] private int _currentWave = 0;
    [SerializeField] private float _firstEnemySpawnDelay;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _spawnDelayAcceleration;
    
    public int CurrentWave => _currentWave;
    private int _enemiesInWave;
    private int _enemiesAvailable;
    private int _enemiesKilled;
    private int _currentSpawnEnemyIndex;
    
    
    [Header("---- Debug ------")]
    [SerializeField] private int _enemiesInWaveCount;
    [SerializeField] private int _enemiesLeftCount;
    
    private List<EnemyBase> _enemies;
    private List<EnemyBase> _enemiesReadyToAttack;
    private List<EnemyBase> _ladybugsPatrolling;
    private EnemiesLampAttackHandler _enemiesLampAttackHandler;
    private EnemiesExplosionHandler _enemiesExplosionHandler;

        private bool _isWaveInitialized = false;
    
    private float _attackDelay;
    private float _attackLocalTime;
    private bool _isAttacking;
    
    private float _localTime;
    private float _explosionLocalTime;
    
    private bool _isBossActive = false;
    private int _maxBossCount = 1;
    private int _currentBossCount = 0;
    
    public static event Action<int> OnWaveStarted;
    public static event Action<int> OnWaveEnded;
    public static event Action OnFireflyExplosion;
    public static event Action OnEnemyDamaged; 
    public static event Action OnBossAppear;
    public static event Action OnBossDeath;

    private void OnEnable()
    {
        Enemy.OnEnemyDeactivated += UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivated += StartExplodeEnemyOnDeath;
        LampAttackModel.OnLampAttack += LampAttack;
        LampAttackModel.OnLampBlockedAttack += LampBlockedAttack;
        Lamp.OnLampCollidedWithStickyEnemy += UpdateLadybugsOnScreen;
        BossBase.OnTriggerSpread += SpreadEnemies;
        BossBase.OnDeath += HandleBossEnd;
    }
    
    private void OnDisable()
    {
        Enemy.OnEnemyDeactivated -= UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivated -= StartExplodeEnemyOnDeath;
        LampAttackModel.OnLampAttack -= LampAttack;
        LampAttackModel.OnLampBlockedAttack -= LampBlockedAttack;
        Lamp.OnLampCollidedWithStickyEnemy -= UpdateLadybugsOnScreen;
        BossBase.OnTriggerSpread -= SpreadEnemies;
        BossBase.OnDeath -= HandleBossEnd;
    }
    
    public void Initialize()
    {
        _enemyPool.Initialize();
        _spawnQueueGenerator = new SpawnQueueGenerator(_spawnQueueDataCache.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        _enemies = new List<EnemyBase>();
        _ladybugsPatrolling = new List<EnemyBase>();
        _enemiesReadyToAttack = new List<EnemyBase>();
        _enemiesLampAttackHandler = new EnemiesLampAttackHandler();
        _enemiesExplosionHandler = new EnemiesExplosionHandler();
        
        _isBossActive = false;
        _currentBossCount = _maxBossCount;
        // Init all bosses
        _waspBoss.Initialize();
        _currentWave = _startAtWave;
        
        // Check all waves
        // for( int i=1; i<_spawnQueue.Count(); i++)
        // {
        //     var enemyQueue = _spawnQueue.Get(i);
        //     Debug.Log("--------------------------");
        //     Debug.Log("Wave: " + i.ToString());
        //     for (int j = 0; j < enemyQueue.Count(); j++)
        //     {
        //         Debug.Log("Enemy: " + enemyQueue.Get(j).ToString());   
        //     }
        // }
        
        _isWaveInitialized = false;
    }

    public void StartWave()
    {
        if (!_isWaveInitialized)
        {
            SetupWave(_currentWave);
            OnWaveStarted?.Invoke(_currentWave);
        }
    }
    
    private void SetupWave(int waveNum)
    {
        _currentSpawnEnemyIndex = 0;
        _enemyQueue = _spawnQueue.Get(waveNum);
        _enemiesInWave = _enemyQueue.Count();
        _enemiesAvailable = _enemiesInWave;
        _enemiesKilled = 0;
        _localTime = 0;
        _maxEnemiesOnScreen = _enemyQueue.MaxEnemiesOnScreen;
        _aggresionLevel = _enemyQueue.AggressionLevel;  
        _aggressionLevelNormalized = _aggresionLevel / _maxAggressionLevel;
        _spawnDelay = _enemyQueue.SpawnDelay;
        _spawnDelayAcceleration = _enemyQueue.SpawnDelayAcceleration;

        // Init Attack
        _attackDelay = GetRandomAttackDelay(4.5f, 1.8f, 6.5f, 2.8f, _aggressionLevelNormalized);
        
        _attackLocalTime = 0;
        _isWaveInitialized = true;
        
        // Debug
        _enemiesInWaveCount = _enemiesInWave;
        _enemiesLeftCount = _enemiesInWave;
    }

    private Enemy SpawnEnemy(EnemyTypes enemyType)
    {
        var enemy = _enemyPool.Get(enemyType);
        enemy.Initialize();
        return enemy;
    }
    
    private void SpawnBoss(EnemyTypes bossType)
    {
        if (bossType == EnemyTypes.Wasp)
        {
            _currentBoss = _waspBoss;
        }
        _currentBoss.Reset();
        _currentBoss.Play();
        _isBossActive = true;
        _attackLocalTime = 0;
        OnBossAppear?.Invoke();
    }
    
    private void LampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _enemiesLampAttackHandler.HandleLampAttack(attackPower, currentPower, attackDuration, attackDistance, _enemies);
    }
    
    private void LampBlockedAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _enemiesLampAttackHandler.HandleLampBlockedAttack(attackPower, currentPower, attackDuration, attackDistance, _enemies);
    }
    
    private void UpdateEnemiesOnScreen(EnemyBase enemy)
    {
        _enemies.Remove(enemy);
        _enemiesKilled++;
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _ladybugsPatrolling.Remove(enemy);
        }
    }
    
    private void UpdateLadybugsOnScreen(EnemyBase enemy)
    {
        // Remove stick ladybug for damageable list
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _ladybugsPatrolling.Remove(enemy);    
        }
    }

    private void StartExplodeEnemyOnDeath(EnemyBase explosionSource)
    {
        if(explosionSource.EnemyType != EnemyTypes.Firefly)
        {
            return;
        }
        _explosionSource = explosionSource;
        _explosionPosition = explosionSource.transform.position;
        _fireflyExplosion.Play(_explosionPosition, _fireflyExplosionRadius * 2);
        OnFireflyExplosion?.Invoke();
        _explosionLocalTime = 0;
        _isExplosionActive = true;
    }
    
    private void SpreadEnemies()
    {
        foreach (var enemy in _enemies)
        {
            enemy.SpreadStart();
        }
    }

    private void HandleBossEnd()
    {
        _isBossActive = false;
        _enemies.Remove(_currentBoss);
        _enemiesKilled++;
        OnBossDeath?.Invoke();
    }

    private void Update()
    {
        if (_isWaveInitialized)
        {
            if (_enemiesAvailable > 0)
            {
                SpawnEnemies();
            }

            UpdateEnemiesReadyToAttack(_enemiesReadyToAttack, _enemies);

            if (!_isAttacking)
            {
                DelayAttack();
            }

            if (_enemiesReadyToAttack.Count > 0)
            {
                if (_isAttacking == true)
                {
                    StartEnemyAttack();
                }
            }

            if (_isExplosionActive)
            {
                float explosionPhase = _explosionLocalTime / _explosionDuration;
                if (explosionPhase > 1)
                {
                    _isExplosionActive = false;
                }
                else
                {
                    _enemiesExplosionHandler.HandleExplosion(_enemies, _explosionSource, _explosionPosition, _fireflyExplosionRadius);
                }
            }
            
            if (_enemiesKilled == _enemiesInWave)
            {
                _isWaveInitialized = false;
                _currentWave++;
                OnWaveEnded?.Invoke(_currentWave);
                return;
            }
            
            // Check if Ladybugs are blocking other enemies from attacking
            bool isAttackTimerUpdateAllowed = CheckIfAttackTimeUpdateIsAllowed(_ladybugsPatrolling.Count, _ladybugsPatrolling);
            
            UpdateTimers(ref _localTime, ref _attackLocalTime, ref _explosionLocalTime, 
                isAttackTimerUpdateAllowed, _isBossActive);
        }
    }
    
    private void SpawnEnemies()
    {
        // TODO: Extract this part to a separate method and run it onece in the beginning of the wave
        // Check if it is the first enemy in the wave
        float localSpawnDelay = _spawnDelay;
        if(_currentSpawnEnemyIndex == 0)
        {
            localSpawnDelay = _firstEnemySpawnDelay;
            _attackLocalTime = 0;
        }
        
        // Apply spawnDelay Acceleration
        if(_currentSpawnEnemyIndex > 0)
        {
            float spawnDelayPhase = (float)(_currentSpawnEnemyIndex-1) / (_enemiesInWave-1);
            float spawnDelayAcceleration = 1f/Mathf.Lerp(1, _spawnDelayAcceleration, spawnDelayPhase);
            localSpawnDelay = _spawnDelay * spawnDelayAcceleration;
        }
        
        //-----------------------------------
        
        float phase = (_localTime) / localSpawnDelay;
        if (phase > 1)
        {
            if(_enemies.Count < _maxEnemiesOnScreen)
            {
                _localTime = 0;
                EnemyTypes enemyType = _enemyQueue.Get(_currentSpawnEnemyIndex);
                if (enemyType == EnemyTypes.Wasp)
                {
                    SpawnBoss(enemyType);
                    _enemies.Add(_currentBoss);
                }
                else
                {
                    var enemy = SpawnEnemy(enemyType);
                    if (enemy != null)
                    {
                        _enemies.Add(enemy);
                        if (enemy.EnemyType == EnemyTypes.Ladybug)
                        {
                            _ladybugsPatrolling.Add(enemy);
                        }
                    }
                }
                _enemiesAvailable--;
                _currentSpawnEnemyIndex++;
            }
            else
            {
                _localTime = 0;
            }
        }
    }
    
    private void UpdateEnemiesReadyToAttack(List<EnemyBase> enemiesReadyToAttack, List<EnemyBase> enemies)
    {
        enemiesReadyToAttack.Clear();
        foreach (var enemy in enemies)
        {
            enemy.UpdateAttackAvailability();
            if (enemy.ReadyToAttack)
            {
                enemiesReadyToAttack.Add(enemy);
            }
        }
    }
    
    private void DelayAttack()
    {
        float attackPhase = _attackLocalTime / _attackDelay;
        if (attackPhase > 1)
        {
            _isAttacking = true;
        }   
    }
    
    private void StartEnemyAttack()
    {
        var attackingEnemy = _enemiesReadyToAttack[Random.Range(0, _enemiesReadyToAttack.Count)];
        attackingEnemy.AttackStart();
        _attackLocalTime = 0;
        _attackDelay = GetRandomAttackDelay(2.5f, 0.8f, 6.1f, 1.8f, _aggressionLevelNormalized);
        _isAttacking = false;
    }
    
    private bool CheckIfAttackTimeUpdateIsAllowed(int ladybugPatrollingCount, List<EnemyBase> ladybugsPatrolling)
    {
        if (ladybugPatrollingCount > 0)
        {
            foreach (var ladybug in ladybugsPatrolling)
            {
                Vector3 pos = ladybug.transform.position;
                pos.z = 0;
                // if any Ladybug is at 0.87f distance or close - no enemy attack is allowed
                if (pos.magnitude < 0.87f || ladybug.IsAttacking)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    private void UpdateTimers(ref float localTime, ref float attackLocalTime, ref float explosionLocalTime, 
        bool isAttackUpdateAllowed, bool isBossActive)
    {
        localTime += Time.deltaTime;
        if(isAttackUpdateAllowed && !isBossActive)
        {
            attackLocalTime += Time.deltaTime;
        }
        explosionLocalTime += Time.deltaTime;
    }

    private float GetRandomAttackDelay(float minMin, float minMax, float maxMin, float maxMax, float aggressionLevel)
    {
        return Random.Range(
            Mathf.Lerp(minMin, minMax, aggressionLevel),
            Mathf.Lerp(maxMin, maxMax, aggressionLevel)
        );
    }
    
    // Event Handlers
    private void HandleOnEnemyDamaged()
    {
        OnEnemyDamaged?.Invoke();
    }
}
