using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyEnterToPatrolState", menuName = "DragonflyStates/DragonflyEnterToPatrolState")]
public class DragonflyEnterToPatrolState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.EnterToPatrolL;
    public override DragonflyMovementState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyMovementState.EnterToPatrolL;
        }
        else
        {
            _state = DragonflyMovementState.EnterToPatrolR;
        }
        
        _stateData.Owner.PlayClip(_state);
    }
}
