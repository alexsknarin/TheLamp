using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnTransitionBTStateL", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionBTStateL")]
public class FDragonflyReturnTransitionBTStateL : FDragonflyAnimBaseState
{
    public void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.ReturnTransitionRLBT);
    }
}
