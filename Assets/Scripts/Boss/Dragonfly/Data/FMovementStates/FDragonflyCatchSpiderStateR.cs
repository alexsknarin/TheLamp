using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyCatchSpiderStateR", menuName = "FDragonflyMovementStates/FDragonflyCatchSpiderStateR")]
public class FDragonflyCatchSpiderStateR: FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.CatchSpiderR);
    }
}
