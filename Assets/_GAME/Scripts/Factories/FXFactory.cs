using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class FXFactory
{
    private IGameConfigService _gameConfigService;
    public FXFactory(IGameConfigService gameConfigService)
    {
        _gameConfigService = gameConfigService;
    }
    
    AsyncOperationHandle<GameObject> _fireflyExplosionAssetHandle;
    FireflyExplosion _fireflyExplosion = null;
    public bool IsFireflyExplosionLoaded { get; private set; }

    
    public async void Load(Type type)
    {
        if (type == typeof(FireflyExplosion))
        {
            _fireflyExplosionAssetHandle = Addressables.LoadAssetAsync<GameObject>("FX/FireflyExplosion.prefab");
            await _fireflyExplosionAssetHandle.Task;
            IsFireflyExplosionLoaded = true;
        }
    }
    
    public FireflyExplosion GetFireflyExplosion()
    {
        if (_fireflyExplosion != null)
        {
            return _fireflyExplosion;
        }
        
        if (!IsFireflyExplosionLoaded)
        {
            Debug.LogError("FireflyExplosion prefab is not loaded");
            return null;
        }
        
        var fireflyExplosionObject = GameObject.Instantiate(_fireflyExplosionAssetHandle.Result);
        fireflyExplosionObject.name = "FireflyExplosion01";
        var fireflyExplosion = fireflyExplosionObject.GetComponent<FireflyExplosion>();
        fireflyExplosion.Initialize();
        fireflyExplosion.Counstruct(
            _gameConfigService.GameConfig.FireflyExplosionRadius, 
            _gameConfigService.GameConfig.FireflyExplosionDuration
            );
        return fireflyExplosion;
    }
}
