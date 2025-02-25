using UnityEngine;

public class FSpiderMovementEnterState: RegularEnemyMovementStateBase
{
    private readonly float _speed;
    
    private Vector2 _hangingPoint;
    private float _localTime;
    
    public FSpiderMovementEnterState(float speed, float xCenter, float height)
    {
        _speed = speed;
        _hangingPoint.x = xCenter;
        _hangingPoint.y = height;
    }
    
    public override void OnEnter()
    {
        IsReadyToSwitch = false;
        Position2D = _hangingPoint; 
        _localTime = 0;
    }

    public override void Tick()
    {
        Vector2 newPosition = Position2D;
        float phase = Mathf.Pow(Position2D.y / 5f, 0.5f);
        newPosition.y = Position2D.y - _speed * phase * Time.deltaTime;
        
        float swing = Mathf.Sin(_localTime * 4.1f) * 0.1f * phase + _hangingPoint.x;
        newPosition.x = swing; 

        Position2D = newPosition;
        _localTime += Time.deltaTime;
        
        if(Position2D.y < 0.005f)
        {
            IsReadyToSwitch = true;
        }
    }
}
