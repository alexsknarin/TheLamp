using System;
using _GAME.Scripts.Enemies;
using UnityEngine;

public interface IForcedCollidableEnemy
{
    public event Action<CollidableEnemy> CollisionRequested;
}
