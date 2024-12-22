using System;
using UnityEngine;

public class LampStickZoneCollisionHandler : MonoBehaviour
{
    public event Action<EnemyBase> OnCollidedWithStickyEnemyEvent;
    public static event Action<EnemyBase> OnCollidedWithStickyEnemyStaticEvent; // TODO: remove it after refactoring
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("StickyEnemy")) // TODO: replace with ISticky Interface
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            OnCollidedWithStickyEnemyEvent?.Invoke(enemy);
            OnCollidedWithStickyEnemyStaticEvent?.Invoke(enemy); // TODO: remove it after refactoring
        }
    }
}
