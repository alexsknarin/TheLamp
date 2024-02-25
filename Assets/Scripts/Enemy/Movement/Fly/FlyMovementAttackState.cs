using UnityEngine;

public class FlyMovementAttackState: EnemyMovementBaseState
{
    private float _acceleration = 0.06f;
    private float _depthDecrement = 0.2f;
    private float _acceleratedSpeed = 1f;
    public FlyMovementAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    public override EnemyStates State => EnemyStates.Attack;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _acceleratedSpeed = 1f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 newPosition = currentPosition;
        Vector3 direction = -newPosition.normalized;
        newPosition += direction * (_speed * _acceleratedSpeed * Time.deltaTime);
        _acceleratedSpeed += _acceleration;
        Position = newPosition;
        
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * (2.5f * _depthDecrement);
        Debug.Log(State);
    }

    public override void ExitState()
    {
    }
}
