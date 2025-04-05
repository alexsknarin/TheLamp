using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies
{
    public abstract class CollidableEnemy: Enemy, ICollidableWithLamp
    {
        public virtual  Vector2 Position { get; protected set; }
        public virtual float Radius { get; protected set; }
        public CollidableState CollisionState { get; protected set; }
    
        public virtual void HandleEnterAttackZone()
        {
            CollisionState = CollidableState.InAttackZone;
            IsReadyForDamage = true;
        }

        public abstract void HandleCollision();

        public virtual void HandleExitAttackZone()
        {
            CollisionState = CollidableState.Outside;
            IsReadyForDamage = false;
        }

        public virtual Vector3 ProvideImpactPoint()
        {
            return transform.position;
        }
    }
}
