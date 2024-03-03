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
    [SerializeField] private GameObject _flyEnemyPrefab;
    [SerializeField] private GameObject _mothEnemyPrefab;
    [SerializeField] private GameObject _ladybugEnemyPrefab;
    [SerializeField] private GameObject _fireflyEnemyPrefab;
    [Header("---- Waves Generation ------")]
    [SerializeField] private int _enemiesOnScreen;
    [Header("")]
    [SerializeField] private int _currentWave = 1;
    private int _enemiesInWave;
    private int _enemiesAvailable;
    private int _enemiesKilled;
    private int _currentSpawnEnemyIndex;
    [SerializeField] private float _spawnDelay;
    
    private List<Enemy> _enemies;
    private List<Enemy> _enemiesReadyToAttack;
    private bool _isWaveInitialized = false;
    
    private float _attackDelay;
    private float _attackPrevTime;
    
    private float _prevTime;
    
    public static event Action<int> OnWavePrepared;
    public static event Action OnWaveStarted;
    
    private void OnEnable()
    {
        Enemy.OnEnemyDeath += UpdateEnemiesOnScreen;
        LampAttackModel.OnLampAttack += LampAttack;
        PlayerInputHandler.OnPlayerAttack += StartWave;
    }
    
    private void OnDisable()
    {
        Enemy.OnEnemyDeath -= UpdateEnemiesOnScreen;
        LampAttackModel.OnLampAttack -= LampAttack;
        PlayerInputHandler.OnPlayerAttack -= StartWave;
    }
    
    public void Initialize()
    {
        _spawnQueueGenerator = new SpawnQueueGenerator(_spawnQueueDataCache.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        _enemies = new List<Enemy>();
        _enemiesReadyToAttack = new List<Enemy>();
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
        _prevTime = Time.time;
        
        // Attack
        _attackDelay = _spawnDelay + 2f;
        _attackPrevTime = Time.time;
        // TODO: reading from the spawn queue object
        
        _isWaveInitialized = true;
    }
    
    public void SpawnEnemy(int enemyIndex)
    {
        EnemyTypes enemyType = _enemyQueue.Get(enemyIndex);
        GameObject prefab = null;

        switch (enemyType)
        {
            case EnemyTypes.Fly:
                prefab = _flyEnemyPrefab;
                break;
            case EnemyTypes.Moth:
                prefab = _mothEnemyPrefab;
                break;
            case EnemyTypes.Ladybug:
                prefab = _ladybugEnemyPrefab;
                break;
            case EnemyTypes.Firefly:
                prefab = _fireflyEnemyPrefab;
                break;
        }
        var enemyObject = Instantiate(prefab, transform.position, Quaternion.identity);
        var enemy = enemyObject.gameObject.GetComponent<Enemy>();
        enemy.Initialize();
        _enemies.Add(enemy);        
    }

    private void LampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
 
    }
    
    private void UpdateEnemiesOnScreen(Enemy enemy)
    {
        _enemies.Remove(enemy);
        _enemiesKilled++;
    }

    private void ExplodeEnemyOnDeath()
    {
        
    }

    private void Update()
    {
        if (_isWaveInitialized)
        {
            // Spawn
            if (_enemiesAvailable > 0)
            {
                float phase = (Time.time - _prevTime) / _spawnDelay;
                if (phase > 1)
                {
                    if(_enemies.Count < _enemiesOnScreen)
                    {
                        _prevTime = Time.time;
                        Debug.Log(_enemyQueue.Get(_currentSpawnEnemyIndex));
                        SpawnEnemy(_currentSpawnEnemyIndex);
                        _enemiesAvailable--;
                        _currentSpawnEnemyIndex++;
                    }
                    else
                    {
                        _prevTime = Time.time;
                    }
                }    
            }
            
            // Check ReadyToAttack
            _enemiesReadyToAttack.Clear();
            foreach (var enemy in _enemies)
            {
                if (enemy.ReadyToAttack)
                {
                    _enemiesReadyToAttack.Add(enemy);
                }
            }
            
            if(_enemiesReadyToAttack.Count > 0)
            {
                float attackPhase = (Time.time - _attackPrevTime) / _attackDelay;
                if (attackPhase > 1)
                {
                    var attackingEnemy = _enemiesReadyToAttack[Random.Range(0, _enemiesReadyToAttack.Count)];
                    attackingEnemy.Attack();
                    _attackPrevTime = Time.time;
                    _attackDelay = Random.Range(2f, 5f);
                }
            }

            if (_enemiesKilled == _enemiesInWave)
            {
                Debug.Log("Wave finished");
                _isWaveInitialized = false;
                _currentWave++;
                OnWavePrepared?.Invoke(_currentWave+1);
            }
        }
    }


}
