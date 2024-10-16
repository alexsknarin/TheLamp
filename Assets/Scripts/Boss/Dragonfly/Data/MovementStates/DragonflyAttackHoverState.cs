using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyAttackHoverState", menuName = "DragonflyStates/DragonflyAttackHoverState")]
public class DragonflyAttackHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.AttackHover;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _acceleration = 0.75f;
    
    public override DragonflyMovementState State => _state;
    private float _attackAccelerationValue = 0;
    private Vector3 _attackDirection;

    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        // _attackDirection = -_stateData.VisibleBodyTransform.position.normalized;
        _attackAccelerationValue = 0;
        
        Vector3 sideGoal = currentPosition;
        sideGoal.z = 0;
        sideGoal.Normalize();
        sideGoal *= 0.85f;
        
        if (currentPosition.z > 0)
        {
            sideGoal *= 0.95f;
            sideGoal.y *= 0.75f;
        }
        else
        {
            sideGoal *= 0.85f;
        }
        _attackDirection = (sideGoal - currentPosition).normalized;
                
        Debug.DrawLine(currentPosition, sideGoal, Color.yellow, 5f);
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        _stateData.VisibleBodyTransform.position += _attackDirection * (_speed * Time.deltaTime + _attackAccelerationValue);
        _attackAccelerationValue += _acceleration * Time.deltaTime;
        
    }
}
