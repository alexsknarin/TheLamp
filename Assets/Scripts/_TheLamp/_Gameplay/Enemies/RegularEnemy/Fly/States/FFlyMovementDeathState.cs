using System;
using UnityEngine;

public class FFlyMovementDeathState : FFlyMovementStateBase
{
    private IPositionDirectionProvider _positionDirectionProvider;
    private bool _isDeathByTimer;
    
    // State specific attributes
    private Vector2 _bounceForce;
    private Vector2 _gravityForce;
    private readonly float _bounceForceMagnitude = 4f;
    private readonly float _gravityForceMagnitude = .2f;
    private readonly float _dragAmount = 0.94f;
    private readonly float _speedMultiplier = 0.8f;
    private readonly float _fallBottomYcoordinate = -6f;
    private float _duration = 0.32f;
    private float _localTime;
    
    
    public FFlyMovementDeathState(IPositionDirectionProvider positionDirectionProvider, bool isDeathByTimer)
    {
        _positionDirectionProvider = positionDirectionProvider;
        _isDeathByTimer = isDeathByTimer;
    }

    public event Action Ended;
    
    public override void OnEnter()
    {
        Position2D = _positionDirectionProvider.Position2D;
        DepthDirection = Vector3.zero;
        
        _bounceForce = Position2D.normalized * _bounceForceMagnitude;
        _gravityForce = Vector2.zero;
        IsReadyToSwitch = false;
        
        _localTime = 0;
    }

    public override void Tick()
    {
        Position2D += _bounceForce * (Time.deltaTime * _speedMultiplier) + _gravityForce;
        _bounceForce *= _dragAmount;
        _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);

        if (_isDeathByTimer)
        {
            if (_localTime >= _duration)
                Ended?.Invoke();
            _localTime += Time.deltaTime;
        }
        else 
        {
            if (Position2D.y < _fallBottomYcoordinate)
                Ended?.Invoke();
        }
    }
}
