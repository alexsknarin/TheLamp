using UnityEngine;

public class EnemyMovementPatrolState: EnemyMovementBaseState
{
    private float _verticalAdaptDuration = 2f; // TODO: Serialized Field
    private float _prevTime;
    private float _patrolStartOffsetAngle;
    private float _enterTimeOffset; // TMP
    
    public EnemyMovementPatrolState(EnemyBaseMovement owner, int sideDirection, float speed, float radius, float verticalAmplitude) : base()
    {
        _sideDirection = sideDirection;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override EnemyStates State
    {
        get => EnemyStates.Patrol;
        protected set  { }
    }

    public override void EnterState(Vector3 currentPosition)
    {
        _prevTime = Time.time;
        Vector3 horizntalVector = Vector3.right;
        horizntalVector.x = _sideDirection;
        _patrolStartOffsetAngle = Mathf.Acos(Vector3.Dot(horizntalVector.normalized, currentPosition.normalized));
        
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        // Adapt Radius
        float radiusAdaptPhase = (Time.time - _prevTime) / _verticalAdaptDuration;
        float finalXRadius = _radius;
        if (radiusAdaptPhase < 1f)
        {
            finalXRadius = Mathf.Lerp(_radius * _verticalAmplitude, _radius,  Mathf.SmoothStep(0, 1, radiusAdaptPhase));
        }
        
        float phase = Time.time * _speed * _sideDirection;  // find a way to provide _enterTimeOffset;
        
        float offsetAngleWithDirection;
        if (_sideDirection < 0)
        {
            offsetAngleWithDirection = _patrolStartOffsetAngle;
        }
        else
        {
            offsetAngleWithDirection = -_patrolStartOffsetAngle-Mathf.PI;
        }
        
        Vector3 newPosition = Vector3.zero;
        
        newPosition.x = Mathf.Cos(phase + offsetAngleWithDirection) * finalXRadius;               //TODO: X radius Y radius
        newPosition.y = Mathf.Sin(phase + offsetAngleWithDirection) * _radius * _verticalAmplitude;
        
        Position = newPosition;
        
    }

    public void ExitState()
    {
    }
}