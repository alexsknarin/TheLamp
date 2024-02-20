using UnityEngine;

public class LadybugMovementStickState: EnemyMovementBaseState
{
    public LadybugMovementStickState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    public override EnemyStates State => EnemyStates.Stick;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition;
    }

    public override void ExitState()
    {
    }
}