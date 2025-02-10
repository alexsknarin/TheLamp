using UnityEngine;

public class FMothMovementPatrolState: RegularEnemyMovementStateBase
{
    // Dependencies
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private readonly float _speed;
    private readonly float _radius;
    private readonly float _verticalAmplitude;
    
    // State specific attributes
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset; // TMP
    private float _phase;
    private float _mainTrajectoryAdaptTime = 0.45f;
    private float _patrolDuration;
    private float _patrolDurationMin = 0.5f;
    private float _patrolDurationMax = 1.2f;
    private float _noiseFrequency = 9f;
    private float _noiseAmplitude = 0.05f;
    private float _localTime;
    
    public FMothMovementPatrolState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        float speed,
        float radius,
        float verticalAmplitude
    )
    {
        // _cameraPosition = cameraPosition; // TODO: enable later
        _positionDirectionProvider = positionDirectionProvider;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
    }    
    
    public override void OnEnter()
    {
        IsReadyToSwitch = false;
        _phase = 0;
        _patrolDuration = Random.Range(_patrolDurationMin, _patrolDurationMax);
        Position2D = _positionDirectionProvider.Position2D;
        Vector3 horizontalVector = Vector2.right;
        _localTime = 0;
        
        _patrolStartOffsetAngle = Mathf.Acos(Vector3.Dot(horizontalVector.normalized, Position2D.normalized));
        _patrolStartOffsetAngle *= Mathf.Sign(Position2D.y);
        
        // if (_sideDirection < 0)
        // {
        //     _patrolStartOffsetAngle = Mathf.PI - _patrolStartOffsetAngle;
        // }
    }

    public override void Tick()
    {
        float trajectoryAdaptPhase = _localTime / _mainTrajectoryAdaptTime;
        _phase += Time.deltaTime * _speed;

        // Circle motion
        Vector3 circlePosition = EnemyMovementPatterns.CircleMotion(_patrolStartOffsetAngle, _radius, _radius, _verticalAmplitude, _phase);
        if (trajectoryAdaptPhase < 1)
        {
            circlePosition = Vector2.Lerp(Position2D, circlePosition, Mathf.SmoothStep(0, 1, trajectoryAdaptPhase));
        }
        
        // Add noise
        Vector3 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
        Position2D = circlePosition + trajectoryNoise * _noiseAmplitude;
        
        _localTime += Time.deltaTime;
        
        if(_localTime > _patrolDuration)
        {
            IsReadyToSwitch = true;
        }
    }
}
