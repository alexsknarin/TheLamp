using UnityEngine;

public class FMothlingMovementAttackState: FMothlingMovementStateBase
{
    private readonly IPosition2DProvider _positionProvider;
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f); // DI
    
    // State specific attributes
    private readonly float _depthDecrement = 0.42f;
    private float _startDistance;
    private readonly float _speed = 0.81f;
    
    public FMothlingMovementAttackState(IPosition2DProvider positionProvider)
    {
        _positionProvider = positionProvider;
    }
    
    public override void OnEnter()
    {
        _startDistance = _positionProvider.Position2D.magnitude - 0.65f;
    }

    public override void Tick()
    {
        Vector2 newPosition = _positionProvider.Position2D;
        Vector2 direction = -newPosition.normalized;
        newPosition += direction * (_speed * Time.deltaTime);
        Position2D = newPosition;
        
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        float attackProximityGradient = Mathf.Clamp((Position2D.magnitude - 0.72f) / _startDistance, 0.0f, 1.0f);
        attackProximityGradient = Mathf.Pow(attackProximityGradient, 1.9f) + 0.1f;
        DepthDirection = cameraDirection * (1.1f * _depthDecrement * attackProximityGradient);
    }
}
