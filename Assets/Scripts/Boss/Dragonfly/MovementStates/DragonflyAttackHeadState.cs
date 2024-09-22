using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyAttackHeadState", menuName = "DragonflyStates/DragonflyAttackHeadState")]
public class DragonflyAttackHeadState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackHead;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _acceleration = 0.75f;
    
    public override DragonflyStates State => _state;
    private float _attackAccelerationValue = 0;
    private Vector3 _attackDirection;

    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        _attackDirection = -_stateData.VisibleBodyTransform.position.normalized;

    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        _stateData.VisibleBodyTransform.position += _attackDirection * (_speed * Time.deltaTime + _attackAccelerationValue);
        _attackAccelerationValue += _acceleration * Time.deltaTime;
        
    }
}
