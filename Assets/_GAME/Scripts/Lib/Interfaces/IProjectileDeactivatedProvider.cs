using System;
using _GAME.Scripts.Enemies;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IProjectileDeactivatedProvider
    {
        public event Action<FEnemy> ProjectileDestroyed;
    }
}
