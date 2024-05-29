using UnityEngine;

public class SpiderMovementReturnState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Return;
    private Vector3 _hangingPoint;
    private float _localTime;
    private float _initialAmplitude;
    private float _swingAmplitude;
    private float _initialXpos;
    private int _returnPhase;
    private Vector3 _initialDirection;
    private float _decceleration = 0.05f;
    
    public SpiderMovementReturnState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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
        _initialAmplitude = Mathf.Abs(Mathf.Abs(currentPosition.x) - Mathf.Abs(_hangingPoint.x));
        _swingAmplitude = _initialAmplitude;
        _initialDirection = currentPosition.normalized;
        _initialDirection.y = 0;
        _initialDirection.z = 0;
        _initialXpos = currentPosition.x;
        _returnPhase = 0;
        _localTime = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        if (_returnPhase == 0)
        {
            Vector3 newPosition = currentPosition;
            float phase = 1 - Mathf.Abs(currentPosition.x - _initialXpos) / (_initialAmplitude * 2);
            phase = Mathf.Pow(phase, 0.5f);
            newPosition += _initialDirection * (_speed * phase * Time.deltaTime);
            newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint;
            Position = newPosition;
            
            if(Mathf.Abs(Position.x - _initialXpos) > (_initialAmplitude * 2 - 0.05f))
            {
                _returnPhase = 1;
                _localTime = 0;
                _swingAmplitude = _initialAmplitude - 0.05f;
            }
        }
        if (_returnPhase == 1)
        {
            float swing = _sideDirection * Mathf.Cos(_localTime * 1.5f) * _swingAmplitude + _hangingPoint.x;
            Vector3 newPosition = currentPosition;
            
            newPosition.x = swing;
            newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint;
            Position = newPosition;
            
            _localTime += Time.deltaTime;
            _swingAmplitude -= _decceleration * Time.deltaTime;
        }
    }
    
    public override void CheckForStateChange()
    {
        if(_swingAmplitude < 0)
        {
            _owner.SwitchState();
        }
    }
}