using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyReturnTransitionLRTBState", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionLRTBState")]
    public class FDragonflyReturnTransitionLRTBState : FDragonflyAnimBaseState, IRight
    {
        public event Action Started;
    
        public override void Enter()
        {
            base.Enter();
            Started?.Invoke();
        }
    }
}
