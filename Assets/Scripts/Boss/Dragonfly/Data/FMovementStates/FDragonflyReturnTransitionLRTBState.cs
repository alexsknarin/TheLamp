using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnTransitionLRTBState", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionLRTBState")]
public class FDragonflyReturnTransitionLRTBState : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.ReturnTransitionLRTB);
    }
}
