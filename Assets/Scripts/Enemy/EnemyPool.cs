using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour, IInitializable
{
    [SerializeField] private Enemy _flyPrefab;
    [SerializeField] private Enemy _mothPrefab;
    [SerializeField] private Enemy _ladybugPrefab;
    [SerializeField] private Enemy _fireflyPrefab;
    private ObjectPool<Enemy> _flyPool;
    private ObjectPool<Enemy> _mothPool;
    private ObjectPool<Enemy> _ladybugPool;
    private ObjectPool<Enemy> _fireflyPool;
    [SerializeField] private int _poolSize;


    public void Initialize()
    {
        _flyPool = new ObjectPool<Enemy>(CreateFly, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
        _mothPool = new ObjectPool<Enemy>(CreateMoth, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
        _ladybugPool = new ObjectPool<Enemy>(CreateLadybug, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
        _fireflyPool = new ObjectPool<Enemy>(CreateFirefly, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolSize, _poolSize);
    }
    
    private Enemy CreateFly()
    {
        Enemy enemyInstance = Instantiate(_flyPrefab);
        enemyInstance.ObjectPool = _flyPool;
        return enemyInstance;
    }
    
    private Enemy CreateMoth()
    {
        Enemy enemyInstance = Instantiate(_mothPrefab);
        enemyInstance.ObjectPool = _mothPool;
        return enemyInstance;
    }
    
    private Enemy CreateLadybug()
    {
        Enemy enemyInstance = Instantiate(_ladybugPrefab);
        enemyInstance.ObjectPool = _ladybugPool;
        return enemyInstance;
    }
    
    private Enemy CreateFirefly()
    {
        Enemy enemyInstance = Instantiate(_fireflyPrefab);
        enemyInstance.ObjectPool = _fireflyPool;
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
        }
        return null;
    }
}

