using UnityEngine;

public class FFlyGenericMovementAcceleratedAttackState : RegularEnemyMovementStateBase
{
    // Dependencies
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private readonly ILampPositionProviderService _lampPositionProviderService;
    private readonly float _speed;
    private readonly float _proximityOffset;

    // State specific attributes
    private float _acceleration = 13.5f;
    private float _depthDecrement = 0.4f;

    private float _acceleratedSpeed = 1f;
    private float _startDistance;
    private readonly float _speedMultiplier =  1.1f;

    public FFlyGenericMovementAcceleratedAttackState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        ILampPositionProviderService lampPositionProviderService,
        float speed,
        float proximityOffset
    )
    {
        // _cameraPosition = cameraPosition; // TODO: enable later
        _positionDirectionProvider = positionDirectionProvider;
        _lampPositionProviderService = lampPositionProviderService;
        _speed = speed * _speedMultiplier;
        _proximityOffset = proximityOffset;
    }
    
    public override void OnEnter()
    {
        _acceleratedSpeed = 1f;
        _startDistance = _positionDirectionProvider.Position2D.magnitude - 0.65f;
        Position2D = _positionDirectionProvider.Position2D;
    }

    public override void Tick()
    {
        Vector2 direction = -(Position2D - (Vector2)_lampPositionProviderService.GetLampPosition()).normalized;
        Position2D += direction * (_speed * _acceleratedSpeed * Time.deltaTime);
        _acceleratedSpeed += _acceleration * Time.deltaTime;
        
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        float attackProximityGradient = Mathf.Clamp((Position2D.magnitude - 0.65f) / _startDistance, 0.0f, 1.0f);
        attackProximityGradient = Mathf.Pow(attackProximityGradient, 2.0f) + _proximityOffset;
        DepthDirection = cameraDirection * (2.5f * _depthDecrement * attackProximityGradient);
    }
}
