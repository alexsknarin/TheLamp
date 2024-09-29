using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyEnterToHoverState", menuName = "DragonflyStates/DragonflyEnterToHoverState")]
public class DragonflyEnterToHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.EnterToHoverL;
    public override DragonflyState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyState.EnterToHoverL;
        }
        else
        {
            _state = DragonflyState.EnterToHoverR;
        }
       
        _stateData.Owner.PlayClip(_state);
    }
}
