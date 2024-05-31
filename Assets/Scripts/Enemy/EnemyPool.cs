using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour, IInitializable
{
    [SerializeField] private Enemy _mothlingPrefab;
    [SerializeField] private Enemy _flyPrefab;
    [SerializeField] private Enemy _mothPrefab;
    [SerializeField] private Enemy _ladybugPrefab;
    [SerializeField] private Enemy _fireflyPrefab;
    [SerializeField] private Enemy _spiderPrefab;
    private ObjectPool<Enemy> _mothlingPool;
    private ObjectPool<Enemy> _flyPool;
    private ObjectPool<Enemy> _mothPool;
    private ObjectPool<Enemy> _ladybugPool;
    private ObjectPool<Enemy> _fireflyPool;
    private ObjectPool<Enemy> _spiderPool;
    [SerializeField] private int _poolSize;
    private int _mothlingCount;
    private int _flyCount;
    private int _mothCount;
    private int _ladybugCount;
    private int _fireflyCount;
    private int _spiderCount;


    public void Initialize()
    {
        _flyCount = 0;
        _mothCount= 0;
        _mothlingCount= 0;
        _ladybugCount = 0;
        _fireflyCount = 0;
        _spiderCount = 0;
        _mothlingPool = new ObjectPool<Enemy>(CreateMothling, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
        _flyPool = new ObjectPool<Enemy>(CreateFly, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
        _mothPool = new ObjectPool<Enemy>(CreateMoth, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
        _ladybugPool = new ObjectPool<Enemy>(CreateLadybug, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
        _fireflyPool = new ObjectPool<Enemy>(CreateFirefly, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
        _spiderPool = new ObjectPool<Enemy>(CreateSpider, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
    }
    
    private Enemy CreateMothling()
    {
        Enemy enemyInstance = Instantiate(_mothlingPrefab);
        enemyInstance.ObjectPool = _mothlingPool;
        enemyInstance.name = "Mothling" + _mothlingCount;
        _mothlingCount++;
        return enemyInstance;
    }
    
    private Enemy CreateFly()
    {
        Enemy enemyInstance = Instantiate(_flyPrefab);
        enemyInstance.ObjectPool = _flyPool;
        enemyInstance.name = "Fly" + _flyCount;
        _flyCount++;
        return enemyInstance;
    }
    
    private Enemy CreateMoth()
    {
        Enemy enemyInstance = Instantiate(_mothPrefab);
        enemyInstance.ObjectPool = _mothPool;
        enemyInstance.name = "Moth" + _mothCount;
        _mothCount++;
        return enemyInstance;
    }
    
    private Enemy CreateLadybug()
    {
        Enemy enemyInstance = Instantiate(_ladybugPrefab);
        enemyInstance.ObjectPool = _ladybugPool;
        enemyInstance.name = "Ladybug" + _ladybugCount;
        _ladybugCount++;
        return enemyInstance;
    }
    
    private Enemy CreateFirefly()
    {
        Enemy enemyInstance = Instantiate(_fireflyPrefab);
        enemyInstance.ObjectPool = _fireflyPool;
        enemyInstance.name = "Firefly" + _fireflyCount;
        _fireflyCount++;
        return enemyInstance;
    }
    
    private Enemy CreateSpider()
    {
        Enemy enemyInstance = Instantiate(_spiderPrefab);
        enemyInstance.ObjectPool = _spiderPool;
        enemyInstance.name = "Spider" + _spiderCount;
        _spiderCount++;
        return enemyInstance;
    }

    private void OnGetFromPool(Enemy pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }
    
    private void OnReleaseToPool(Enemy pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }
    
    private void OnDestroyPooledObject(Enemy pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }
    
    public Enemy Get(EnemyTypes enemyType)
    {
        switch (enemyType)
        {
            case EnemyTypes.Mothling:
                if (_mothlingPool.CountAll <= _poolSize)
                {
                    return _mothlingPool.Get();
                }
                break;
            case EnemyTypes.Fly:
                if (_flyPool.CountAll <= _poolSize)
                {
                    return _flyPool.Get();
                }
                break;
            case EnemyTypes.Moth:
                if (_mothPool.CountAll <= _poolSize)
                {
                    return _mothPool.Get();
                }
                break;
            case EnemyTypes.Ladybug:
                if (_ladybugPool.CountAll <= _poolSize)
                {
                    return _ladybugPool.Get();
                }
                break;
            case EnemyTypes.Firefly:
                if (_fireflyPool.CountAll <= _poolSize)
                {
                    return _fireflyPool.Get();
                }
                break;
            case EnemyTypes.Spider:
                if (_spiderPool.CountAll <= _poolSize)
                {
                    return _spiderPool.Get();
                }
                break;
        }
        return null;
    }
}

