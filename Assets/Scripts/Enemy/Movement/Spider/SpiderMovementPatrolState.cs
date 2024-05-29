using UnityEngine;

public class SpiderMovementPatrolState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Patrol;
    private Vector3 _hangingPoint; 
    private float _localTime;

    public SpiderMovementPatrolState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
        
        _hangingPoint = Vector3.up * 5f;
        _hangingPoint.x = radius;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _hangingPoint.x = Mathf.Abs(_hangingPoint.x) * sideDirection;
        Position = currentPosition;
        _localTime = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        float swing = Mathf.Sin(_localTime) * 0.06f + _hangingPoint.x;
        
        Vector3 newPosition = currentPosition;
        newPosition.x = swing;
        newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint;
        
        Position = newPosition;
        _localTime += Time.deltaTime;
    }
}