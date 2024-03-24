using System;
using UnityEngine;

public class StickZoneCollisionHandler : MonoBehaviour
{
    public event Action<EnemyBase> OnCollidedWithStickyEnemy; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("StickyEnemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            enemy.HandleCollisionWithStickZone();
            OnCollidedWithStickyEnemy?.Invoke(other.GetComponent<EnemyBase>());
        }
    }
}
