using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyCatchSpiderStateL", menuName = "FDragonflyMovementStates/FDragonflyCatchSpiderStateL")]
    public class FDragonflyCatchSpiderStateL : FDragonflyAnimBaseState
    {
        public event Action Started;
    
        public override void Enter()
        {
            base.Enter();
            Started?.Invoke();
        }
    }
}
