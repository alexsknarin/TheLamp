using UnityEngine;

public class FMothlingMovementPatrolStateRIn: FMothlingMovementStateBase
{
    // TODO: provided by the factory or even hardcoded
    // Get from the config via factory DI into all states
    protected readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    protected Vector2 _spawnAreaCenter = new Vector2(3.6f, -2.6f); 
    protected float _speed = 0.81f;
    protected float _radius = 1.55f;
    protected float _verticalAmplitude = 0.93f;
    protected float _spawnAreaSize = 0.5f;
    
    // Dependencies
    private readonly IPosition2DProvider _positionProvider;
    
    // LR IN OUT settings
    protected int _sideDirection = 1; // L R
    protected int _depthDirection = 1; // In Out
    
    // Local variables
    private float _verticalAdaptDuration = 2f;
    private float _patrolStartOffsetAngle;
    private float _depthMultiplier = 1f;
    private float _phase;
    private float _localTime;
    
    public FMothlingMovementPatrolStateRIn(IPosition2DProvider positionProvider)
    {
        _positionProvider = positionProvider;
    }
    
    public override void OnEnter()
    {
        Debug.Log("FMothlingMovementPatrolStateRIn OnEnter");
        Position2D = _positionProvider.Position2D;
        Vector3 horizontalVector = Vector2.right;
        horizontalVector.x *= _sideDirection;
        _patrolStartOffsetAngle = Mathf.Acos(Vector3.Dot(horizontalVector.normalized, Position2D.normalized));
        _phase = 0;
        _localTime = 0;
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
        Position2D = circlePosition;
        
        // Depth To Camera
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * (_depthDirection * Position2D.y * _depthMultiplier);
        
        _localTime += Time.deltaTime;
    }

    public override void OnExit()
    {
        _localTime = 0;
        _patrolStartOffsetAngle = 0;
    }
}
