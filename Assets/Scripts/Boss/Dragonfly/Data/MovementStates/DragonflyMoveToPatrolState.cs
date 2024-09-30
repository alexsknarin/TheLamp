using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyMoveToPatrolState", menuName = "DragonflyStates/DragonflyMoveToPatrolState")]
public class DragonflyMoveToPatrolState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.MoveToPatrolL;
    public override DragonflyState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyState.MoveToPatrolL;
        }
        else
        {
            _state = DragonflyState.MoveToPatrolR;
        }
        
        _stateData.Owner.PlayClip(_state);
    }
}
