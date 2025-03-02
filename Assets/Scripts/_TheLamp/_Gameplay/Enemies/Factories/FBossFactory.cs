using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class FBossFactory
{
    private ILampPositionProviderService _lampPositionProviderService;
    private MegamothlingMovementStateFactory _megamothlingMovementStateFactory;
    AsyncOperationHandle<GameObject> _megamothlingEnemyAssetHandle;
    
    public FBossFactory(
        MegamothlingMovementStateFactory megamothlingMovementStateFactory,
        ILampPositionProviderService lampPositionProviderService
    )
    {
        _megamothlingMovementStateFactory = megamothlingMovementStateFactory;
        _lampPositionProviderService = lampPositionProviderService;
        
        IsMegamothlingLoaded = false;
    }
    
    public bool IsMegamothlingLoaded { get; private set; }
    
    public async void LoadBoss(Type type)
    {
        if (type == typeof(FMegamothling))
        {
            _megamothlingEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/FMothling.prefab");
            await _megamothlingEnemyAssetHandle.Task;
            IsMegamothlingLoaded = true;
            Debug.Log("Megamothling Loaded");
        }
    }
    
    public FEnemy CreateBoss(Type type)
    {
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
