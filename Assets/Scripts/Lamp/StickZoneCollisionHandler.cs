using System;
using UnityEngine;

public class StickZoneCollisionHandler : MonoBehaviour
{
    public event Action<Enemy> OnCollidedWithStickyEnemy; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("StickyEnemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.HandleCollisionWithStickZone();
            OnCollidedWithStickyEnemy?.Invoke(other.GetComponent<Enemy>());
        }
    }
}
