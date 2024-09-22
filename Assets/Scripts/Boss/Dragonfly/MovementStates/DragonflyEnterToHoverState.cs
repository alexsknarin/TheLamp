using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "DragonflyEnterToHoverState", menuName = "DragonflyStates/DragonflyEnterToHoverState")]
public class DragonflyEnterToHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.EnterToHoverL;
    public override DragonflyStates State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyStates.EnterToHoverL;
        }
        else
        {
            _state = DragonflyStates.EnterToHoverR;
        }
       
        _stateData.Owner.PlayClip(_state);
    }
}
