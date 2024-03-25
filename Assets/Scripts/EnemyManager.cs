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
    [SerializeField] private Wasp _waspBoss;
    
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
    [SerializeField] private int _currentWave = 1;
    private int _enemiesInWave;
    private int _enemiesAvailable;
    private int _enemiesKilled;
    private int _currentSpawnEnemyIndex;
    [SerializeField] private float _spawnDelay;
    
    [Header("---- Debug ------")]
    [SerializeField] private int _enemiesInWaveCount;
    [SerializeField] private int _enemiesLeftCount;
    
    private List<EnemyBase> _enemies;
    private List<EnemyBase> _enemiesReadyToAttack;
    private List<EnemyBase> _ladybugsPatroling;
    private bool _isWaveInitialized = false;
    
    private float _attackDelay;
    private float _attackLocalTime;
    private bool _isAttacking;
    
    private float _localTime;
    private float _localExplosionTime;
    
    private bool _isBossActive = false;
    private int _maxBossCount = 1;
    private int _currentBossCount = 0;
    
    public static event Action<int> OnWavePrepared;
    public static event Action OnWaveStarted;
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
        PlayerInputHandler.OnPlayerAttack += StartWave;
        _waspBoss.OnTriggerSpread += SpreadEnemies;
        _waspBoss.OnDeath += HandleBossEnd;
    }
    
    private void OnDisable()
    {
        Enemy.OnEnemyDeactivated -= UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivated -= StartExplodeEnemyOnDeath;
        LampAttackModel.OnLampAttack -= LampAttack;
        LampAttackModel.OnLampBlockedAttack -= LampBlockedAttack;
        Lamp.OnLampCollidedWithStickyEnemy -= UpdateLadybugsOnScreen;
        PlayerInputHandler.OnPlayerAttack -= StartWave;
        _waspBoss.OnTriggerSpread -= SpreadEnemies;
        _waspBoss.OnDeath -= HandleBossEnd;
    }
    
    public void Initialize()
    {
        _spawnQueueGenerator = new SpawnQueueGenerator(_spawnQueueDataCache.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        _enemies = new List<EnemyBase>();
        _ladybugsPatroling = new List<EnemyBase>();
        _enemiesReadyToAttack = new List<EnemyBase>();
        _isBossActive = false;
        _currentBossCount = _maxBossCount;
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
    }
    
    private void StartWave()
    {
        if (!_isWaveInitialized)
        {
            OnWaveStarted?.Invoke();
            SetupWave(_currentWave);
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

        // Init Attack
        _attackDelay = Random.Range(
            Mathf.Lerp(4.5f, 1.8f, _aggressionLevelNormalized),
            Mathf.Lerp(6.5f, 2.8f, _aggressionLevelNormalized)
        );
        
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
            _waspBoss.Reset();
            _waspBoss.Play();
            _isBossActive = true;
            _attackLocalTime = 0;
            OnBossAppear?.Invoke();    
        }
    }
    
    private void LampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.gameObject.activeInHierarchy && enemy.ReadyToLampDamage)
            {
                if (attackPower > 0)
                {
                    enemy.ReceiveDamage(attackPower);
                    OnEnemyDamaged?.Invoke();    
                }
            }
        }
    }
    
    private void LampBlockedAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.gameObject.activeInHierarchy && enemy.ReadyToLampDamage && enemy.IsStick && enemy.EnemyType == EnemyTypes.Ladybug)
            {
                if (attackPower > 0)
                {
                    enemy.ReceiveDamage(attackPower);
                    OnEnemyDamaged?.Invoke();   
                }
            }
        }
    }
    
    private void UpdateEnemiesOnScreen(EnemyBase enemy)
    {
        _enemies.Remove(enemy);
        _enemiesKilled++;
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _ladybugsPatroling.Remove(enemy);
        }
    }
    
    private void UpdateLadybugsOnScreen(EnemyBase enemy)
    {
        // Remove stick ladybug for damageable list
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _ladybugsPatroling.Remove(enemy);    
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
        _localExplosionTime = 0;
        _isExplosionActive = true;
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
            enemyPosition2d.z = 0;
            Vector3 explosionPosition2d = _explosionPosition;
            explosionPosition2d.z = 0;
            if((explosionPosition2d - enemyPosition2d).magnitude < _fireflyExplosionRadius)
            {
                enemy.ReceiveDamage(100);
            }
        }
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
        _enemies.Remove(_waspBoss);
        _enemiesKilled++;
        OnBossDeath?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpreadEnemies();
        }
        
        if (_isWaveInitialized)
        {
            // Spawn
            if (_enemiesAvailable > 0)
            {
                float phase = (_localTime) / _spawnDelay;
                if (phase > 1)
                {
                    if(_enemies.Count < _maxEnemiesOnScreen)
                    {
                        _localTime = 0;
                        EnemyTypes enemyType = _enemyQueue.Get(_currentSpawnEnemyIndex);
                        if (enemyType == EnemyTypes.Wasp)
                        {
                            SpawnBoss(enemyType);
                            _enemies.Add(_waspBoss);
                        }
                        else
                        {
                            var enemy = SpawnEnemy(enemyType);
                            if (enemy != null)
                            {
                                _enemies.Add(enemy);
                                if (enemy.EnemyType == EnemyTypes.Ladybug)
                                {
                                    _ladybugsPatroling.Add(enemy);
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
            
            // Check ReadyToAttack
            _enemiesReadyToAttack.Clear();
            foreach (var enemy in _enemies)
            {
                enemy.UpdateAttackAvailability();
                if (enemy.ReadyToAttack)
                {
                    _enemiesReadyToAttack.Add(enemy);
                }
            }
            
            // Attack delay
            if (!_isAttacking)
            {
                float attackPhase = _attackLocalTime / _attackDelay;
                if (attackPhase > 1)
                {
                    _isAttacking = true;
                }   
            }
            
            if (_enemiesReadyToAttack.Count > 0)
            {
                if (_isAttacking == true)
                {
                    var attackingEnemy = _enemiesReadyToAttack[Random.Range(0, _enemiesReadyToAttack.Count)];
                    attackingEnemy.AttackStart();
                    _attackLocalTime = 0;
                    _attackDelay = Random.Range(
                        Mathf.Lerp(2.5f, 0.8f, _aggressionLevelNormalized),
                        Mathf.Lerp(6.1f, 1.8f, _aggressionLevelNormalized)
                        );
                    _isAttacking = false;
                }
            }

            if (_isExplosionActive)
            {
                float explosionPhase = _localExplosionTime / _explosionDuration;
                if (explosionPhase > 1)
                {
                    _isExplosionActive = false;
                }
                else
                {
                    PerformExplosion();
                }
            }

            if (_enemiesKilled == _enemiesInWave)
            {
                _isWaveInitialized = false;
                _currentWave++;
                OnWavePrepared?.Invoke(_currentWave);
            }

            // Check Ladybugs
            bool isAttackTimerUpdateAllowed = true;
            if (_ladybugsPatroling.Count > 0)
            {
                foreach (var ladybug in _ladybugsPatroling)
                {
                    Vector3 pos = ladybug.transform.position;
                    pos.z = 0;
                    if (pos.magnitude < 0.87f || ladybug.IsAttacking)
                    {
                        isAttackTimerUpdateAllowed = false;
                        break;
                    }
                }
            }
            
            _localTime += Time.deltaTime;
            if(isAttackTimerUpdateAllowed && !_isBossActive)
            {
                _attackLocalTime += Time.deltaTime;
            }
            _localExplosionTime += Time.deltaTime;
        }
        
        _enemiesLeftCount = _enemies.Count;
    }
}
