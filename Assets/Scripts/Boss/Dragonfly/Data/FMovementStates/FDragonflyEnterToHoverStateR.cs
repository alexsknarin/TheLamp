using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyEnterToHoverStateR", menuName = "FDragonflyMovementStates/FDragonflyEnterToHoverStateR")]
public class FDragonflyEnterToHoverStateR : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.EnterToHoverR);
    }
}
