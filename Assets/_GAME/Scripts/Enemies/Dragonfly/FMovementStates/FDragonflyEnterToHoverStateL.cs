using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyEnterToHoverStateL", menuName = "FDragonflyMovementStates/FDragonflyEnterToHoverStateL")]
    public class FDragonflyEnterToHoverStateL : FDragonflyAnimBaseState, ILeft
    {
        public override void Enter()
        {
            ParentVisibleBodyToAnimatedTransform();
            _animator.Play(_clipHash, -1, 0);
        }
    }
}
