using System;
using _GAME.Scripts.Enemies;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IEnemyDeactivatedProvider
    {
        public event Action<FEnemy> EnemyReleasedToPool;
    }
}