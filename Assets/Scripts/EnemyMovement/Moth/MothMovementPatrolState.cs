using UnityEngine;

public class MothMovementPatrolState: EnemyMovementBaseState
{
    private float _prevTime;
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset; // TMP
    private float _phase;
    private float _depthMultiplier = 1f;
    private float _mainTrajectoryAdaptTime = 0.45f;
    
    private float _patrolDuration;
    private float _noiseFrequency = 9f;
    private float _noiseAmplitude = 0.05f;

    public MothMovementPatrolState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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
        _prevTime = Time.time;
        _phase = 0;
        _patrolDuration = Random.Range(.5f, 1.2f);
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
        float trajectoryAdaptPhase = (Time.time - _prevTime) / _mainTrajectoryAdaptTime;
        _phase += Time.deltaTime * _speed * _sideDirection;

        // Circle motion
        Vector3 circlePosition = EnemyMovementPatterns.CircleMotion(_patrolStartOffsetAngle, _radius, _radius, _verticalAmplitude, _phase);
        
        if (trajectoryAdaptPhase < 1)
        {
            circlePosition = Vector3.Lerp(currentPosition, circlePosition, Mathf.SmoothStep(0, 1, trajectoryAdaptPhase));
        }
        
        // Add noise
        Vector3 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
        Position = circlePosition + trajectoryNoise * _noiseAmplitude;
    }
    
    public override void CheckForStateChange()
    {
        if(Time.time - _prevTime > _patrolDuration)
        {
            _owner.SwitchState();
        }
    }

    public void ExitState()
    {

    }
}