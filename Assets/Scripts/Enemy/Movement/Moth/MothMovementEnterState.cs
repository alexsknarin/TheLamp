// https://mathworld.wolfram.com/Ellipse-LineIntersection.html
using UnityEngine;

public class MothMovementEnterState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Enter;
    private Vector3 _endPos = Vector3.zero;
    private Vector3 _enterDirection;
    private float _depthMultiplier = 1.6f;
    private float _initialDistance;
    private float _phase;

    public MothMovementEnterState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    } 

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _depthDirection = depthDirection;
        Position = currentPosition;
        
        // Find intersection to the ellipse
        float a = _radius;
        float b = _radius * _verticalAmplitude;

        float denominator = ((a * b) / (Mathf.Sqrt((a * a) * (Position.y * Position.y) + (b * b) * (Position.x * Position.x)))); 
        _endPos = Vector3.zero;
        _endPos.x = denominator * Position.x;
        _endPos.y = denominator * Position.y;
       
        _enterDirection = (Vector3.zero - currentPosition).normalized;
        _initialDistance = (_endPos - Position).magnitude;
        _phase = 1;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _enterDirection * (_speed * Time.deltaTime * (Mathf.PI/2));
        _phase = (_endPos - Position).magnitude / _initialDistance;

        // Depth To Camera
        float distancePhase = 1 - (_endPos - Position).magnitude / _initialDistance;
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * (_depthDirection * Mathf.Lerp(Position.y * _depthMultiplier, Position.y, distancePhase));
    }
    
    public override void CheckForStateChange()
    {
        if(_phase < 0.02f || Position.magnitude < _radius)
        {
            _owner.SwitchState();
        }
    }
}