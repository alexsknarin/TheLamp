using UnityEngine;

public class MothMovementPatrolState: MothMovementBaseState
{
    private float _prevTime;
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset; // TMP
    private float _phase;
    private float _depthMultiplier = 1f;
    private float _mainTrajectoryAdaptTime = 0.45f;
    
    private float _patrolDuration;
    
    public MothMovementPatrolState(MothMovement owner, float speed, float radius, float verticalAmplitude) : base()
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
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        float trajectoryAdaptPhase = (Time.time - _prevTime) / _mainTrajectoryAdaptTime;
        
        float offsetAngleWithDirection;
        if (_sideDirection > 0)
        {
            offsetAngleWithDirection = -_patrolStartOffsetAngle;
        }
        else
        {
            offsetAngleWithDirection = _patrolStartOffsetAngle-Mathf.PI;
        }
        
        _phase += Time.deltaTime * _speed * _sideDirection;
        Vector3 newPosition = Vector3.zero;
        newPosition.x = Mathf.Cos(_phase + offsetAngleWithDirection) * _radius;
        newPosition.y = Mathf.Sin(_phase + offsetAngleWithDirection) * _radius * _verticalAmplitude;

        if (trajectoryAdaptPhase < 1)
        {
            newPosition = Vector3.Lerp(currentPosition, newPosition, Mathf.SmoothStep(0, 1, trajectoryAdaptPhase));
        }
        
        Position = newPosition;
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