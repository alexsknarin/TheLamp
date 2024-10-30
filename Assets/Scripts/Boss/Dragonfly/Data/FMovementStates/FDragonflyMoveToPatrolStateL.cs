using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyMoveToPatrolStateL", menuName = "FDragonflyMovementStates/FDragonflyMoveToPatrolStateL")]
public class FDragonflyMoveToPatrolStateL : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(this.GetType());
    }
}
