using System;

public interface IProjectileDeactivatedProvider
{
    public event Action<FEnemy> ProjectileDestroyed;
}
