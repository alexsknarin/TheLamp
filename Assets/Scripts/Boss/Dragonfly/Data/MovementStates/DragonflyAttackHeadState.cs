using UnityEngine;


[CreateAssetMenu(fileName = "DragonflyAttackHeadState", menuName = "DragonflyStates/DragonflyAttackHeadState")]
public class DragonflyAttackHeadState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.AttackHead;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _acceleration = 0.75f;
    
    public override DragonflyState State => _state;
    private float _attackAccelerationValue = 0;
    private Vector3 _attackDirection;

    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        _attackAccelerationValue = 0;
        
        Vector3 sideGoal = currentPosition;
        sideGoal.z = 0;
        sideGoal.Normalize();
        sideGoal *= 0.85f;
        
        if (currentPosition.z > 0)
        {
            sideGoal *= 0.55f;
            sideGoal.y *= 0.75f;
        }
        else
        {
            sideGoal *= 0.85f;
        }

        _attackDirection = (sideGoal - currentPosition).normalized;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        _stateData.VisibleBodyTransform.position += _attackDirection * (_speed * Time.deltaTime + _attackAccelerationValue);
        _attackAccelerationValue += _acceleration * Time.deltaTime;
        
    }
}
