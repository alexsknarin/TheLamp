using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyEnterToPatrolStateR", menuName = "FDragonflyMovementStates/FDragonflyEnterToPatrolStateR")]
public class FDragonflyEnterToPatrolStateR : FDragonflyAnimBaseState
{
    public void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.EnterToPatrolR);
    }
}
