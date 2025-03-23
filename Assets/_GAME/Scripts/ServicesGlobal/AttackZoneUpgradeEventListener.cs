using _GAME.Scripts.GameCoreSystems;using _GAME.Scripts.Lib.Interfaces;
using _GAME.Scripts.ServicesGlobal;
using UnityEngine;

public class AttackZoneUpgradeEventListener: IDisposable
{
    private GameModel _gameModel;
    private LampCollisionDetectionService _lampCollisionDetectionService;
    private LampStickyDetectionService _lampStickyDetectionService;

    public AttackZoneUpgradeEventListener(
        GameModel gameModel,
        LampCollisionDetectionService lampCollisionDetectionService,
        LampStickyDetectionService lampStickyDetectionService
        )
    {
        _gameModel = gameModel;
        _lampCollisionDetectionService = lampCollisionDetectionService;
        _lampStickyDetectionService = lampStickyDetectionService;
        
        _gameModel.LampAttackDistanceChanged += _lampCollisionDetectionService.UpdateAttackZoneRadius;
        _gameModel.LampAttackDistanceChanged += _lampStickyDetectionService.UpdateAttackZoneRadius;
        
    }

    public void Dispose()
    {
        _gameModel.LampAttackDistanceChanged -= _lampCollisionDetectionService.UpdateAttackZoneRadius;
        _gameModel.LampAttackDistanceChanged -= _lampStickyDetectionService.UpdateAttackZoneRadius;
    }
}
