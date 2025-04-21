using System;
using _GAME.Scripts.Enemies;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IProjectileShooter
    {
        public event Action<CollidableEnemy> ProjectileShot;
        public event Action<Enemy, bool> ProjectileDeactivated;
    }
}
