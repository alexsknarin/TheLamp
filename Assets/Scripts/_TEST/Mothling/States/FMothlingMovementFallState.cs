using UnityEngine;

public class FMothlingMovementFallState: FMothlingMovementStateBase
{
    private IPositionDirectionProvider _positionDirectionProvider;
    private readonly float _radius;
    private readonly float _verticalAmplitude;
    
    // add Lamp position provider 
    
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
        // Lamp collision radius 0.49 - get from Game config
        // Current radius 0.075 - get from Game Enemy Config
        // 0.0001f - threshold - get from game config
        
        Position2D = _positionDirectionProvider.Position2D.normalized * (0.49f + 0.075f + 0.0001f);
        DepthDirection = Vector3.zero;
        
        _bounceForce = Position2D.normalized * _bounceForceMagnitude;
        _gravityForce = Vector2.zero;
        IsReadyToSwitch = false;
    }

    public override void Tick()
    {
        Position2D += _bounceForce * Time.deltaTime + _gravityForce;
        _bounceForce *= _dragAmount;
        _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
   
        if (Position2D.y < -_radius * _verticalAmplitude * 1.1)
        {
            IsReadyToSwitch = true;
        }
    }
}
