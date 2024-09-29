using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyAttackHoverState", menuName = "DragonflyStates/DragonflyAttackHoverState")]
public class DragonflyAttackHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.AttackHover;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _acceleration = 0.75f;
    
    public override DragonflyState State => _state;
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
