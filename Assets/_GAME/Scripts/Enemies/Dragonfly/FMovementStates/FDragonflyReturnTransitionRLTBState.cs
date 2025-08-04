using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyReturnTransitionRLTBState", menuName = "FDragonflyMovementStates/FDragonflyReturnTransitionRLTBState")]
    public class FDragonflyReturnTransitionRLTBState : FDragonflyAnimBaseState, ILeft
    {
        public event Action Started;
    
        public override void Enter()
        {
            base.Enter();
            Started?.Invoke();
        }
    }
}
