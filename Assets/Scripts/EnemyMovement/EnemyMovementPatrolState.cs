using UnityEngine;

public class EnemyMovementPatrolState: EnemyMovementBaseState
{
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

    public override void EnterState()
    {
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Debug.Log(currentPosition);
        Position = currentPosition;
    }

    public void ExitState()
    {
    }
}