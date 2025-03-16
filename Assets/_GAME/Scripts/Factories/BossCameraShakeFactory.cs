using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Dragonfly;
using _GAME.Scripts.Enemies.Megabeetle;
using _GAME.Scripts.Enemies.Megamothling;
using _GAME.Scripts.Enemies.Wasp;
using _GAME.Scripts.InGamePresentation.CameraShake;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class BossCameraShakeFactory 
    {
        private WaspCameraShakeStrategy _waspCameraShakeStrategy;
        private MegamothlingCameraShakeStrategy _megamothlingCameraShakeStrategy;
        private MegabeetleCameraShakeStrategy _megabeetleCameraShakeStrategy;
        private DragonflyCameraShakeStrategy _dragonflyCameraShakeStrategy;
    
        public ICameraShakeStrategy Create(FEnemy boss)
        {
            if (boss is Wasp)
            {
                if (_waspCameraShakeStrategy is null)
                {
                    LoadStrategy(
                        ref _waspCameraShakeStrategy, 
                        "Behaviors/WaspCameraShakeStrategy",
                        ((Wasp)boss).MovementTransform
                    );
                }
                else
                {
                    _waspCameraShakeStrategy.Construct(((Wasp)boss).MovementTransform);
                }
                return _waspCameraShakeStrategy;
            }
            if (boss is Megamothling)
            {
                if (_megamothlingCameraShakeStrategy is null)
                {
                    LoadStrategy(
                        ref _megamothlingCameraShakeStrategy,
                        "Behaviors/MegamothlingCameraShakeStrategy",
                        boss.transform
                    );
                }
                else
                {
                    _megamothlingCameraShakeStrategy.Construct(boss.transform);
                }
                return _megamothlingCameraShakeStrategy;    
            }
            if (boss is Megabeetle)
            {
                if (_megabeetleCameraShakeStrategy is null)
                {
                    LoadStrategy(
                        ref _megabeetleCameraShakeStrategy,
                        "Behaviors/MegabeetleCameraShakeStrategy",
                        boss.transform
                    );
                }
                else
                {
                    _megabeetleCameraShakeStrategy.Construct(boss.transform);
                }
                return _megabeetleCameraShakeStrategy;    
            }
            if (boss is Dragonfly)
            {
                if (_dragonflyCameraShakeStrategy is null)
                {
                    LoadStrategy(
                        ref _dragonflyCameraShakeStrategy,
                        "Behaviors/DragonflyCameraShakeStrategy",
                        ((Dragonfly)boss).MovementTransform
                    );  
                }
                else
                {
                    _dragonflyCameraShakeStrategy.Construct(((Dragonfly)boss).MovementTransform);
                }
                return _dragonflyCameraShakeStrategy;
            }
            return null;
        }
    
        private void LoadStrategy<T>(ref T strategy, string path, Transform transform) where T : BaseCameraShakeStrategy
        {
            if (strategy is not null)
            {
                strategy.Construct(transform);
                return;
            }
            var so = Resources.Load<T>(path);
            strategy = Object.Instantiate(so);
            strategy.Construct(transform);
            strategy.Initialize();
        }
    }
}
