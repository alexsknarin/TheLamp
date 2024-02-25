    using UnityEngine;

public class FlyMovementEnterState: EnemyMovementBaseState
{
    private Vector3 _endPos = Vector3.zero;
    private Vector3 _enterDirection;
    private float _depthMultiplier = 2f;
    private float _initialDistance;

    public FlyMovementEnterState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    } 

    public override EnemyStates State => EnemyStates.Enter;
   
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _depthDirection = depthDirection;
        
        float xProjectionLength = Mathf.Abs(currentPosition.x);
        float enterDirectionLength = Vector3.Magnitude(currentPosition);
        float r = _radius * _verticalAmplitude;
        
        float patrolStartOffsetAngle = Mathf.PI - Mathf.Acos(r / enterDirectionLength) - Mathf.Acos(xProjectionLength / enterDirectionLength);
        
        _endPos.x = Mathf.Cos(-patrolStartOffsetAngle) * _sideDirection;
        _endPos.y = Mathf.Sin(-patrolStartOffsetAngle);
        _endPos = _endPos.normalized * r;

        _enterDirection = (_endPos - currentPosition);
        _initialDistance = _enterDirection.magnitude;
        _enterDirection = _enterDirection.normalized;
        
        
        Position = currentPosition;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _enterDirection * (_speed * Time.deltaTime * (Mathf.PI/2));
        
        // Depth To Camera
        float distancePhase = 1 - (_endPos - Position).magnitude / _initialDistance;
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * (_depthDirection * Mathf.Lerp(Position.y * _depthMultiplier, Position.y, distancePhase));
    }
    
    public override void CheckForStateChange()
    {
        if(Position.x*_sideDirection > Mathf.Abs(_endPos.x))
        {
            _owner.SwitchState();
        }
    }

    public override void ExitState()
    {
    }
    


}