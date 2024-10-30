using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyEnterToHoverStateL", menuName = "FDragonflyMovementStates/FDragonflyEnterToHoverStateL")]
public class FDragonflyEnterToHoverStateL : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(this.GetType());
    }
}
