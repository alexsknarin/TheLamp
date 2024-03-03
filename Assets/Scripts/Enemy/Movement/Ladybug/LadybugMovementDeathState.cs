using Unity.VisualScripting;
using UnityEngine;

public class LadybugMovementDeathState: EnemyMovementBaseState
{
    private Vector3 _bounceForce;
    private Vector3 _gravityForce;
    private float _bounceForceMagnitude = 3f;
    private float _gravityForceMagnitude = .17f;
    private float _dragAmount = 0.9f;
    
    public LadybugMovementDeathState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _bounceForce * Time.deltaTime + _gravityForce;
        
        _bounceForce = _bounceForce * _dragAmount;
        _gravityForce = _gravityForce + Vector3.down * (_gravityForceMagnitude * Time.deltaTime);
    }

    public override void ExitState()
    {
    }
    
    public override void CheckForStateChange()
    {
        if (Position.y < -4f)
        {
            _owner.SwitchState();
        }
    }
  
}