using UnityEngine;

public class MegabeetleMovementPatrolState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Patrol;
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset;
    private float _phase;
    private float _spiralSpeedStart = 0.2f;
    private float _spiralSpeedEnd = 0.015f;
    private float _spiralPhase = 1f;
    private float _preAttackTriggerDistance = 1.0f;
    private float _preAttackTriggerYThreshold = 0.3f;
    private float _depthMultiplierMax = 3.5f;
    private float _depthMultiplierMin = 0f;

    public MegabeetleMovementPatrolState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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

        _phase = 0;
        _spiralPhase = 1f;
        Vector3 randomizedPosition = currentPosition;
        randomizedPosition.x *= sideDirection;
        Position = randomizedPosition;
        
        Vector3 horizontalVector = Vector3.right;
        horizontalVector.x *= _sideDirection;
        _patrolStartOffsetAngle = Mathf.Acos(Vector3.Dot(horizontalVector.normalized, currentPosition.normalized));
        _patrolStartOffsetAngle *= Mathf.Sign(currentPosition.y);
        
        // if (_sideDirection < 0)
        // {
        //     _patrolStartOffsetAngle = Mathf.PI - _patrolStartOffsetAngle;
        // }
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        float speedCompenstation = (1 - currentPosition.magnitude/_radius) + 1;
       
        _phase += Time.deltaTime * _speed * speedCompenstation * _sideDirection;
        Vector3 ellipsePosition = EnemyMovementPatterns.CircleMotion(_patrolStartOffsetAngle, _radius, _radius, _verticalAmplitude, _phase);
        ellipsePosition *= _spiralPhase;

        Vector3 circlePosition = ellipsePosition;
        circlePosition.y /= _verticalAmplitude;
        
        if (ellipsePosition.magnitude > _preAttackTriggerDistance)
        {
            _spiralPhase -= Mathf.Lerp(_spiralSpeedStart, _spiralSpeedEnd, 1 - (circlePosition.magnitude/_radius)) * Time.deltaTime;
        }

        Position = ellipsePosition;
        
        // Depth To Camera
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        float depthPhase = Mathf.Clamp(circlePosition.magnitude - _preAttackTriggerDistance, 0.0001f, _radius) / (_radius - _preAttackTriggerDistance);
        depthPhase = Mathf.Pow(depthPhase, 0.85f);
        depthPhase = Mathf.Clamp(depthPhase, 0.0001f, 1f);
        float depthValue = Mathf.Lerp(_depthMultiplierMin, _depthMultiplierMax, depthPhase);
        Depth = cameraDirection * depthValue;
    }
    
    public override void CheckForStateChange()
    {
        if(Position.magnitude < _preAttackTriggerDistance && Position.y < _preAttackTriggerYThreshold)
        {
            _owner.SwitchState();
        }
    }
}