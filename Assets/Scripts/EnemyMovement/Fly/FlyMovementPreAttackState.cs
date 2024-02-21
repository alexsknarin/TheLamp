using UnityEngine;

public class FlyMovementPreAttackState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.PreAttack;
    
    private float _duration = .35f;
    private float _acceleratedSpeed;
    private float _acceleration = 0.93f;
    private Vector3 _direction;
    private float _prevTime;
    
    public FlyMovementPreAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _prevTime = Time.time;
        _acceleratedSpeed = 1f;
        _sideDirection = sideDirection;
        _direction = currentPosition.normalized;
        Quaternion rotation = Quaternion.Euler(0, 0, 60 * _sideDirection);
        _direction = rotation * _direction;
        Position = currentPosition;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _direction * (_speed * Time.deltaTime * (Mathf.PI/2) * _acceleratedSpeed);
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * 2.5f;
        _acceleratedSpeed *= _acceleration;
    }
    
    public override void CheckForStateChange()
    {
        if (Time.time - _prevTime > _duration)
        {
            _owner.SwitchState();
        }
    }

    public override void ExitState()
    {
    }
}