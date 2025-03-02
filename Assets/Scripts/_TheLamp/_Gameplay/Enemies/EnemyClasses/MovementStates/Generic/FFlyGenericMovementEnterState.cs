using UnityEngine;

public class FFlyGenericMovementEnterState: RegularEnemyMovementStateBase
{
    // Dependencies
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private ILampPositionProviderService _lampPositionProviderService; // TODO: enable later
    private readonly float _speed;
    private readonly float _radius;
    private readonly float _verticalAmplitude;

    // State specific attributes
    private readonly Vector2 _invertX = new Vector2(-1, 1); // DI?
    private Vector2 _endPos = Vector2.zero;
    private Vector2 _enterDirection;
    private readonly float _depthMultiplier = 2f;
    private float _initialDistance;

    public FFlyGenericMovementEnterState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService,
            float speed,
            float radius,
            float verticalAmplitude
        )
    {
        // _cameraPosition = cameraPosition; // TODO: enable later
        _positionDirectionProvider = positionDirectionProvider;
        _lampPositionProviderService = lampPositionProviderService;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
    }

    public override void OnEnter()
    {
        IsReadyToSwitch = false;
        Position2D = _positionDirectionProvider.Position2D;
        if (Position2D.x > 0)
        {
            Position2D *= _invertX;
        }
        
        float xProjectionLength = Mathf.Abs(Position2D.x);
        float enterDirectionLength = Vector3.Magnitude(Position2D);
        float r = _radius * _verticalAmplitude;
        
        float patrolStartOffsetAngle = 
            Mathf.PI - Mathf.Acos(r / enterDirectionLength) - Mathf.Acos(xProjectionLength / enterDirectionLength);
        
        _endPos.x = Mathf.Cos(-patrolStartOffsetAngle);
        _endPos.y = Mathf.Sin(-patrolStartOffsetAngle);
        _endPos = _endPos.normalized * r;
        
        _enterDirection = (_endPos - Position2D);
        _initialDistance = _enterDirection.magnitude;
        _enterDirection = _enterDirection.normalized;
        
        Position2D = Position2D;
        DepthDirection = (_cameraPosition - (Vector3)Position2D).normalized;
    }

    public override void Tick()
    {
        Position2D += _enterDirection * (_speed * Time.deltaTime * (Mathf.PI/2));
        
        // Depth To Camera
        float distancePhase = 1 - (_endPos - Position2D).magnitude / _initialDistance;
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * (Mathf.Lerp(Position2D.y * _depthMultiplier, Position2D.y, distancePhase));
        
        if(Position2D.x > Mathf.Abs(_endPos.x))
        {
            IsReadyToSwitch = true;
        }
    }
}
