using System;
using _GAME.Scripts.Lib.Enums;
using UnityEngine;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IStickableWithLamp
    {
        public event Action<IStickableWithLamp> StickReadyStarted;
        public Vector2 Position { get; }
        public float Radius { get; }
        public bool IsSticked { get; }
        public AttackBlockerState AttackBlockState { get; }
        public StickableState StickState { get; }
        public void HandleEnterAttackZone();
        public void HandleStick(Transform lampTransform);
        public void HandleExitAttackZone();
        public void HandleEnterAttackBlockerZone();
        public void HandleLampDestroyed();
        public Vector3 ProvideImpactPoint();
    }
}
