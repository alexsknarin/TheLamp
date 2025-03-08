using UnityEngine;

public class FMegabeetleMovementStickLandingState : RegularEnemyMovementStateBase
{
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    
    private readonly float NEUTRAL_DISTANCE = 0.44f;
    private float _duration = .491f;
    private float _phase;
    private float _localTime;
    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private float _startDistance;
    
    public FMegabeetleMovementStickLandingState(IPositionDirectionProvider positionDirectionProvider)
    {
        _positionDirectionProvider = positionDirectionProvider;
    }
    
    public override void OnEnter()
    {
        IsReadyToSwitch = false;
        
        Position2D = _positionDirectionProvider.Position2D;
        _startDistance = Position2D.magnitude; 
        _localTime = 0;
        _phase = 0;
        _startPosition = Position2D;
        _endPosition = Position2D.normalized * NEUTRAL_DISTANCE;
        
    }

    public override void Tick()
    {
        _phase  = _localTime / _duration;
        Position2D = Vector2.Lerp(_startPosition, _endPosition, _phase);
        _localTime += Time.deltaTime;
        
        if (_phase > 1)
        {
            IsReadyToSwitch = true;
        }
    }
}
