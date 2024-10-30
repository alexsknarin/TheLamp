using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyEnterToPatrolStateR", menuName = "FDragonflyMovementStates/FDragonflyEnterToPatrolStateR")]
public class FDragonflyEnterToPatrolStateR : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(this.GetType());
    }
}
