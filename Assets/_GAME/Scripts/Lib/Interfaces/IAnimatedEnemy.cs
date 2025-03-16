using System;
using _GAME.Scripts.Enemies;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IAnimatedEnemy
    {
        public event Action<CollidableEnemy> AnimatedAttackStarted;
    }
}
