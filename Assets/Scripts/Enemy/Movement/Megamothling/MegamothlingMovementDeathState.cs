using UnityEngine;

public class MegamothlingMovementDeathState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Death;
    private Vector3 _bounceForce;
    private Vector3 _gravityForce;
    private float _bounceForceMagnitude = 2f;
    private float _gravityForceMagnitude = .2f;
    private float _dragAmount = 0.94f;
    private float _duration = 1.7f;
    private float _localTime;
    
    public MegamothlingMovementDeathState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _bounceForce = currentPosition.normalized * _bounceForceMagnitude;
        _gravityForce = Vector3.zero;
        _dragAmount = 0.94f;
        _localTime = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _bounceForce * Time.deltaTime + _gravityForce;
        
        _bounceForce = _bounceForce * _dragAmount;
        _dragAmount -= 0.12f * Time.deltaTime;
        if (_dragAmount < 0.0f)
        {
            _dragAmount = 0.0001f;
        }
        
        _localTime += Time.deltaTime;
        _gravityForce = _gravityForce * _dragAmount + Vector3.down * (_gravityForceMagnitude * _dragAmount * Time.deltaTime);
    }
  
    public override void CheckForStateChange()
    {
        if (_localTime > _duration)
        {
            _owner.SwitchState();
        }
    }
  
}