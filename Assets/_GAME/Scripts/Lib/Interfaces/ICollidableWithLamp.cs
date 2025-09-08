using _GAME.Scripts.Lib.Enums;
using UnityEngine;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface ICollidableWithLamp
    {
        public float Radius { get; }
        public Vector2 Position { get; }
        public string CollidableName { get; }
        public bool IsReceivedLampAttackDamage { get; }
        public CollidableState CollisionState { get; }
        public void HandleEnterAttackZone();
        public void HandleCollision();
        public void HandleExitAttackZone();
        public Vector3 ProvideImpactPoint();
    }
}
