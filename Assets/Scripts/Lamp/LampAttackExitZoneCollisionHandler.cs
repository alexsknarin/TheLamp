using System;
using UnityEngine;

public class LampAttackExitZoneCollisionHandler : MonoBehaviour
{
    public event Action<EnemyBase> OnExitAttackExitZoneEvent; 
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            OnExitAttackExitZoneEvent?.Invoke(enemy);
            enemy.HandleExitingAttackExitZone();
        }
        
        if (other.gameObject.CompareTag("Dragonfly"))
        {
            EnemyBase dragonfly = other.attachedRigidbody.gameObject.GetComponent<Dragonfly>();
            OnExitAttackExitZoneEvent?.Invoke(dragonfly);
            dragonfly.HandleExitingAttackExitZone();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Dragonfly"))
        {
            other.attachedRigidbody.gameObject.GetComponent<Dragonfly>().HandleEnteringAttackExitZone();
        }
    }
}
