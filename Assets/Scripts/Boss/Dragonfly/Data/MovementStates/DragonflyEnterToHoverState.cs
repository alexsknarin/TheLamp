using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyEnterToHoverState", menuName = "DragonflyStates/DragonflyEnterToHoverState")]
public class DragonflyEnterToHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.EnterToHoverL;
    public override DragonflyMovementState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyMovementState.EnterToHoverL;
        }
        else
        {
            _state = DragonflyMovementState.EnterToHoverR;
        }
       
        _stateData.Owner.PlayClip(_state);
    }
}
