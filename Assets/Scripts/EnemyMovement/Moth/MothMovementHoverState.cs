using UnityEngine;

public class MothMovementHoverState: MothMovementBaseState
{
    private float _prevTime;
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset; // TMP
    private float _phase;
    private float _depthMultiplier = 1.8f;
    private float _hoverRadius = .35f;
    private Vector3 _hoverCenter;
    private float _speedFactor;
    private float _moveFromCenterDuration = 0.2f;
    private float _speedNoiseCompensation = 0.4f;

    private float _hoverDuration;
    private float _hoverPhase;
    
    public MothMovementHoverState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override EnemyStates State => EnemyStates.Hover;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _depthDirection = depthDirection;
        _prevTime = Time.time;
        _hoverDuration = Random.Range(.5f, 2f);
        _phase = 0;
        Position = currentPosition;
        _hoverCenter = currentPosition;
        _speedFactor = _radius / _hoverRadius;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        float radiusAdaptPhase = (Time.time - _prevTime) / _moveFromCenterDuration;
        float speedNoiseCompensation = Mathf.Lerp(1, _speedNoiseCompensation, radiusAdaptPhase);
        _phase += Time.deltaTime * _speed * _sideDirection * _speedFactor * speedNoiseCompensation;
        _hoverPhase = (Time.time - _prevTime) / _hoverDuration;

        Vector3 newPosition = Vector3.zero;
        newPosition.x = Mathf.Cos(_phase) * _hoverRadius;
        newPosition.y = Mathf.Sin(_phase) * _hoverRadius;
        
        newPosition += _hoverCenter;
        
        if (radiusAdaptPhase < 1f)
        {
            newPosition = Vector3.Lerp(_hoverCenter, newPosition, Mathf.SmoothStep(0, 1, radiusAdaptPhase));
        }
        
        // Add noise
        Vector3 noiseValue = Vector3.zero;
        noiseValue.x = Mathf.PerlinNoise(8f * Time.time, 0) * 2 - 1;
        noiseValue.y = Mathf.PerlinNoise( 0, 8f * Time.time) * 2 - 1;
        
        
        Position = newPosition + noiseValue * (Mathf.Clamp(radiusAdaptPhase, 0, 1) * 0.29f);
        
        
        // Depth To Camera
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * _depthMultiplier;


    }
    
    public override void CheckForStateChange()
    {
        if(_hoverPhase > 1f)
        {
            _owner.SwitchState();
        }
    }

    public void ExitState()
    {

    }
}