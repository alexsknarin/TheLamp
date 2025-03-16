using System;

public interface IProjectileShooter
{
    public event Action<CollidableEnemy> ProjectileShot;
    public event Action<FEnemy, bool> ProjectileDeactivated;
}
