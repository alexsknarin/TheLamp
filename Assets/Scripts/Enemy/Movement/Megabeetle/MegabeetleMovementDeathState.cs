using UnityEngine;

public class MegabeetleMovementDeathState: EnemyMovementBaseState
{
    private Vector3 _bounceForce;
    private Vector3 _gravityForce;
    private float _duration = 1.7f;
    private float _localTime = 0f;
    private float _phase = 0f;
    private float _bounceForceMagnitude = 3f;
    private float _gravityForceMagnitude = .17f;
    private float _dragAmount = 0.9f;
    private float _gravityDecreaseAmount = 0.2f;
    private Vector3 IDLE_POSITION = new Vector3(0f, -4.5f, 0f); 
    
    public MegabeetleMovementDeathState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    public override EnemyStates State => EnemyStates.Death;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _bounceForce = currentPosition.normalized * _bounceForceMagnitude;
        _gravityForce = Vector3.zero;
        _localTime = 0f;
        _phase = 0f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        _phase = _localTime / _duration;
        Position = currentPosition + (_bounceForce * Time.deltaTime + _gravityForce) * Mathf.Pow(Mathf.Clamp01(1 - _phase), 2f);
        _bounceForce = _bounceForce * _dragAmount;
        _gravityForce = _gravityForce + Vector3.down * (_gravityForceMagnitude * Time.deltaTime);
        
        _localTime += Time.deltaTime;
        
        if (_phase > 1f)
        {
            Position = IDLE_POSITION;
        }
    }
    
    public override void CheckForStateChange()
    {
        if (_phase > 1f)
        {
            _owner.SwitchState();
        }
    }
  
}