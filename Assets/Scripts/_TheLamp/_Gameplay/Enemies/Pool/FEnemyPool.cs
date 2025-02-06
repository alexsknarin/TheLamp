using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FEnemyPool
{
    private ObjectPool<FEnemy> _mothlingPool;
    private ObjectPool<FEnemy> _flyPool;
    private ObjectPool<FEnemy> _fireFlyPool;
    private FEnemyFactory _enemyFactory; // TODO: enemy factory per Enemy Type???
    private int _poolSize = 5;
    private int _mothlingCount;
    private int _flyCount;
    private int _fireFlyCount;
    private List<Type> _preloadedEnemyTypes = new List<Type>();
    
    public event Action<FEnemy> EnemyReleased;
    
    public FEnemyPool(FEnemyFactory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    public void Initialize()
    {
        _mothlingCount= 0;
        _mothlingPool = new ObjectPool<FEnemy>(
            CreateMothling, 
            OnGetFromPool, 
            OnReleaseToPool, 
            OnDestroyPooledObject,
            true,
            _poolSize,
            _poolSize
            );
        _flyPool = new ObjectPool<FEnemy>(
            CreateFly, 
            OnGetFromPool, 
            OnReleaseToPool, 
            OnDestroyPooledObject,
            true,
            _poolSize,
            _poolSize
            );
        _fireFlyPool = new ObjectPool<FEnemy>(
            CreateFireFly, 
            OnGetFromPool, 
            OnReleaseToPool, 
            OnDestroyPooledObject,
            true,
            _poolSize,
            _poolSize
            );
    }

    public void PreloadEnemy(Type type)
    {
        if (!_preloadedEnemyTypes.Contains(type))
        {
            _preloadedEnemyTypes.Add(type);
            _enemyFactory.LoadEnemy(type);
        }
    }

    public FEnemy Get(Type type)
    {
        if (type == typeof(FMothling))
        {
            if (_enemyFactory.IsMothlingLoaded)
            {
                return _mothlingPool.Get();
            }
            throw new Exception("Mothling prefab is not loaded yet");
        }
        if (type == typeof(FFly))
        {
            if (_enemyFactory.IsFlyLoaded)
            {
                return _flyPool.Get();
            }
            throw new Exception("Fly prefab is not loaded yet");
        }
        if (type == typeof(FFireFly))
        {
            if (_enemyFactory.IsFireFlyLoaded)
            {
                return _fireFlyPool.Get();
            }
            throw new Exception("FireFly prefab is not loaded yet");
        }
        
        else
        {
            throw new Exception("Enemy type not supported");
        }
    }

    
    private FEnemy CreateMothling()
    {
        FEnemy enemyInstance = _enemyFactory.CreateEnemy(typeof(FMothling));
        enemyInstance.SetObjectPool(_mothlingPool);
        enemyInstance.name = "Mothling" + _mothlingCount;
        _mothlingCount++;
        return enemyInstance;
    }
    
    private FEnemy CreateFly()
    {
        FEnemy enemyInstance = _enemyFactory.CreateEnemy(typeof(FFly));
        enemyInstance.SetObjectPool(_flyPool);
        enemyInstance.name = "Fly" + _mothlingCount;
        _flyCount++;
        return enemyInstance;
    }
    
    private FEnemy CreateFireFly()
    {
        FEnemy enemyInstance = _enemyFactory.CreateEnemy(typeof(FFireFly));
        enemyInstance.SetObjectPool(_flyPool);
        enemyInstance.name = "FireFly" + _mothlingCount;
        _flyCount++;
        return enemyInstance;
    }
    
    private void OnGetFromPool(FEnemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }
    
    private void OnReleaseToPool(FEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
        EnemyReleased?.Invoke(enemy);
    }
    
    private void OnDestroyPooledObject(FEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }
}
