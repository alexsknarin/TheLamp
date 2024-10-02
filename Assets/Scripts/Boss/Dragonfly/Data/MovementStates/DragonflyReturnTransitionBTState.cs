using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyReturnTransitionBTState", menuName = "DragonflyStates/DragonflyReturnTransitionBTState")]
public class DragonflyReturnTransitionBTState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.ReturnTransitionRLBT;
    public override DragonflyState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyState.ReturnTransitionRLBT;
        }
        else
        {
            _state = DragonflyState.ReturnTransitionLRBT;
        }
        
        _stateData.Owner.PlayClip(_state);
    }
}
