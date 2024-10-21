using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyCatchSpiderStateL", menuName = "FDragonflyMovementStates/FDragonflyCatchSpiderStateL")]
public class FDragonflyCatchSpiderStateL : FDragonflyAnimBaseState
{
    public void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.CatchSpiderL);
    }
}
