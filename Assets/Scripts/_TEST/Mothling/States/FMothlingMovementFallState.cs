using UnityEngine;

public class FMothlingMovementFallState: FMothlingMovementStateBase
{
    private IPosition2DProvider _positionProvider;
    private float _radius;
    private float _verticalAmplitude;
    
    public FMothlingMovementFallState(
        IPosition2DProvider positionProvider,
        float radius,
        float verticalAmplitude
    )
    {
        _positionProvider = positionProvider;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
    }
    
    // State specific attributes
    private Vector2 _bounceForce;
    private Vector2 _gravityForce;
    private float _bounceForceMagnitude = 4f;
    private float _gravityForceMagnitude = .2f;
    private float _dragAmount = 0.94f;
    
    public override void OnEnter()
    {
        Position2D = _positionProvider.Position2D;
        DepthDirection = Vector3.zero;
        
        _bounceForce = Position2D.normalized * _bounceForceMagnitude;
        _gravityForce = Vector2.zero;
        ReadyToSwitch = false;
    }

    public override void Tick()
    {
        Position2D += _bounceForce * Time.deltaTime + _gravityForce;
        _bounceForce *= _dragAmount;
        _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
   
        if (Position2D.y < -_radius * _verticalAmplitude * 1.1)
        {
            ReadyToSwitch = true;
        }
    }
}
