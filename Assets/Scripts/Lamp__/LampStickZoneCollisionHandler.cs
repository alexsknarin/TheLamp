using System;
using UnityEngine;

public class LampStickZoneCollisionHandler : MonoBehaviour
{
    public event Action<EnemyBase> CollidedWithStickyEnemy;
    public static event Action<EnemyBase> CollidedWithStickyEnemyStatic; // TODO: remove it after refactoring
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("StickyEnemy")) // TODO: replace with ISticky Interface
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            CollidedWithStickyEnemy?.Invoke(enemy);
            CollidedWithStickyEnemyStatic?.Invoke(enemy); // TODO: remove it after refactoring
        }
    }
}
