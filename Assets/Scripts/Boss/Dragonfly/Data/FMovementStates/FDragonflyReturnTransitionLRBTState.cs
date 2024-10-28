using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnTransitionLRBTState", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionLRBTState")]
public class FDragonflyReturnTransitionLRBTState : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.ReturnTransitionLRBT);
    }
}
