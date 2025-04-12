using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies
{
    public abstract class EnemyMovementStateBase: IState
    {
        public Vector2 Position2D { get; protected set; }
        public Vector3 DepthDirection { get; protected set; }
        public bool IsReadyToSwitch { get; protected set; }
        public abstract void Enter();
        public abstract void Tick();
        public virtual void Exit() { }
    }
}
