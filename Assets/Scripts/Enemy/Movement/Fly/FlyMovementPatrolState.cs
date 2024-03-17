using UnityEngine;

public class FlyMovementPatrolState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Patrol;
    private float _verticalAdaptDuration = 2f; // TODO: Serialized Field
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset; // TMP
    private float _depthMultiplier = 1f;
    private float _phase;
    private float _localTime;
    
    public FlyMovementPatrolState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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
        Vector3 horizontalVector = Vector3.right;
        horizontalVector.x *= _sideDirection;
        _patrolStartOffsetAngle = Mathf.Acos(Vector3.Dot(horizontalVector.normalized, currentPosition.normalized));
        _phase = 0;
        _localTime = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        // Adapt Radius
        float radiusAdaptPhase = _localTime / _verticalAdaptDuration;
        float finalXRadius = _radius;
        if (radiusAdaptPhase < 1f)
        {
            finalXRadius = Mathf.Lerp(_radius * _verticalAmplitude, _radius,  Mathf.SmoothStep(0, 1, radiusAdaptPhase));
        }
        
        // Circle motion
        _phase += Time.deltaTime * _speed * _sideDirection;
        
        float offsetAngleWithDirection;
        if (_sideDirection > 0)
        {
            offsetAngleWithDirection = -_patrolStartOffsetAngle;
        }
        else
        {
            offsetAngleWithDirection = _patrolStartOffsetAngle-Mathf.PI;
        }
        
        Vector3 circlePosition = EnemyMovementPatterns.CircleMotion(offsetAngleWithDirection, finalXRadius, _radius, _verticalAmplitude, _phase);
        Position = circlePosition;
        
        // Depth To Camera
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * (_depthDirection * Position.y * _depthMultiplier);
        
        _localTime += Time.deltaTime;
    }
    
    public override void ExitState()
    {
        _localTime = 0;
        _patrolStartOffsetAngle = 0;
    }
}