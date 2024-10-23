using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnTransitionBTStateR", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionBTStateR")]
public class FDragonflyReturnTransitionBTStateR : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.ReturnTransitionLRBT);
    }
}
