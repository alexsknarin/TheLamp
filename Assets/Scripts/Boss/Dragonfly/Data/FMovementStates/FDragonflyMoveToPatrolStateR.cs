using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyMoveToPatrolStateR", menuName = "FDragonflyMovementStates/FDragonflyMoveToPatrolStateR")]
public class FDragonflyMoveToPatrolStateR : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(this.GetType());
    }
}
