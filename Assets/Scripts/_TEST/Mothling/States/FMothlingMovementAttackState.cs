using UnityEngine;

public class FMothlingMovementAttackState: FMothlingMovementStateBase
{
    // Dependencies
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPosition2DProvider _position2DProvider;
    private ILampPositionProviderService _lampPositionProviderService;  // TODO: enable later
    private readonly float _speed;

    // State specific attributes
    private readonly float _depthDecrement = 0.42f;
    private float _startDistance;
    private readonly float _speedMultiplier =  1.1f;

    public FMothlingMovementAttackState(
        Vector3 cameraPosition,
        IPosition2DProvider positionProvider,
        ILampPositionProviderService lampPositionProviderService,
        float speed
        )
    {
        // _cameraPosition = cameraPosition; // TODO: enable later
        _position2DProvider = positionProvider;
        _lampPositionProviderService = lampPositionProviderService;
        _speed = speed * _speedMultiplier;
    }
    
    public override void OnEnter()
    {
        _startDistance = _position2DProvider.Position2D.magnitude - 0.65f;
    }

    public override void Tick()
    {
        Vector2 newPosition = _position2DProvider.Position2D;
        Vector2 direction = -newPosition.normalized;
        newPosition += direction * (_speed * Time.deltaTime);
        Position2D = newPosition;
        
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        float attackProximityGradient = Mathf.Clamp((Position2D.magnitude - 0.72f) / _startDistance, 0.0f, 1.0f);
        attackProximityGradient = Mathf.Pow(attackProximityGradient, 1.9f) + 0.1f;
        DepthDirection = cameraDirection * (1.1f * _depthDecrement * attackProximityGradient);
    }
}
