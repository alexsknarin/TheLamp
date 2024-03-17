using UnityEngine;

public class SpiderMovementAttackState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Attack;
    private Vector3 _hangingPoint;
    private float _localTime;
    private float _acceleration = 8.7f;
    private float _acceleratedSpeed;
    private float _sideDirection;

    public SpiderMovementAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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
        Vector3 newPosition = currentPosition;
        Position = newPosition;
        _acceleratedSpeed = 1f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 newPosition = currentPosition;
        Vector3 direction = -Vector3.right * _sideDirection;
        newPosition += direction * (_speed * _acceleratedSpeed * Time.deltaTime);
        
        newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint;
        
        _acceleratedSpeed += _acceleration*Time.deltaTime;
        Position = newPosition;
    }
}