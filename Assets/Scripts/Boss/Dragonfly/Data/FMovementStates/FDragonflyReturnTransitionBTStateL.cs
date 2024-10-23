using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnTransitionBTStateL", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionBTStateL")]
public class FDragonflyReturnTransitionBTStateL : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.ReturnTransitionRLBT);
    }
}
