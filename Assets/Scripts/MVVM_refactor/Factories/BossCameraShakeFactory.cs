using System.Data.Common;
using UnityEngine;

public class BossCameraShakeFactory 
{
    private WaspCameraShakeStrategy _waspCameraShakeStrategy;
    
    public ICameraShakeStrategy CreateCameraShakeStrategy(BossBase boss)
    {
        if (boss is Wasp)
        {
           if (_waspCameraShakeStrategy is not null)
           {
               _waspCameraShakeStrategy.Construct(boss.transform);
               return _waspCameraShakeStrategy;
           }
           else
           {
               var so = Resources.Load<WaspCameraShakeStrategy>("Behaviors/WaspCameraShakeStrategy");
               _waspCameraShakeStrategy = Object.Instantiate(so);
               _waspCameraShakeStrategy.Construct(boss.transform);
               _waspCameraShakeStrategy.Initialize();
               return _waspCameraShakeStrategy;
           }
        }
        
        return null;
    }
}
