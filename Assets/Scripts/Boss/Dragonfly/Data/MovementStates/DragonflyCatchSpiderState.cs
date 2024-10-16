using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyCatchSpiderState", menuName = "DragonflyStates/DragonflyCatchSpiderState")]
public class DragonflyCatchSpiderState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.CatchSpiderL;
    public override DragonflyMovementState State => _state;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if (sideDirection == 1)
        {
            _state = DragonflyMovementState.CatchSpiderL;
        }
        else
        {
            _state = DragonflyMovementState.CatchSpiderR;
        }
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        _stateData.Owner.PlayClip(_state);
    }
}
