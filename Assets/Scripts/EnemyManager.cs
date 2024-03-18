using System;
using System.Collections.Generic;
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

    [Header("------ Explosions -------")]
    [SerializeField] private FireflyExplosion _fireflyExplosion;
    [SerializeField] private float _fireflyExplosionRadius;
    [SerializeField] private float _explosionDuration;
    private Enemy _explosionSource;
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
    
    private List<Enemy> _enemies;
    private List<Enemy> _enemiesReadyToAttack;
    private List<Enemy> _ladybugs;
    private bool _isWaveInitialized = false;
    
    private float _attackDelay;
    private float _attackLocalTime;
    private bool _isAttacking;
    
    private float _localTime;
    private float _localExplosionTime;
    
    public static event Action<int> OnWavePrepared;
    public static event Action OnWaveStarted;
    public static event Action OnFireflyExplosion;
    public static event Action OnEnemyDamaged; 

    private void OnEnable()
    {
        Enemy.OnEnemyDeactivated += UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivated += StartExplodeEnemyOnDeath;
        LampAttackModel.OnLampAttack += LampAttack;
        LampAttackModel.OnLampBlockedAttack += LampBlockedAttack;
        Lamp.OnLampCollidedWithStickyEnemy += UpdateLadybugsOnScreen;
        PlayerInputHandler.OnPlayerAttack += StartWave;
    }
    
    private void OnDisable()
    {
        Enemy.OnEnemyDeactivated -= UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivated -= StartExplodeEnemyOnDeath;
        LampAttackModel.OnLampAttack -= LampAttack;
        LampAttackModel.OnLampBlockedAttack -= LampBlockedAttack;
        Lamp.OnLampCollidedWithStickyEnemy -= UpdateLadybugsOnScreen;
        PlayerInputHandler.OnPlayerAttack -= StartWave;
    }
    
    public void Initialize()
    {
        _spawnQueueGenerator = new SpawnQueueGenerator(_spawnQueueDataCache.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        _enemies = new List<Enemy>();
        _ladybugs = new List<Enemy>();
        _enemiesReadyToAttack = new List<Enemy>();
        _currentWave = _startAtWave;
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

        // Attack
        _attackDelay = _spawnDelay + 2f;
        _attackLocalTime = 0;
        _isWaveInitialized = true;
        
        // Debug
        _enemiesInWaveCount = _enemiesInWave;
        _enemiesLeftCount = _enemiesInWave;
    }
    
    public Enemy SpawnEnemy(int enemyIndex)
    {
        EnemyTypes enemyType = _enemyQueue.Get(enemyIndex);
        var enemy = _enemyPool.Get(enemyType);
        enemy.Initialize();
        return enemy;
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
    
    private void UpdateEnemiesOnScreen(Enemy enemy)
    {
        _enemies.Remove(enemy);
        _enemiesKilled++;
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _ladybugs.Remove(enemy);
        }
    }
    
    private void UpdateLadybugsOnScreen(Enemy enemy)
    {
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _ladybugs.Remove(enemy);    
        }
    }

    private void StartExplodeEnemyOnDeath(Enemy explosionSource)
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
                        var enemy = SpawnEnemy(_currentSpawnEnemyIndex);
                        if (enemy != null)
                        {
                            _enemies.Add(enemy);
                            if (enemy.EnemyType == EnemyTypes.Ladybug)
                            {
                                _ladybugs.Add(enemy);
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
                        Mathf.Lerp(1.5f, 0.8f, _aggressionLevelNormalized),
                        Mathf.Lerp(4.1f, 1.8f, _aggressionLevelNormalized)
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
                OnWavePrepared?.Invoke(_currentWave+1);
            }

            // Check Ladybugs
            bool isAttackTimerUpdateAllowed = true;
            if (_ladybugs.Count > 0)
            {
                foreach (var ladybug in _ladybugs)
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
            if(isAttackTimerUpdateAllowed)
            {
                _attackLocalTime += Time.deltaTime;
            }
            _localExplosionTime += Time.deltaTime;
        }
    }
}
