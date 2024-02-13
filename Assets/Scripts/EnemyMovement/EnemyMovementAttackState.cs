using Unity.VisualScripting;
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

    public override void EnterState(Vector3 currentPosition)
    {
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 newPosition = currentPosition;
        Vector3 direction = -newPosition.normalized;
        newPosition += direction * _speed * Time.deltaTime;
        Position = newPosition;
    }

    public void ExitState()
    {
    }
}
