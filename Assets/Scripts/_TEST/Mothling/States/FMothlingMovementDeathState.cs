using UnityEngine;

public class FMothlingMovementDeathState: FMothlingMovementStateBase
{
    private IPosition2DProvider _positionProvider;
    
    // State specific attributes
    private Vector2 _bounceForce;
    private Vector2 _gravityForce;
    private readonly float _bounceForceMagnitude = 4f;
    private readonly float _gravityForceMagnitude = .2f;
    private readonly float _dragAmount = 0.94f;
    private readonly float _speedMultiplier = 0.9f;
    private readonly float _fallBottomYcoordinate = -6f;
    
    public FMothlingMovementDeathState(IPosition2DProvider positionProvider)
    {
        _positionProvider = positionProvider;
    }
    
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
        Position2D += _bounceForce * (Time.deltaTime * _speedMultiplier) + _gravityForce;
        _bounceForce *= _dragAmount;
        _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
   
        if (Position2D.y < -_fallBottomYcoordinate)
        {
            ReadyToSwitch = true;
        }
    }
}
