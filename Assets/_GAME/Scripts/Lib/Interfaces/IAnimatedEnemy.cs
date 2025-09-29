using System;
using _GAME.Scripts.Enemies;

namespace _GAME.Scripts.Lib.Interfaces
{
    /// <summary>
    /// For enemies that attack on their own, without EnemyAttack Controller to be involved.
    /// Provided event allows enemy controiller to know when enemy is about to attack.
    /// </summary>
    public interface IAnimatedEnemy
    {
        
        public event Action<CollidableEnemy> AnimatedAttackStarted;
    }
}
