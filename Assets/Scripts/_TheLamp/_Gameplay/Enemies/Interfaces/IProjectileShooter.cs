using System;

public interface IProjectileShooter
{
    public event Action<CollidableEnemy> ProjectileShot;
}
