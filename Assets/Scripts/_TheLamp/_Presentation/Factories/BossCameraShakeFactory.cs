using System.Data.Common;
using UnityEngine;

public class BossCameraShakeFactory 
{
    private WaspCameraShakeStrategy _waspCameraShakeStrategy;
    private MegamothlingCameraShakeStrategy _megamothlingCameraShakeStrategy;
    private MegabeetleCameraShakeStrategy _megabeetleCameraShakeStrategy;
    private DragonflyCameraShakeStrategy _dragonflyCameraShakeStrategy;
    
    public ICameraShakeStrategy Create(FEnemy boss)
    {
        if (boss is FWasp)
        {
            if (_waspCameraShakeStrategy is null)
            {
                LoadStrategy(
                    ref _waspCameraShakeStrategy, 
                    "Behaviors/WaspCameraShakeStrategy",
                    ((FWasp)boss).MovementTransform
                );
            }
            else
            {
                _waspCameraShakeStrategy.Construct(((FWasp)boss).MovementTransform);
            }
            return _waspCameraShakeStrategy;
        }
        if (boss is FMegamothling)
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
        if (boss is FMegabeetle)
        {
            LoadStrategy(
                ref _megabeetleCameraShakeStrategy,
                "Behaviors/MegabeetleCameraShakeStrategy",
                boss.transform
                );
            return _megabeetleCameraShakeStrategy;
        }
        // if (boss is Dragonfly)
        // {
        //     LoadStrategy(
        //         ref _dragonflyCameraShakeStrategy,
        //         "Behaviors/DragonflyCameraShakeStrategy",
        //         boss.transform
        //         );
        //     return _dragonflyCameraShakeStrategy;
        // }
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
