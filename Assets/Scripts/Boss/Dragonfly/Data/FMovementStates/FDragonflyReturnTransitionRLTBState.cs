using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnTransitionRLTBState", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionRLTBState")]
public class FDragonflyReturnTransitionRLTBState : FDragonflyAnimBaseState
{
    public override void OnEnter()
    {
        ParentVisibleBodyToAnimatedTransform();
        _movement.PlayClip(this.GetType());
    }
}
