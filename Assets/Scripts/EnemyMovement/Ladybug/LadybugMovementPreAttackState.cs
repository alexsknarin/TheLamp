using UnityEngine;

public class LadybugMovementPreAttackState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.PreAttack;
    
    private float _duration = .30f;
    private float _acceleratedSpeed;
    private float _acceleration = 0.93f;
    private Vector3 _direction;
    private Vector3 _tangentDirection;
    private Vector3 _endPos;
    private float _prevTime;
    
    public LadybugMovementPreAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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
        Quaternion rotation = Quaternion.Euler(0, 0, 90 * _sideDirection);
        _tangentDirection = rotation * _direction;
        
        Position = currentPosition;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 direction;
        float phase = (Time.time - _prevTime) / _duration;
        if (phase < 0.5f)
        {
            direction = Vector3.Lerp(_tangentDirection, _direction, phase * 2).normalized;
        }
        else
        {
            direction = Vector3.Lerp(_direction, -_tangentDirection , (phase - 0.5f) * 2).normalized;
        }
        
        Position = currentPosition + direction * (_speed * Time.deltaTime * (Mathf.PI/2) * _acceleratedSpeed);
        
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * 0.1f;
        _acceleratedSpeed *= _acceleration;
    }
    
    public override void CheckForStateChange()
    {
        if (Time.time - _prevTime > _duration)
        {
            _owner.SwitchState();
        }
    }

    public void ExitState()
    {
    }
}