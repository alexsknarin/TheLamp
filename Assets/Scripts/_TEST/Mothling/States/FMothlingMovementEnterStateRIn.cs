using UnityEngine;

public class FMothlingMovementEnterStateRIn: FMothlingMovementStateBase
{
    // LR IN OUT settings
    protected int _sideDirection = 1; // L R
    protected int _depthDirection = 1; // In Out

    // TODO: Common attributes - setup though the Factory + abstract class
    // Dependencies

    // TODO: provided by the factory or even hardcoded
    // Get from the config via factory DI into all states
    protected readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    protected Vector2 _spawnAreaCenter = new Vector2(3.6f, -2.6f); 
    protected float _speed = 0.81f;
    protected float _radius = 1.55f;
    protected float _verticalAmplitude = 0.93f;
    protected float _spawnAreaSize = 0.5f;
    
    
    // State specific attributes
    protected Vector2 _endPos = Vector3.zero;
    protected Vector2 _enterDirection;
    protected float _depthMultiplier = 2f;
    protected float _initialDistance;

    public override void OnEnter()
    {
        ReadyToSwitch = false;
        InitializeMovementData(_sideDirection);
    }

    public override void Tick()
    {
        CalculatePositionAndDepthDirection(_sideDirection, _depthDirection);
    }

    protected Vector3 GenerateSpawnPosition(int direction)
    {
        Vector3 spawnPosition = Random.insideUnitCircle * _spawnAreaSize + _spawnAreaCenter;
        spawnPosition.x *= direction;
        return spawnPosition;
    }

    protected void InitializeMovementData(int sideDirection)
    {
        Position2D = GenerateSpawnPosition(-sideDirection);
        
        float xProjectionLength = Mathf.Abs(Position2D.x);
        float enterDirectionLength = Vector3.Magnitude(Position2D);
        float r = _radius * _verticalAmplitude;
        
        float patrolStartOffsetAngle = 
            Mathf.PI - Mathf.Acos(r / enterDirectionLength) - Mathf.Acos(xProjectionLength / enterDirectionLength);
        
        _endPos.x = Mathf.Cos(-patrolStartOffsetAngle) * sideDirection;
        _endPos.y = Mathf.Sin(-patrolStartOffsetAngle);
        _endPos = _endPos.normalized * r;
        
        _enterDirection = (_endPos - Position2D);
        _initialDistance = _enterDirection.magnitude;
        _enterDirection = _enterDirection.normalized;
        
        Position2D = Position2D;
    }

    private void CalculatePositionAndDepthDirection(int sideDirection, int depthDirection)
    {
        Position2D += _enterDirection * (_speed * Time.deltaTime * (Mathf.PI/2));
        
        // Depth To Camera
        float distancePhase = 1 - (_endPos - Position2D).magnitude / _initialDistance;
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * (depthDirection * Mathf.Lerp(Position2D.y * _depthMultiplier, Position2D.y, distancePhase));
        
        if(Position2D.x * sideDirection > Mathf.Abs(_endPos.x))
        {
            ReadyToSwitch = true;
        }
    }
}
