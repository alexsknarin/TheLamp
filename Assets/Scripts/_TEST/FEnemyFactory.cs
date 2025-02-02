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
    AsyncOperationHandle<GameObject> _mothlingEnemyAssetHandle;
    
    public FEnemyFactory(MothlingMovementStateFactory mothlingMovementStateFactory)
    {
        _mothlingMovementStateFactory = mothlingMovementStateFactory;
    }
    
    public async Task<FEnemy> CreateMothling()
    {
        if (_mothlingEnemyAssetHandle.IsValid())
        {
            var prefab = _mothlingEnemyAssetHandle.Result;
            return CreateEnemyInstance(prefab);    
        }
        else
        {
            _mothlingEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/FMothling.prefab");
            await _mothlingEnemyAssetHandle.Task;
        
            var prefab = _mothlingEnemyAssetHandle.Result;
            return CreateEnemyInstance(prefab);    
        }
    }
    
    private FEnemy CreateEnemyInstance(GameObject prefab)
    {
        GameObject enemyInstance = Object.Instantiate(prefab);
        enemyInstance.GetComponent<FMothlingMovement>().Construct(_mothlingMovementStateFactory);
        enemyInstance.GetComponent<FMothlingPresentation>().Initialize();
        enemyInstance.GetComponent<FMothling>().Initialize();
        
        return enemyInstance.GetComponent<FMothling>();
    } 
}
