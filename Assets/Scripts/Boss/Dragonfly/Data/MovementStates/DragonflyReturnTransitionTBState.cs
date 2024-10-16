using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyReturnTransitionTBState", menuName = "DragonflyStates/DragonflyReturnTransitionTBState")]
public class DragonflyReturnTransitionTBState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.ReturnTransitionRLTB;
    public override DragonflyMovementState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyMovementState.ReturnTransitionRLTB;
        }
        else
        {
            _state = DragonflyMovementState.ReturnTransitionLRTB;
        }
        
        _stateData.Owner.PlayClip(_state);
    }
}
