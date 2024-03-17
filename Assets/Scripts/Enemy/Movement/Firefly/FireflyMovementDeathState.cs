using UnityEngine;

public class FireflyMovementDeathState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Death;
    private Vector3 _bounceForce;
    private Vector3 _gravityForce;
    private float _bounceForceMagnitude = 4f;
    private float _gravityForceMagnitude = .2f;
    private float _dragAmount = 0.94f;
    private float _deathDuration = .32f;
    private float _prevTime;
    
    public FireflyMovementDeathState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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
        _prevTime = Time.time;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _bounceForce * Time.deltaTime + _gravityForce;
        
        _bounceForce *= _dragAmount;
        _gravityForce += Vector3.down * (_gravityForceMagnitude * Time.deltaTime);
    }

    public override void ExitState()
    {
    }
    
    public override void CheckForStateChange()
    {
        float phase = (Time.time - _prevTime) / _deathDuration;
        if (phase > 1f)
        {
            _owner.SwitchState();
        }
    }
}