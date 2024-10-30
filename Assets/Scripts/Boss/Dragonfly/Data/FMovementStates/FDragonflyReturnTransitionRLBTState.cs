using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnTransitionRLBTState", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionRLBTState")]
public class FDragonflyReturnTransitionRLBTState : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(this.GetType());
    }
}
