using System;
using UnityEngine;

public class LampStickZoneCollisionHandler : MonoBehaviour
{
    public event Action<EnemyBase> OnCollidedWithStickyEnemyEvent; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("StickyEnemy")) // TODO: replace with ISticky Interface
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            OnCollidedWithStickyEnemyEvent?.Invoke(enemy);
        }
    }
}
