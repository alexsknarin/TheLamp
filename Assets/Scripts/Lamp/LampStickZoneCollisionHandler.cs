using System;
using UnityEngine;

public class LampStickZoneCollisionHandler : MonoBehaviour
{
    public event Action<EnemyBase> OnCollidedWithStickyEnemy; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("StickyEnemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            OnCollidedWithStickyEnemy?.Invoke(enemy);
        }
    }
}
