using UnityEngine;

public class FLadybugMovementStickState: RegularEnemyMovementStateBase
{
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private readonly ILampPositionProviderService _lampPositionProviderService;
    private readonly float _collisionRadius;
    
    private readonly float _lampStickRadius = 0.35f;
    private readonly float _depth = 0.3f;
    
    public FLadybugMovementStickState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        ILampPositionProviderService lampPositionProviderService,
        float collisionRadius
        )
    {
        _cameraPosition = cameraPosition;
        _positionDirectionProvider = positionDirectionProvider;
        _lampPositionProviderService = lampPositionProviderService;
        _collisionRadius = collisionRadius;
    }
    
    public override void OnEnter()
    {
        Vector2 newPosition = (_positionDirectionProvider.Position2D - _lampPositionProviderService.GetLampPosition()).normalized 
                              * (_lampStickRadius + _collisionRadius); 
        Position2D = newPosition;
        
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * _depth;
    }

    public override void Tick()
    {
    }
}
