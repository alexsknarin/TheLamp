using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyEnterToPatrolStateL", menuName = "FDragonflyMovementStates/FDragonflyEnterToPatrolStateL")]
public class FDragonflyEnterToPatrolStateL : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(this.GetType());
    }
}
