using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyEnterToHoverStateL", menuName = "FDragonflyMovementStates/FDragonflyEnterToHoverStateL")]
    public class FDragonflyEnterToHoverStateL : FDragonflyAnimBaseState
    {
        public override void OnEnter()
        {
            ParentVisibleBodyToAnimatedTransform();
            _animator.Play(_clipHash, -1, 0);
        }
    }
}
