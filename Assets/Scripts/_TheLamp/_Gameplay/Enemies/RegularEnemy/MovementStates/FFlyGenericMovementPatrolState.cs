using System;
using UnityEngine;

public class FFlyGenericMovementPatrolState: RegularEnemyMovementStateBase
{
    // Dependencies

    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f); // DI?
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private ILampPositionProviderService _lampPositionProviderService; // TODO: enable later
    private readonly float _speed;
    private readonly float _radius;
    private readonly float _verticalAmplitude;

    // Local variables
    private readonly float _verticalAdaptDuration = 2f;
    private readonly float _depthMultiplier = 1f;
    private float _patrolStartOffsetAngle;
    private float _phase;
    private float _localTime;
    
    public FFlyGenericMovementPatrolState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        ILampPositionProviderService lampPositionProviderService,
        float speed,
        float radius,
        float verticalAmplitude)
    {
        // _cameraPosition = cameraPosition; // TODO: enable later
        _positionDirectionProvider = positionDirectionProvider;
        _lampPositionProviderService = lampPositionProviderService;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
    }

    public event Action Started;
    public event Action Ended;
    
    public override void OnEnter()
    {
        Position2D = _positionDirectionProvider.Position2D;
        Vector3 horizontalVector = Vector2.right;
        _patrolStartOffsetAngle = Mathf.Acos(Vector3.Dot(horizontalVector.normalized, Position2D.normalized));
        _phase = 0;
        _localTime = 0;
        Started?.Invoke();
    }

    public override void Tick()
    {
        // Adapt Radius
        float radiusAdaptPhase = _localTime / _verticalAdaptDuration;
        float finalXRadius = _radius;
        if (radiusAdaptPhase < 1f)
        {
            finalXRadius = Mathf.Lerp(_radius * _verticalAmplitude, _radius,  Mathf.SmoothStep(0, 1, radiusAdaptPhase));
        }
        
        // Circle motion
        _phase += Time.deltaTime * _speed;
        
        Vector3 circlePosition = EnemyMovementPatterns.CircleMotion(
            -_patrolStartOffsetAngle, finalXRadius, _radius, _verticalAmplitude, _phase);
        
        Position2D = circlePosition;
        
        // Depth To Camera
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * (Position2D.y * _depthMultiplier);
        
        _localTime += Time.deltaTime;
    }

    public override void OnExit()
    {
        _localTime = 0;
        _patrolStartOffsetAngle = 0;
        Ended?.Invoke();
    }
}
