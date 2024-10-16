using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyMoveToPatrolState", menuName = "DragonflyStates/DragonflyMoveToPatrolState")]
public class DragonflyMoveToPatrolState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.MoveToPatrolL;
    public override DragonflyMovementState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyMovementState.MoveToPatrolL;
        }
        else
        {
            _state = DragonflyMovementState.MoveToPatrolR;
        }
        
        _stateData.Owner.PlayClip(_state);
    }
}
