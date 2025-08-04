using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyEnterToHoverStateL", menuName = "FDragonflyMovementStates/FDragonflyEnterToHoverStateL")]
    public class FDragonflyEnterToHoverStateL : FDragonflyAnimBaseState, ILeft
    {
        public event Action Started;
    
        public override void Enter()
        {
            base.Enter();
            Started?.Invoke();
        }
    }
}
