using UnityEngine;

public class FLadybugMovementPatrolState: RegularEnemyMovementStateBase
{
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private readonly ILampPositionProviderService _lampPositionProviderService;
    private readonly float _speed;
    private readonly float _radius;
    private readonly float _verticalAmplitude;
    
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset;
    private float _phase;
    private float _spiralSpeedStart = 0.2f;
    private float _spiralSpeedEnd = 0.015f;
    private float _spiralPhase = 1f;
    private float _preAttackTriggerDistance = 0.7f;
    private float _preAttackTriggerYThreshold = 0.3f;
    private float _depthMultiplierMax = 3f;
    private float _depthMultiplierMin = 0f;
    
    public FLadybugMovementPatrolState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        ILampPositionProviderService lampPositionProviderService,
        float speed,
        float radius,
        float verticalAmplitude
        )
    { 
        // _cameraPosition = cameraPosition; TODO: enable later
        _positionDirectionProvider = positionDirectionProvider;
        _lampPositionProviderService = lampPositionProviderService;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
    }
    
    
    public override void OnEnter()
    {
        _phase = 0;
        _spiralPhase = 1f;
        Position2D = _positionDirectionProvider.Position2D;
        
        Vector2 horizontalVector = Vector2.right;
        _patrolStartOffsetAngle = Mathf.Acos(Vector2.Dot(horizontalVector.normalized, Position2D.normalized));
        _patrolStartOffsetAngle *= Mathf.Sign(Position2D.y);
        
        IsReadyToSwitch = false;
    }

    public override void Tick()
    {
        float speedCompenstation = (1 - Position2D.magnitude/_radius) + 1;
       
        _phase += Time.deltaTime * _speed * speedCompenstation;
        Vector2 ellipsePosition = EnemyMovementPatterns.CircleMotion(_patrolStartOffsetAngle, _radius, _radius, _verticalAmplitude, _phase);
        ellipsePosition *= _spiralPhase;

        Vector2 circlePosition = ellipsePosition;
        circlePosition.y /= _verticalAmplitude;
        
        if (ellipsePosition.magnitude > _preAttackTriggerDistance)
        {
            _spiralPhase -= Mathf.Lerp(_spiralSpeedStart, _spiralSpeedEnd, 1 - (circlePosition.magnitude/_radius)) 
                            * Time.deltaTime;
        }

        Position2D = ellipsePosition + _lampPositionProviderService.GetLampPosition();
        
        // Depth To Camera
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        float depthPhase = Mathf.Clamp(circlePosition.magnitude - _preAttackTriggerDistance, 0.0001f, _radius) 
                           / (_radius - _preAttackTriggerDistance);
        depthPhase = Mathf.Pow(depthPhase, 0.85f);
        depthPhase = Mathf.Clamp(depthPhase, 0.0001f, 1f);
        float depthValue = Mathf.Lerp(_depthMultiplierMin, _depthMultiplierMax, depthPhase);
        DepthDirection = cameraDirection * depthValue;
        
        
        if((Position2D - _lampPositionProviderService.GetLampPosition()).magnitude < _preAttackTriggerDistance 
           && Position2D.y < _preAttackTriggerYThreshold)
        {
            Debug.Log("PreAttack Ready");
            IsReadyToSwitch = true;
        }
    }
}
