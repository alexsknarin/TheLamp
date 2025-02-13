using System;
using UnityEngine;

public class FSpiderMovementPatrolState: RegularEnemyMovementStateBase
{
    private IPositionDirectionProvider _positionDirectionProvider;
    private Vector2 _hangingPoint;
    private float _localTime;
    
    public FSpiderMovementPatrolState(IPositionDirectionProvider positionDirectionProvider, float xCenter, float height)
    {
        _positionDirectionProvider = positionDirectionProvider;
        _hangingPoint.x = xCenter;
        _hangingPoint.y = height;
    }
    
    public event Action Started;
    public event Action Ended;
   
    public override void OnEnter()
    {
        _hangingPoint.x = Mathf.Abs(_hangingPoint.x);
        Position2D = _positionDirectionProvider.Position2D;
        _localTime = 0;
        Started?.Invoke();
    }

    public override void Tick()
    {
        float swing = Mathf.Sin(_localTime) * 0.06f + _hangingPoint.x;
        
        Vector2 newPosition = Position2D;
        newPosition.x = swing;
        newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint;
        
        Position2D = newPosition;
        _localTime += Time.deltaTime;
    }
    
    public override void OnExit()
    {
        Ended?.Invoke();
    }
}
