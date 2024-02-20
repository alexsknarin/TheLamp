using UnityEngine;

public class LadybugMovementPatrolState: EnemyMovementBaseState
{
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset; // TMP
    private float _phase;
    private float _spiralSpeedStart = 0.13f;
    private float _spiralSpeedEnd = 0.009f;
    private float _spiralPhase = 1f;
    
    private float _spiralAcceleration = .25f;
    private float _spiralAccelerationPhase = 1f;
    private float _preAttackTriggerDistance = 0.7f;
    private float _preAttackTriggerYThreshold = 0.3f;

    public LadybugMovementPatrolState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override EnemyStates State => EnemyStates.Patrol;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _depthDirection = depthDirection;

        _phase = 0;
        _spiralPhase = 1f;
        _spiralAccelerationPhase = 1f;
        Position = currentPosition;
        
        Vector3 horizontalVector = Vector3.right;
        horizontalVector.x *= _sideDirection;
        _patrolStartOffsetAngle = Mathf.Acos(Vector3.Dot(horizontalVector.normalized, currentPosition.normalized));
        _patrolStartOffsetAngle *= Mathf.Sign(currentPosition.y);
        if (_sideDirection < 0)
        {
            _patrolStartOffsetAngle = Mathf.PI - _patrolStartOffsetAngle;
        }
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        float speedCompenstation = (1 - currentPosition.magnitude/_radius) + 1;
       
        _phase += Time.deltaTime * _speed * speedCompenstation * _sideDirection;
        Vector3 ellipsePosition = EnemyMovementPatterns.CircleMotion(_patrolStartOffsetAngle, _radius, _radius, _verticalAmplitude, _phase);
        ellipsePosition *= _spiralPhase;

        if (ellipsePosition.magnitude > _preAttackTriggerDistance)
        {
            Vector3 circlePosition = ellipsePosition;
            circlePosition.y /= _verticalAmplitude;
            _spiralPhase -= Mathf.Lerp(_spiralSpeedStart, _spiralSpeedEnd, 1- (circlePosition.magnitude/_radius)) * Time.deltaTime;
        }

        Position = ellipsePosition;
    }
    
    public override void CheckForStateChange()
    {
        if(Position.magnitude < _preAttackTriggerDistance && Position.y < _preAttackTriggerYThreshold)
        {
            _owner.SwitchState();
        }
    }

    public void ExitState()
    {

    }
}