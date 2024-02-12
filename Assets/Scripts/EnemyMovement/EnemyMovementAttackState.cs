using UnityEngine;

public class EnemyMovementAttackState: EnemyMovementBaseState
{
    public EnemyMovementAttackState(EnemyBaseMovement owner, int sideDirection, float speed, float radius, float verticalAmplitude) : base()
    {
        _sideDirection = sideDirection;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    public override EnemyStates State
    {
        get => EnemyStates.Attack;
        protected set  { }
    }

    public override void EnterState()
    {
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
    }

    public void ExitState()
    {
    }
}
