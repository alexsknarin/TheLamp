using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = System.Object;

public class FEnemyPool : IEnemyDeactivatedProvider
{
    private ObjectPool<FEnemy> _mothlingPool;
    private ObjectPool<FEnemy> _flyPool;
    private ObjectPool<FEnemy> _fireFlyPool;
    private ObjectPool<FEnemy> _mothPool;
    private ObjectPool<FEnemy> _spiderPool;
    private ObjectPool<FEnemy> _ladybugPool;
    private ObjectPool<FEnemy> _megamothlingPool;
    private ObjectPool<FEnemy> _waspPool;
    private readonly FEnemyFactory _enemyFactory;
    private readonly int _poolSize = 5;
    private int _mothlingCount;
    private int _flyCount;
    private int _fireFlyCount;
    private int _mothCount;
    private int _spiderCount;
    private int _ladybugCount;
    private int _megamothlingCount;
    private int _waspCount;
    
    private readonly List<Type> _preloadedEnemyTypes = new List<Type>();
    
    public event Action<FEnemy> EnemyReleasedToPool;
    
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
        _mothPool = new ObjectPool<FEnemy>(
            CreateMoth,
            OnGetFromPool, 
            OnReleaseToPool, 
            OnDestroyPooledObject,
            true,
            _poolSize,
            _poolSize
        );
        _spiderPool = new ObjectPool<FEnemy>(
            CreateSpider,
            OnGetFromPool, 
            OnReleaseToPool, 
            OnDestroyPooledObject,
            true,
            _poolSize,
            _poolSize
        );
        _ladybugPool = new ObjectPool<FEnemy>(
            CreateLadybug,
            OnGetFromPool, 
            OnReleaseToPool, 
            OnDestroyPooledObject,
            true,
            _poolSize,
            _poolSize
        );
        _megamothlingPool = new ObjectPool<FEnemy>(
            CreateMegamothling,
            OnGetFromPool, 
            OnReleaseToPool, 
            OnDestroyPooledObject,
            true,
            _poolSize,
            _poolSize
        );
        
        _waspPool = new ObjectPool<FEnemy>(
            CreateWasp,
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
        if (type == typeof(FMoth))
        {
            if (_enemyFactory.IsMothLoaded)
            {
                return _mothPool.Get();
            }
            throw new Exception("Moth prefab is not loaded yet");
        }
        if (type == typeof(FSpider))
        {
            if (_enemyFactory.IsSpiderLoaded)
            {
                return _spiderPool.Get();
            }
            throw new Exception("Spider prefab is not loaded yet");
        }
        if (type == typeof(FLadybug))
        {
            if (_enemyFactory.IsLadybugLoaded)
            {
                return _ladybugPool.Get();
            }
            throw new Exception("Ladybug prefab is not loaded yet");
        }
        if (type == typeof(FMegamothling))
        {
            if (_enemyFactory.IsMegamothlingLoaded)
            {
                return _megamothlingPool.Get();
            }
            throw new Exception("Megamothling prefab is not loaded yet");
        }
        if (type == typeof(FWasp))
        {
            if (_enemyFactory.IsWaspLoaded)
            {
                return _waspPool.Get();
            }
            throw new Exception("Wasp prefab is not loaded yet");
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
        enemyInstance.name = "Fly" + _flyCount;
        _flyCount++;
        return enemyInstance;
    }
    
    private FEnemy CreateFireFly()
    {
        FEnemy enemyInstance = _enemyFactory.CreateEnemy(typeof(FFireFly));
        enemyInstance.SetObjectPool(_flyPool);
        enemyInstance.name = "FireFly" + _fireFlyCount;
        _flyCount++;
        return enemyInstance;
    }
    
    private FEnemy CreateMoth()
    {
        FEnemy enemyInstance = _enemyFactory.CreateEnemy(typeof(FMoth));
        enemyInstance.SetObjectPool(_mothPool);
        enemyInstance.name = "Moth" + _mothCount;
        _mothCount++;
        return enemyInstance;
    }
    
    private FEnemy CreateSpider()
    {
        FEnemy enemyInstance = _enemyFactory.CreateEnemy(typeof(FSpider));
        enemyInstance.SetObjectPool(_spiderPool);
        enemyInstance.name = "Spider" + _spiderCount;
        _spiderCount++;
        return enemyInstance;
    }
    
    private FEnemy CreateLadybug()
    {
        FEnemy enemyInstance = _enemyFactory.CreateEnemy(typeof(FLadybug));
        enemyInstance.SetObjectPool(_ladybugPool);
        enemyInstance.name = "Ladybug" + _ladybugCount;
        _ladybugCount++;
        return enemyInstance;
    }
    
    private FEnemy CreateMegamothling()
    {
        FEnemy enemyInstance = _enemyFactory.CreateEnemy(typeof(FMegamothling));
        enemyInstance.SetObjectPool(_megamothlingPool);
        enemyInstance.name = "Megamothling" + _megamothlingCount;
        _megamothlingCount++;
        return enemyInstance;
    }
    
    private FEnemy CreateWasp()
    {
        FEnemy enemyInstance = _enemyFactory.CreateEnemy(typeof(FWasp));
        enemyInstance.SetObjectPool(_waspPool);
        enemyInstance.name = "Wasp" + _waspCount;
        _waspCount++;
        return enemyInstance;
    }
    
    private void OnGetFromPool(FEnemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }
    
    private void OnReleaseToPool(FEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
        EnemyReleasedToPool?.Invoke(enemy);
    }
    
    private void OnDestroyPooledObject(FEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }
}
