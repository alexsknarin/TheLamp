using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovementFallState: EnemyMovementBaseState
{
    private Vector3 _bounceForce;
    private Vector3 _gravityForce;
    private float _bounceForceMagnitude = 4f;
    private float _gravityForceMagnitude = .1f;
    private float _dragAmount = 0.94f;
    
    public EnemyMovementFallState(EnemyBaseMovement owner, int sideDirection, float speed, float radius, float verticalAmplitude) : base()
    {
        _sideDirection = sideDirection;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    public override EnemyStates State
    {
        get => EnemyStates.Fall;
        protected set  { }
    }

    public override void EnterState(Vector3 currentPosition)
    {
        _bounceForce = currentPosition.normalized * _bounceForceMagnitude;
        _gravityForce = Vector3.zero;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _bounceForce * Time.deltaTime + _gravityForce;
        
        _bounceForce = _bounceForce * _dragAmount;
        _gravityForce = _gravityForce + Vector3.down * (_gravityForceMagnitude * Time.deltaTime);
    }

    public void ExitState()
    {
    }
    
    public override void CheckForStateChange()
    {
        if (Position.y < -_radius * _verticalAmplitude * 1.1)
        {
            _owner.SwitchState();
        }
    }
  
}