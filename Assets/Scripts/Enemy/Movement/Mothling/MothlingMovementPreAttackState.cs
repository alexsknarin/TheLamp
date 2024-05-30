using UnityEngine;

public class MothlingMovementPreAttackState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.PreAttack;
    private float _duration = .35f;
    private float _acceleratedSpeed;
    private float _acceleration = 0.93f;
    private Vector3 _direction;
    private float _localTime;
    
    public MothlingMovementPreAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _acceleratedSpeed = 1f;
        _sideDirection = sideDirection;
        _direction = currentPosition.normalized;
        Quaternion rotation = Quaternion.Euler(0, 0, 60 * _sideDirection);
        _direction = rotation * _direction;
        Position = currentPosition;
        _localTime = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * 1.0f;
        _acceleratedSpeed *= _acceleration;
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        if (_localTime > _duration)
        {
            _owner.SwitchState();
        }
    }
}