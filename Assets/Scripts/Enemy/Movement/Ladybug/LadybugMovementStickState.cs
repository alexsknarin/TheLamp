using UnityEngine;

public class LadybugMovementStickState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Stick;
    public LadybugMovementStickState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * 0.3f;
        Position = currentPosition;
    }
}