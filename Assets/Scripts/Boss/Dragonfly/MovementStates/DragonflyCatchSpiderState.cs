using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyCatchSpiderState", menuName = "DragonflyStates/DragonflyCatchSpiderState")]
public class DragonflyCatchSpiderState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.CatchSpiderL;
    public override DragonflyState State => _state;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if (sideDirection == 1)
        {
            _state = DragonflyState.CatchSpiderL;
        }
        else
        {
            _state = DragonflyState.CatchSpiderR;
        }
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        _stateData.Owner.PlayClip(_state);
    }
}
