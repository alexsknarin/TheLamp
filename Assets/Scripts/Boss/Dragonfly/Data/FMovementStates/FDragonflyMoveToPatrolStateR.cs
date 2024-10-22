using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyMoveToPatrolStateR", menuName = "FDragonflyMovementStates/FDragonflyMoveToPatrolStateR")]
public class FDragonflyMoveToPatrolStateR : FDragonflyAnimBaseState
{
    public void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.MoveToPatrolR);
    }
}
