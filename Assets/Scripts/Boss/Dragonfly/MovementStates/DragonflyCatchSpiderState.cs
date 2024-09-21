using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyCatchSpiderState", menuName = "DragonflyStates/DragonflyCatchSpiderState")]
public class DragonflyCatchSpiderState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.CatchSpiderL;
    public override DragonflyStates State => _state;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        _stateData.Owner.PlayClip(_state);
    }
}
