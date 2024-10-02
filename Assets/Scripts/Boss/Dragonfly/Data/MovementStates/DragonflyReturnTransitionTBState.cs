using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyReturnTransitionTBState", menuName = "DragonflyStates/DragonflyReturnTransitionTBState")]
public class DragonflyReturnTransitionTBState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.ReturnTransitionRLTB;
    public override DragonflyState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyState.ReturnTransitionRLTB;
        }
        else
        {
            _state = DragonflyState.ReturnTransitionLRTB;
        }
        
        _stateData.Owner.PlayClip(_state);
    }
}
