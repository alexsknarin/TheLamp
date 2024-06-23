using UnityEngine;

public class FlyMovementAttackState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Attack;
    private float _acceleration = 13.5f;
    private float _depthDecrement = 0.4f;
    private float _acceleratedSpeed = 1f;
    private float _startDistance;
    
    public FlyMovementAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _acceleratedSpeed = 1f;
        _startDistance = currentPosition.magnitude - 0.65f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 newPosition = currentPosition;
        Vector3 direction = -newPosition.normalized;
        newPosition += direction * (_speed * _acceleratedSpeed * Time.deltaTime);
        _acceleratedSpeed += _acceleration * Time.deltaTime;
        Position = newPosition;
        
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        
        float attackProximityGradient = Mathf.Clamp((Position.magnitude - 0.65f) / _startDistance, 0.0f, 1.0f);
        
        attackProximityGradient = Mathf.Pow(attackProximityGradient, 2.0f) + 0.11f; 
        
        Depth = cameraDirection * (2.5f * _depthDecrement * attackProximityGradient);
    }
}
