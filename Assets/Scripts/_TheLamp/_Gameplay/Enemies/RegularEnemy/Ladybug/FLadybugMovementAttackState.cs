using UnityEngine;

public class FLadybugMovementAttackState : RegularEnemyMovementStateBase
{
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private readonly ILampPositionProviderService _lampPositionProviderService;
    private readonly float _speed;

    public FLadybugMovementAttackState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        ILampPositionProviderService lampPositionProviderService,
        float speed
    )
    {
        _cameraPosition = cameraPosition;
        _positionDirectionProvider = positionDirectionProvider;
        _lampPositionProviderService = lampPositionProviderService;
        _speed = speed;
    }
    
    public override void OnEnter()
    {
    }
    
    public override void Tick()
    {
        Vector2 newPosition = _positionDirectionProvider.Position2D;
        Vector2 direction = (_lampPositionProviderService.GetLampPosition() - Position2D).normalized;
        newPosition += direction * (_speed * Time.deltaTime);
        Position2D = newPosition;
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * 0.3f;
    }
}
