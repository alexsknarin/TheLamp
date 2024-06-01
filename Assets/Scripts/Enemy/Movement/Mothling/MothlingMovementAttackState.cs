using UnityEngine;

public class MothlingMovementAttackState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Attack;
    private readonly float DEPTH_DECREMENT = 0.2f;
    private float _startDistance;
    
    public MothlingMovementAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed * 1.1f;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _startDistance = currentPosition.magnitude - 0.65f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 newPosition = currentPosition;
        Vector3 direction = -newPosition.normalized;
        newPosition += direction * (_speed * Time.deltaTime);
        Position = newPosition;
        
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        float attackProximityGradient = Mathf.Clamp((Position.magnitude - 0.65f) / _startDistance, 0.5f, 1.0f);
        Depth = cameraDirection * (1.1f * DEPTH_DECREMENT * attackProximityGradient);
    }
}
