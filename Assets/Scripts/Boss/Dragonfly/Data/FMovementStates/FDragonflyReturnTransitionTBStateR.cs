using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnTransitionTBStateR", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionTBStateR")]
public class FDragonflyReturnTransitionTBStateR : FDragonflyAnimBaseState
{
    public void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.ReturnTransitionLRTB);
    }
}
