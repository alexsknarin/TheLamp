using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyCatchSpiderStateR", menuName = "FDragonflyMovementStates/FDragonflyCatchSpiderStateR")]
    public class FDragonflyCatchSpiderStateR: FDragonflyAnimBaseState
    {
        public event Action Started;
        public override void Enter()
        {
            base.Enter();
            Started?.Invoke();
        }
    }
}
