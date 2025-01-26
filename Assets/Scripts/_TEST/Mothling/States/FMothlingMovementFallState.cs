using UnityEngine;

public class FMothlingMovementFallState: FMothlingMovementStateBase
{
    private IPositionDirectionProvider _positionDirectionProvider;
    private readonly float _radius;
    private readonly float _verticalAmplitude;
    
    public FMothlingMovementFallState(
        IPositionDirectionProvider positionDirectionProvider,
        float radius,
        float verticalAmplitude
    )
    {
        _positionDirectionProvider = positionDirectionProvider;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
    }
    
    // State specific attributes
    private Vector2 _bounceForce;
    private Vector2 _gravityForce;
    private readonly float _bounceForceMagnitude = 4f;
    private readonly float _gravityForceMagnitude = .2f;
    private readonly float _dragAmount = 0.94f;
    
    public override void OnEnter()
    {
        Position2D = _positionDirectionProvider.Position2D;
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
