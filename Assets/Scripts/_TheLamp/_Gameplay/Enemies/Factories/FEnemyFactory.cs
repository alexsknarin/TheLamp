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
    
    AsyncOperationHandle<GameObject> _mothlingEnemyAssetHandle;
    AsyncOperationHandle<GameObject> _flyEnemyAssetHandle;
    
    public FEnemyFactory(
        MothlingMovementStateFactory mothlingMovementStateFactory,
        FlyMovementStateFactory flyMovementStateFactory
    )
    {
        _mothlingMovementStateFactory = mothlingMovementStateFactory;
        _flyMovementStateFactory = flyMovementStateFactory;
        IsMothlingLoaded = false;
    }
    
    public bool IsMothlingLoaded { get; private set; }
    public bool IsFlyLoaded { get; private set; }

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
}
