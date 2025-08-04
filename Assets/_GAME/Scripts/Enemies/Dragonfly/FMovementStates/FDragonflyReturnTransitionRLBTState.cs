using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyReturnTransitionRLBTState", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionRLBTState")]
    public class FDragonflyReturnTransitionRLBTState : FDragonflyAnimBaseState, ILeft
    {
        public event Action Started;
    
        public override void Enter()
        {
            base.Enter();
            Started?.Invoke();
        }
    }
}
