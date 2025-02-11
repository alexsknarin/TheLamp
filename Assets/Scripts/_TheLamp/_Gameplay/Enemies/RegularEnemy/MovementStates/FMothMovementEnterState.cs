using UnityEngine;

public class FMothMovementEnterState: RegularEnemyMovementStateBase
{
    // Dependencies
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private readonly float _speed;
    private readonly float _radius;
    private readonly float _verticalAmplitude;
    
    // State specific attributes
    private float _deadZoneAngle = 35;
    private float _minDistance = 2.5f;
    private float _maxDistance = 4.7f;
    
    private Vector2 _endPos = Vector2.zero;
    private Vector2 _enterDirection;
    private float _depthMultiplier = 1.6f;
    private float _initialDistance;
    private float _phase;
    private float _noiseFrequency = 9f;
    private float _noiseAmplitude = 0.05f;
    
    
    public FMothMovementEnterState(
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
        Position2D = GenerateSpawnPosition(_deadZoneAngle, _minDistance, _maxDistance);
        
        // Find intersection to the ellipse
        float a = _radius;
        float b = _radius * _verticalAmplitude;

        float denominator = ((a * b) / (Mathf.Sqrt((a * a) * (Position2D.y * Position2D.y) 
                                                   + (b * b) * (Position2D.x * Position2D.x)))); 
        _endPos = Vector3.zero;
        _endPos.x = denominator * Position2D.x;
        _endPos.y = denominator * Position2D.y;
       
        _enterDirection = (Vector2.zero - Position2D).normalized;
        _initialDistance = (_endPos - Position2D).magnitude;
        _phase = 1;
    }

    public override void Tick()
    {
        
        Position2D += _enterDirection * (_speed * Time.deltaTime * (Mathf.PI/2));
        _phase = (_endPos - Position2D).magnitude / _initialDistance;
        
        // Add noise
        Vector2 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
        Position2D += trajectoryNoise * _noiseAmplitude;

        // Depth To Camera
        float distancePhase = 1 - (_endPos - Position2D).magnitude / _initialDistance;
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * (Mathf.Lerp(Position2D.y * _depthMultiplier, Position2D.y, distancePhase));
    

        if(_phase < 0.02f || Position2D.magnitude < _radius)
        {
            IsReadyToSwitch = true;
        }
    }

    // TODO: make Appear from the top more often
    private Vector2 GenerateSpawnPosition(float deadZoneAngle, float minDistance, float maxDistance)
    {
        Vector3 spawnPosition = Vector3.up;
        float angle = deadZoneAngle + Random.Range(0, 360-deadZoneAngle*2);
        
        spawnPosition = Quaternion.AngleAxis(angle, Vector3.forward) * spawnPosition;
        
        float distance = Mathf.Lerp(minDistance, maxDistance, Mathf.Abs(spawnPosition.y));
        spawnPosition *= distance;
            
        return spawnPosition;
    }
}
