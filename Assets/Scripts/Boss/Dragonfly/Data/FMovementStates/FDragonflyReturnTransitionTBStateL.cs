using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnTransitionTBStateL", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionTBStateL")]
public class FDragonflyReturnTransitionTBStateL : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.ReturnTransitionRLTB);
    }
}
