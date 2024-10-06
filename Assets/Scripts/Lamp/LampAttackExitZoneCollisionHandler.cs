using System;
using UnityEngine;

public class LampAttackExitZoneCollisionHandler : MonoBehaviour
{
    public event Action<EnemyBase> OnExitAttackExitZone; 
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            OnExitAttackExitZone?.Invoke(enemy);
            enemy.HandleExitingAttackExitZone();
        }
        if (other.gameObject.CompareTag("Dragonfly"))
        {
            EnemyBase dragonfly = other.attachedRigidbody.gameObject.GetComponent<EnemyBase>();
            OnExitAttackExitZone?.Invoke(dragonfly);
            dragonfly.HandleExitingAttackExitZone();
        }
    }
}
