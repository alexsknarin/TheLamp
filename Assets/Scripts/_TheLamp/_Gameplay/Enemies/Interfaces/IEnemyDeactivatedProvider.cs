using System;

public interface IEnemyDeactivatedProvider
{
    public event Action<FEnemy> EnemyReleasedToPool;
}