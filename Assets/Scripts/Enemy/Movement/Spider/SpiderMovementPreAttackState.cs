using UnityEngine;

public class SpiderMovementPreAttackState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.PreAttack;
    private Vector3 _hangingPoint;
    private float _localTime;
    private float _acceleration = 9.7f;
    private float _acceleratedSpeed;
    private float _sideDirection;
    private float _lastXPosition;
    private float _swingPhase;
    private float _prevSwingPhase;

    public SpiderMovementPreAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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
        newPosition.x = _hangingPoint.x;
        _lastXPosition = currentPosition.x;
        Position = newPosition;
        _localTime = 0;
        _acceleratedSpeed = 1f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        _prevSwingPhase = _swingPhase;
        _swingPhase = Mathf.Sin(_localTime * 4.2f);
        float swing = -_sideDirection * _swingPhase * 0.036f * _acceleratedSpeed + _lastXPosition;
        
        Vector3 newPosition = currentPosition;
        newPosition.x = swing;
        newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint;
        
        Position = newPosition;
        
        _localTime += Time.deltaTime;
        _acceleratedSpeed += _acceleration * Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        if(_swingPhase < 0 && _swingPhase > _prevSwingPhase)
        {
            _owner.SwitchState();
        }
    }
}