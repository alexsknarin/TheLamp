using System;

public interface IAnimatedEnemy
{
    public event Action<CollidableEnemy> AnimatedAttackStarted;
}
