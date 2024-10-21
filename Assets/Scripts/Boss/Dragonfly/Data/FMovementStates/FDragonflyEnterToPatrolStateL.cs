using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyEnterToPatrolStateL", menuName = "FDragonflyMovementStates/FDragonflyEnterToPatrolStateL")]
public class FDragonflyEnterToPatrolStateL : FDragonflyAnimBaseState
{
    public void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(DragonflyMovementState.EnterToPatrolL);
    }
}
