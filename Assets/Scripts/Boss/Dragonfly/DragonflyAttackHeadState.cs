using UnityEngine;

public class DragonflyAttackHeadState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovement _owner;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackHeadL;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _acceleration = 0.75f;
    
    public override DragonflyStates State => _state;
    private float _attackAccelerationValue = 0;
    private Vector3 _attackDirection;

    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _visibleBodyTransform.SetParent(_owner.transform);
        _attackDirection = -_visibleBodyTransform.position.normalized;

    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        _visibleBodyTransform.position += _attackDirection * (_speed * Time.deltaTime + _attackAccelerationValue);
        _attackAccelerationValue += _acceleration * Time.deltaTime;
        
    }
}
