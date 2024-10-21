using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyEnterToHoverStateL", menuName = "FDragonflyMovementStates/FDragonflyEnterToHoverStateL")]
public class FDragonflyEnterToHoverStateL : FDragonflyAnimBaseState
{
    public void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.EnterToHoverL);
    }
}
