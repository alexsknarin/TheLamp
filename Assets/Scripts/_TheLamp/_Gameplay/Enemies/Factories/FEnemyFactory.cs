using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class FEnemyFactory
{
    private FMothling _mothlingEnemyPrefab;
    private MothlingMovementStateFactory _mothlingMovementStateFactory;
    private FlyMovementStateFactory _flyMovementStateFactory;
    private MothMovementStateFactory _mothMovementStateFactory;
    private SpiderMovementStateFactory _spiderMovementStateFactory;
    
    AsyncOperationHandle<GameObject> _mothlingEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _flyEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _fireFlyEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _mothEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _spiderEnemyAssetHandle;
    
    
    public FEnemyFactory(
        MothlingMovementStateFactory mothlingMovementStateFactory,
        FlyMovementStateFactory flyMovementStateFactory,
        MothMovementStateFactory mothMovementStateFactory,
        SpiderMovementStateFactory spiderMovementStateFactory
    )
    {
        _mothlingMovementStateFactory = mothlingMovementStateFactory;
        _flyMovementStateFactory = flyMovementStateFactory;
        _mothMovementStateFactory = mothMovementStateFactory;
        _spiderMovementStateFactory = spiderMovementStateFactory;
        
        IsMothlingLoaded = false;
        IsFlyLoaded = false;
        IsFireFlyLoaded = false;
        IsMothLoaded = false;
        IsSpiderLoaded = false;
    }
    
    public bool IsMothlingLoaded { get; private set; }
    public bool IsFlyLoaded { get; private set; }
    public bool IsFireFlyLoaded { get; private set; }
    public bool IsMothLoaded { get; private set; }
    public bool IsSpiderLoaded { get; private set; }

    public async void LoadEnemy(Type type)
    {
        if (type == typeof(FMothling))
        {
            _mothlingEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/FMothling.prefab");
            await _mothlingEnemyAssetHandle.Task;
            IsMothlingLoaded = true;
            Debug.Log("Mothling Loaded");
        }
        if (type == typeof(FFly))
        {
            _flyEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/FFly.prefab");
            await _flyEnemyAssetHandle.Task;
            IsFlyLoaded = true;
            Debug.Log("Fly Loaded");
        }

        if (type == typeof(FFireFly))
        {
            _fireFlyEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/FFireFly.prefab");
            await _fireFlyEnemyAssetHandle.Task;
            IsFireFlyLoaded = true;
            Debug.Log("FireFly Loaded");
        }
        
        if (type == typeof(FMoth))
        {
            _mothEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/FMoth.prefab");
            await _mothEnemyAssetHandle.Task;
            IsMothLoaded = true;
            Debug.Log("Moth Loaded");
        }
        
        if (type == typeof(FSpider))
        {
            _spiderEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/FSpider.prefab");
            await _spiderEnemyAssetHandle.Task;
            IsSpiderLoaded = true;
            Debug.Log("Spider Loaded");
        }
        
    }
    
    public FEnemy CreateEnemy(Type type)
    {
        if (type == typeof(FMothling) && _mothlingEnemyAssetHandle.IsValid())
        {
            var prefab = _mothlingEnemyAssetHandle.Result;
            return CreateMothlingInstance(prefab);    
        }
        if (type == typeof(FFly) && _flyEnemyAssetHandle.IsValid())
        {
            var prefab = _flyEnemyAssetHandle.Result;
            return CreateFlyInstance(prefab);    
        }
        if (type == typeof(FFireFly) && _fireFlyEnemyAssetHandle.IsValid())
        {
            var prefab = _fireFlyEnemyAssetHandle.Result;
            return CreateFireFlyInstance(prefab);    
        }
        if (type == typeof(FMoth) && _mothEnemyAssetHandle.IsValid())
        {
            var prefab = _mothEnemyAssetHandle.Result;
            return CreateMothInstance(prefab);    
        }
        if (type == typeof(FSpider) && _spiderEnemyAssetHandle.IsValid())
        {
            var prefab = _spiderEnemyAssetHandle.Result;
            return CreateSpiderInstance(prefab);    
        }
        else
        {
            throw new Exception("Enemy Factory: Enemy type not loaded");
        }
    }
    
    private FEnemy CreateMothlingInstance(GameObject prefab)
    {
        GameObject enemyInstance = Object.Instantiate(prefab);
        enemyInstance.GetComponent<FMothlingMovement>().Construct(_mothlingMovementStateFactory);
        enemyInstance.GetComponent<FMothlingPresentation>().Initialize();
        var enemy = enemyInstance.GetComponent<FMothling>();
        enemy.Initialize();
        
        return enemy;
    } 
    
    private FEnemy CreateFlyInstance(GameObject prefab)
    {
        GameObject enemyInstance = Object.Instantiate(prefab);
        enemyInstance.GetComponent<FFlyMovement>().Construct(_flyMovementStateFactory);
        enemyInstance.GetComponent<FFlyPresentation>().Initialize();
        var enemy = enemyInstance.GetComponent<FFly>();
        enemy.Initialize();
        
        return enemy;
    }
    
    private FEnemy CreateFireFlyInstance(GameObject prefab)
    {
        GameObject enemyInstance = Object.Instantiate(prefab);
        enemyInstance.GetComponent<FFlyMovement>().Construct(_flyMovementStateFactory);
        enemyInstance.GetComponent<FFireFlyPresentation>().Initialize();
        var enemy = enemyInstance.GetComponent<FFireFly>();
        enemy.Initialize();
        
        return enemy;
    }
    
    private FEnemy CreateMothInstance(GameObject prefab)
    {
        GameObject enemyInstance = Object.Instantiate(prefab);
        enemyInstance.GetComponent<FMothMovement>().Construct(_mothMovementStateFactory);
        enemyInstance.GetComponent<FMothPresentation>().Initialize();
        var enemy = enemyInstance.GetComponent<FMoth>();
        enemy.Initialize();
        
        return enemy;
    }
    
    private FEnemy CreateSpiderInstance(GameObject prefab)
    {
        GameObject enemyInstance = Object.Instantiate(prefab);
        enemyInstance.GetComponent<FSpiderMovement>().Construct(_spiderMovementStateFactory);
        enemyInstance.GetComponent<FSpiderPresentation>().Initialize();
        var enemy = enemyInstance.GetComponent<FSpider>();
        enemy.Initialize();
        
        return enemy;
    }
    
}
