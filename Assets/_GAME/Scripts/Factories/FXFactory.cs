using System;
using _GAME.Scripts.InGamePresentation.FX;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _GAME.Scripts.Factories
{
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
}
