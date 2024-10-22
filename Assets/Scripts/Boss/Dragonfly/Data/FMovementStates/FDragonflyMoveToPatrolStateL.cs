using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyMoveToPatrolStateL", menuName = "FDragonflyMovementStates/FDragonflyMoveToPatrolStateL")]
public class FDragonflyMoveToPatrolStateL : FDragonflyAnimBaseState
{
    public void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.MoveToPatrolL);
    }
}
