using System;
using UnityEngine;

public class FMegabeetleMovementFallState : RegularEnemyMovementStateBase
{
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    
    private Vector2 _bounceForce;
    private Vector2 _gravityForce;
    private float _bounceForceMagnitude = 3f;
    private float _gravityForceMagnitude = .17f;
    private float _dragAmount = 0.9f;
    
    public FMegabeetleMovementFallState(IPositionDirectionProvider positionDirectionProvider)
    {
        _positionDirectionProvider = positionDirectionProvider;
    }

    public event Action Ended;
    
    public override void OnEnter()
    {
        IsReadyToSwitch = false;
        Position2D = _positionDirectionProvider.Position2D;
        _bounceForce = Position2D.normalized * _bounceForceMagnitude;
        _gravityForce = Vector3.zero;
    }

    public override void Tick()
    {
        Position2D += _bounceForce * Time.deltaTime + _gravityForce;
        _bounceForce *= _dragAmount;
        _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
        if (Position2D.y < -7.3f)
        {
            Ended?.Invoke();
            IsReadyToSwitch = true;
        }
    }
}
