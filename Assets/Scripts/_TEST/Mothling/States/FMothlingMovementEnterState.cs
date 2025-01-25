using UnityEngine;

public class FMothlingMovementEnterState: FMothlingMovementStateBase
{
    // TODO: provided by the factory or even hardcoded
    // Get from the config via factory DI into all states
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f); // DI?
    
    // from config???
    private Vector2 _spawnAreaCenter = new Vector2(3.6f, -2.6f);
    private float _speed = 0.81f;
    private float _radius = 1.55f;
    private float _verticalAmplitude = 0.93f;
    private float _spawnAreaSize = 0.5f;
    
    // State specific attributes
    private Vector2 _endPos = Vector3.zero;
    private Vector2 _enterDirection;
    private readonly float _depthMultiplier = 2f;
    private float _initialDistance;

    public override void OnEnter()
    {
        ReadyToSwitch = false;
        
        Position2D = GenerateSpawnPosition(-1);
        
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
            ReadyToSwitch = true;
        }
    }

    private Vector3 GenerateSpawnPosition(int direction)
    {
        Vector3 spawnPosition = Random.insideUnitCircle * _spawnAreaSize + _spawnAreaCenter;
        spawnPosition = _spawnAreaCenter;
        spawnPosition.x *= direction;
        return spawnPosition;
    }

}
