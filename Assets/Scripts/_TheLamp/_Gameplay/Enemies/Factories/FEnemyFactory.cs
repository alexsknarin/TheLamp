using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class FEnemyFactory
{
    private ILampPositionProviderService _lampPositionProviderService;
    private MothlingMovementStateFactory _mothlingMovementStateFactory;
    private FlyMovementStateFactory _flyMovementStateFactory;
    private MothMovementStateFactory _mothMovementStateFactory;
    private SpiderMovementStateFactory _spiderMovementStateFactory;
    private LadybugMovementStateFactory _ladybugMovementStateFactory;
    private MegamothlingMovementStateFactory _megamothlingMovementStateFactory;
    
    AsyncOperationHandle<GameObject> _mothlingEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _flyEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _fireFlyEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _mothEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _spiderEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _ladybugEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _megamothlingEnemyAssetHandle;
    
    
    public FEnemyFactory(
        MothlingMovementStateFactory mothlingMovementStateFactory,
        FlyMovementStateFactory flyMovementStateFactory,
        MothMovementStateFactory mothMovementStateFactory,
        SpiderMovementStateFactory spiderMovementStateFactory,
        LadybugMovementStateFactory ladybugMovementStateFactory,
        MegamothlingMovementStateFactory megamothlingMovementStateFactory,
        ILampPositionProviderService lampPositionProviderService
    )
    {
        _mothlingMovementStateFactory = mothlingMovementStateFactory;
        _flyMovementStateFactory = flyMovementStateFactory;
        _mothMovementStateFactory = mothMovementStateFactory;
        _spiderMovementStateFactory = spiderMovementStateFactory;
        _ladybugMovementStateFactory = ladybugMovementStateFactory;
        _megamothlingMovementStateFactory = megamothlingMovementStateFactory;
        _lampPositionProviderService = lampPositionProviderService;
        
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
    public bool IsLadybugLoaded { get; private set; }
    public bool IsMegamothlingLoaded { get; private set; }
    

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
        
        if (type == typeof(FLadybug))
        {
            _ladybugEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/FLadybug.prefab");
            await _ladybugEnemyAssetHandle.Task;
            IsLadybugLoaded = true;
            Debug.Log("Ladybug Loaded");
        }
        
        if (type == typeof(FMegamothling))
        {
            _megamothlingEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Boss/FMegamothling.prefab");
            await _megamothlingEnemyAssetHandle.Task;
            IsMegamothlingLoaded = true;
            Debug.Log("Megamothling Loaded");
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
        if (type == typeof(FLadybug) && _ladybugEnemyAssetHandle.IsValid())
        {
            var prefab = _ladybugEnemyAssetHandle.Result;
            return CreateLadybugInstance(prefab);    
        }
        if (type == typeof(FMegamothling) && _megamothlingEnemyAssetHandle.IsValid())
        {
            var prefab = _megamothlingEnemyAssetHandle.Result;
            return CreateMegamothlingInstance(prefab);    
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
        enemyInstance.GetComponent<FSpiderMovement>().Construct(
            _spiderMovementStateFactory,
            _lampPositionProviderService);
        enemyInstance.GetComponent<FSpiderPresentation>().Initialize();
        var enemy = enemyInstance.GetComponent<FSpider>();
        enemy.Initialize();
        
        return enemy;
    }
    
    private FEnemy CreateLadybugInstance(GameObject prefab)
    {
        GameObject enemyInstance = Object.Instantiate(prefab);
        enemyInstance.GetComponent<FLadybugMovement>().Construct(
            _ladybugMovementStateFactory);
        enemyInstance.GetComponent<FLadybugPresentation>().Initialize();
        var enemy = enemyInstance.GetComponent<FLadybug>();
        enemy.Initialize();
        
        return enemy;
    }
    
    private FEnemy CreateMegamothlingInstance(GameObject prefab)
    {
        GameObject enemyInstance = Object.Instantiate(prefab);
        enemyInstance.GetComponent<FMegamothlingMovement>().Construct(_megamothlingMovementStateFactory);
        // enemyInstance.GetComponent<FMothlingPresentation>().Initialize();
        var enemy = enemyInstance.GetComponent<FMegamothling>();
        enemy.Initialize();
        
        return enemy;
    } 
}
