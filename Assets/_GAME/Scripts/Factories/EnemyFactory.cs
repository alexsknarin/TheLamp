using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Dragonfly;
using _GAME.Scripts.Enemies.Dragonfly.Presentation;
using _GAME.Scripts.Enemies.FireFly;
using _GAME.Scripts.Enemies.Fly;
using _GAME.Scripts.Enemies.Ladybug;
using _GAME.Scripts.Enemies.Megabeetle;
using _GAME.Scripts.Enemies.Megamothling;
using _GAME.Scripts.Enemies.Megaspider;
using _GAME.Scripts.Enemies.Moth;
using _GAME.Scripts.Enemies.Mothling;
using _GAME.Scripts.Enemies.Spider;
using _GAME.Scripts.Enemies.Wasp;
using _GAME.Scripts.Lib.Interfaces;
using _GAME.Scripts.ServicesGlobal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace _GAME.Scripts.Factories
{
    public class EnemyFactory
    {
        private readonly ILampPositionProviderService _lampPositionProviderService;
        private readonly MothlingMovementStateFactory _mothlingMovementStateFactory;
        private readonly FlyMovementStateFactory _flyMovementStateFactory;
        private readonly MothMovementStateFactory _mothMovementStateFactory;
        private readonly SpiderMovementStateFactory _spiderMovementStateFactory;
        private readonly LadybugMovementStateFactory _ladybugMovementStateFactory;
        private readonly MegamothlingMovementStateFactory _megamothlingMovementStateFactory;
        private readonly MegabeetleMovementStateFactory _megabeetleMovementStateFactory;
        private readonly DragonflyBehaviourStateFactory _dragonflyBehaviourStateFactory;
        private readonly MegaspiderMovementStateFactory _megaspiderMovementStateFactory;
        private readonly MegaspiderProjectileSpiderMovementStateFactory _megaspiderProjectileSpiderMovementStateFactory;
        private readonly IGameConfigService _gameConfigService;
        private readonly ISpiderSideDirectionProvider _spiderSideDirectionProvider;
        private readonly FullscreenRendererFeatureProvider _fullscreenRendererFeatureProvider;
        private readonly Camera _camera;
        private readonly Transform _lampTransform;
    
        AsyncOperationHandle<GameObject> _mothlingEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _flyEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _fireFlyEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _mothEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _spiderEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _ladybugEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _megamothlingEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _waspEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _megabeetleEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _dragonflyEnemyAssetHandle;
        AsyncOperationHandle<GameObject> _megaspiderEnemyAssetHandle;
    
    
        public EnemyFactory(
            MothlingMovementStateFactory mothlingMovementStateFactory,
            FlyMovementStateFactory flyMovementStateFactory,
            MothMovementStateFactory mothMovementStateFactory,
            SpiderMovementStateFactory spiderMovementStateFactory,
            LadybugMovementStateFactory ladybugMovementStateFactory,
            MegamothlingMovementStateFactory megamothlingMovementStateFactory,
            MegabeetleMovementStateFactory megabeetleMovementStateFactory,
            DragonflyBehaviourStateFactory dragonflyBehaviourStateFactory,
            MegaspiderMovementStateFactory megaspiderMovementStateFactory,
            MegaspiderProjectileSpiderMovementStateFactory megaspiderProjectileSpiderMovementStateFactory,
            ILampPositionProviderService lampPositionProviderService,
            IGameConfigService gameConfigService,
            ISpiderSideDirectionProvider spiderSideDirectionProvider,
            FullscreenRendererFeatureProvider fullscreenRendererFeatureProvider,
            Camera currentCamera,
            Transform lampTransform
            )
        {
            _mothlingMovementStateFactory = mothlingMovementStateFactory;
            _flyMovementStateFactory = flyMovementStateFactory;
            _mothMovementStateFactory = mothMovementStateFactory;
            _spiderMovementStateFactory = spiderMovementStateFactory;
            _ladybugMovementStateFactory = ladybugMovementStateFactory;
            _megamothlingMovementStateFactory = megamothlingMovementStateFactory;
            _dragonflyBehaviourStateFactory = dragonflyBehaviourStateFactory;
            _megabeetleMovementStateFactory = megabeetleMovementStateFactory;
            _megaspiderMovementStateFactory = megaspiderMovementStateFactory;
            _megaspiderProjectileSpiderMovementStateFactory = megaspiderProjectileSpiderMovementStateFactory;
            _lampPositionProviderService = lampPositionProviderService;
            _gameConfigService = gameConfigService;
            _spiderSideDirectionProvider = spiderSideDirectionProvider;
            _fullscreenRendererFeatureProvider = fullscreenRendererFeatureProvider;
            _camera = currentCamera;
            _lampTransform = lampTransform;
        
            IsMothlingLoaded = false;
            IsFlyLoaded = false;
            IsFireFlyLoaded = false;
            IsMothLoaded = false;
            IsSpiderLoaded = false;
            IsMegamothlingLoaded = false;
            IsWaspLoaded = false;
            IsMegabeetleLoaded = false;
            IsDragonflyLoaded = false;
            IsMegaspiderLoaded = false;
        }
    
        public bool IsMothlingLoaded { get; private set; }
        public bool IsFlyLoaded { get; private set; }
        public bool IsFireFlyLoaded { get; private set; }
        public bool IsMothLoaded { get; private set; }
        public bool IsSpiderLoaded { get; private set; }
        public bool IsLadybugLoaded { get; private set; }
        public bool IsMegamothlingLoaded { get; private set; }
        public bool IsWaspLoaded { get; private set; }
        public bool IsMegabeetleLoaded { get; private set; }
        public bool IsDragonflyLoaded { get; private set; }
        public bool IsMegaspiderLoaded { get; private set; }
    
    

        public async void LoadEnemy(Type type)
        {
            if (type == typeof(Mothling))
            {
                _mothlingEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/Mothling.prefab");
                await _mothlingEnemyAssetHandle.Task;
                IsMothlingLoaded = true;
            }
            if (type == typeof(Fly))
            {
                _flyEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/Fly.prefab");
                await _flyEnemyAssetHandle.Task;
                IsFlyLoaded = true;
            }

            if (type == typeof(FireFly))
            {
                _fireFlyEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/FireFly.prefab");
                await _fireFlyEnemyAssetHandle.Task;
                IsFireFlyLoaded = true;
            }
        
            if (type == typeof(Moth))
            {
                _mothEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/Moth.prefab");
                await _mothEnemyAssetHandle.Task;
                IsMothLoaded = true;
            }
        
            if (type == typeof(Spider))
            {
                _spiderEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/Spider.prefab");
                await _spiderEnemyAssetHandle.Task;
                IsSpiderLoaded = true;
            }
        
            if (type == typeof(Ladybug))
            {
                _ladybugEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Enemy/Ladybug.prefab");
                await _ladybugEnemyAssetHandle.Task;
                IsLadybugLoaded = true;
            }
        
            if (type == typeof(Megamothling))
            {
                _megamothlingEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Boss/Megamothling.prefab");
                await _megamothlingEnemyAssetHandle.Task;
                IsMegamothlingLoaded = true;
            }
        
            if (type == typeof(Wasp))
            {
                _waspEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Boss/Wasp.prefab");
                await _waspEnemyAssetHandle.Task;
                IsWaspLoaded = true;
            }
        
            if (type == typeof(Megabeetle))
            {
                _megabeetleEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Boss/Megabeetle.prefab");
                await _megabeetleEnemyAssetHandle.Task;
                IsMegabeetleLoaded = true;
            }
        
            if (type == typeof(Dragonfly))
            {
                _dragonflyEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Boss/Dragonfly.prefab");
                await _dragonflyEnemyAssetHandle.Task;
                IsDragonflyLoaded = true;
            }
            
            if (type == typeof(Megaspider))
            {
                _megaspiderEnemyAssetHandle = Addressables.LoadAssetAsync<GameObject>("Boss/Megaspider.prefab");
                await _megaspiderEnemyAssetHandle.Task;
                IsMegaspiderLoaded = true;
            }
        }
    
        public Enemy CreateEnemy(Type type)
        {
            if (type == typeof(Mothling) && _mothlingEnemyAssetHandle.IsValid())
            {
                var prefab = _mothlingEnemyAssetHandle.Result;
                return CreateMothlingInstance(prefab);    
            }
            if (type == typeof(Fly) && _flyEnemyAssetHandle.IsValid())
            {
                var prefab = _flyEnemyAssetHandle.Result;
                return CreateFlyInstance(prefab);    
            }
            if (type == typeof(FireFly) && _fireFlyEnemyAssetHandle.IsValid())
            {
                var prefab = _fireFlyEnemyAssetHandle.Result;
                return CreateFireFlyInstance(prefab);    
            }
            if (type == typeof(Moth) && _mothEnemyAssetHandle.IsValid())
            {
                var prefab = _mothEnemyAssetHandle.Result;
                return CreateMothInstance(prefab);    
            }
            if (type == typeof(Spider) && _spiderEnemyAssetHandle.IsValid())
            {
                var prefab = _spiderEnemyAssetHandle.Result;
                return CreateSpiderInstance(prefab);    
            }
            if (type == typeof(Ladybug) && _ladybugEnemyAssetHandle.IsValid())
            {
                var prefab = _ladybugEnemyAssetHandle.Result;
                return CreateLadybugInstance(prefab);    
            }
            if (type == typeof(Megamothling) && _megamothlingEnemyAssetHandle.IsValid())
            {
                var prefab = _megamothlingEnemyAssetHandle.Result;
                return CreateMegamothlingInstance(prefab);    
            }
            if (type == typeof(Wasp) && _waspEnemyAssetHandle.IsValid())
            {
                var prefab = _waspEnemyAssetHandle.Result;
                return CreateWaspsInstance(prefab);    
            }
            if (type == typeof(Megabeetle) && _megabeetleEnemyAssetHandle.IsValid())
            {
                var prefab = _megabeetleEnemyAssetHandle.Result;
                return CreateMegabeetleInstance(prefab);    
            }
            if (type == typeof(Dragonfly) && _dragonflyEnemyAssetHandle.IsValid())
            {
                var prefab = _dragonflyEnemyAssetHandle.Result;
                return CreateDragonflyInstance(prefab);    
            }
            if (type == typeof(Megaspider) && _megaspiderEnemyAssetHandle.IsValid())
            {
                var prefab = _megaspiderEnemyAssetHandle.Result;
                return CreateMegaspiderInstance(prefab);    
            }
            else
            {
                throw new Exception("Enemy Factory: Enemy type not loaded");
            }
        }
    
        private Enemy CreateMothlingInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            enemyInstance.GetComponent<MothlingMovement>().Construct(_mothlingMovementStateFactory);
            enemyInstance.GetComponent<MothlingPresentation>().Initialize();
            var enemy = enemyInstance.GetComponent<Mothling>();
            enemy.Initialize();
        
            return enemy;
        } 
    
        private Enemy CreateFlyInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            enemyInstance.GetComponent<FlyMovement>().Construct(_flyMovementStateFactory);
            enemyInstance.GetComponent<FlyPresentation>().Initialize();
            var enemy = enemyInstance.GetComponent<Fly>();
            enemy.Initialize();
        
            return enemy;
        }
    
        private Enemy CreateFireFlyInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            enemyInstance.GetComponent<FlyMovement>().Construct(_flyMovementStateFactory);
            enemyInstance.GetComponent<FireFlyPresentation>().Initialize();
            var enemy = enemyInstance.GetComponent<FireFly>();
            enemy.Initialize();
        
            return enemy;
        }
    
        private Enemy CreateMothInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            enemyInstance.GetComponent<MothMovement>().Construct(_mothMovementStateFactory);
            enemyInstance.GetComponent<MothPresentation>().Initialize();
            var enemy = enemyInstance.GetComponent<Moth>();
            enemy.Initialize();
        
            return enemy;
        }
    
        private Enemy CreateSpiderInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            enemyInstance.GetComponent<SpiderMovement>().Construct(
                _spiderMovementStateFactory,
                _lampPositionProviderService,
                _spiderSideDirectionProvider,
                _gameConfigService.PlayerConfig.LampCollisionRadius,
                _gameConfigService.PlayerConfig.CollisionThreshold
                );
            enemyInstance.GetComponent<SpiderPresentation>().Initialize();
            var enemy = enemyInstance.GetComponent<Spider>();
            enemy.Initialize();
        
            return enemy;
        }
    
        private Enemy CreateLadybugInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            enemyInstance.GetComponent<LadybugMovement>().Construct(_ladybugMovementStateFactory);
            enemyInstance.GetComponent<LadybugPresentation>().Initialize();
            enemyInstance.GetComponent<LadybugBodyRotationHandler>().Construct(_lampPositionProviderService);
            var enemy = enemyInstance.GetComponent<Ladybug>();
            enemy.Initialize();
        
            return enemy;
        }
    
        private Enemy CreateMegamothlingInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            enemyInstance.GetComponent<MegamothlingMovement>().Construct(_megamothlingMovementStateFactory);
            enemyInstance.GetComponent<MegamothlingPresentation>().Initialize();
            var enemy = enemyInstance.GetComponent<Megamothling>();
            enemy.Initialize();
        
            return enemy;
        } 
    
        private Enemy CreateWaspsInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            enemyInstance.transform.GetChild(0).GetComponent<WaspMovement>().Construct(
                _lampPositionProviderService);
            enemyInstance.GetComponent<WaspPresentation>().Initialize();
            var enemy = enemyInstance.GetComponent<Wasp>();
            enemy.Initialize();
        
            return enemy;
        }
    
        private Enemy CreateMegabeetleInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            enemyInstance.GetComponent<MegabeetleMovement>().Construct(_megabeetleMovementStateFactory);
            enemyInstance.GetComponent<MegabeetlePresentation>().Initialize();
            enemyInstance.GetComponent<MegabeetleBodyRotationHandler>().Construct(_lampPositionProviderService);
            enemyInstance.GetComponent<MegabeetleDamageFXRotationHandler>().Construct(_lampPositionProviderService);
            var enemy = enemyInstance.GetComponent<Megabeetle>();
            enemy.Initialize();
        
            return enemy;
        }
    
        private Enemy CreateDragonflyInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            var enemy = enemyInstance.GetComponent<Dragonfly>();
            enemy.Construct(_dragonflyBehaviourStateFactory, _lampPositionProviderService);
            enemy.Initialize();
            enemyInstance.GetComponent<DragonflySwarmCallFX>().Construct(
                _fullscreenRendererFeatureProvider.Get(), 
                _camera
                );
            enemyInstance.GetComponent<DragonflyPresentation>().Initialize();

            return enemy;
        }
        
        private Enemy CreateMegaspiderInstance(GameObject prefab)
        {
            GameObject enemyInstance = Object.Instantiate(prefab);
            var enemy = enemyInstance.GetComponent<Megaspider>();
            enemy.Construct(_camera.transform);
            enemyInstance.GetComponent<MegaspiderMovement>().Construct(
                _camera.transform,
                _lampTransform, 
                _megaspiderMovementStateFactory,
                _gameConfigService
                );
            enemyInstance.GetComponent<MegaspiderSwarm>().Construct(_lampTransform, _megaspiderProjectileSpiderMovementStateFactory);
            enemy.Initialize();
            enemyInstance.GetComponent<MegaspiderSpiderwebController>().Construct(_gameConfigService);
            enemyInstance.GetComponent<MegaspiderSpiderwebController>().Initialize();
            enemyInstance.GetComponent<MegaspiderPresentation>().Initialize();
            
            return enemy;
        }
    }
}
