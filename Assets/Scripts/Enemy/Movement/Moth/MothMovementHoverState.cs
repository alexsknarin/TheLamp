using UnityEngine;

public class MothMovementHoverState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Hover;
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset; // TMP
    private float _phase;
    private float _depthMultiplier = 0.6f;
    private float _hoverRadius = .35f;
    private Vector3 _hoverCenter;
    private float _speedFactor;
    private float _moveFromCenterDuration = 0.2f;
    private float _speedNoiseCompensation = 0.4f;
    private float _hoverDuration;
    private float _hoverPhase;
    private float _noiseFrequency = 8f;
    private float _noiseAmplitude = 0.29f;
    private float _localTime;

    public MothMovementHoverState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
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
        _hoverDuration = Random.Range(.5f, 2f);
        _phase = 0;
        Position = currentPosition;
        _hoverCenter = currentPosition;
        _speedFactor = _radius / _hoverRadius;
        _localTime = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        float radiusAdaptPhase = _localTime / _moveFromCenterDuration;
        float speedNoiseCompensation = Mathf.Lerp(1, _speedNoiseCompensation, radiusAdaptPhase);
        _phase += Time.deltaTime * _speed * _sideDirection * _speedFactor * speedNoiseCompensation;
        _hoverPhase = _localTime / _hoverDuration;

        Vector3 circlePosition = _hoverCenter + EnemyMovementPatterns.CircleMotion(0, _hoverRadius, _hoverRadius, 1, _phase);
        
        if (radiusAdaptPhase < 1f)
        {
            circlePosition = Vector3.Lerp(_hoverCenter, circlePosition, Mathf.SmoothStep(0, 1, radiusAdaptPhase));
        }
        
        // Add noise
        Vector3 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
        Position = circlePosition + trajectoryNoise * (Mathf.Clamp(radiusAdaptPhase, 0, 1) * _noiseAmplitude);
        
        // Depth To Camera
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * _depthMultiplier;
        
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        if(_hoverPhase > 1f)
        {
            _owner.SwitchState();
        }
    }
}