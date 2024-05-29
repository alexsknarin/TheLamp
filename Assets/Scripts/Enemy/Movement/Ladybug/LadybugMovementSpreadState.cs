using UnityEngine;

public class LadybugMovementSpreadState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Spread;
    private float _maxDistance = 6.4f;
    private float _acceleratedSpeed;
    private float _acceleration = 3.5f;
    private Vector3 _direction;
    private float _extraDistance;
    
    public LadybugMovementSpreadState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        Position = currentPosition;
        _direction = currentPosition.normalized;
        _acceleratedSpeed = 1;
        _extraDistance = Random.Range(45f, 65f);
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _direction * (_speed * _acceleratedSpeed * Time.deltaTime);
        _acceleratedSpeed += _acceleration * Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        if(Position.magnitude > _maxDistance+_extraDistance)
        {
            _owner.SwitchState();
        }
    }

    public override void ExitState()
    {
    }
}