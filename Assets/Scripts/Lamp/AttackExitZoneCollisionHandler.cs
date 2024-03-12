using System;
using UnityEngine;

public class AttackExitZoneCollisionHandler : MonoBehaviour
{
    public event Action<Enemy> OnExitAttackExitZone; 
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            OnExitAttackExitZone?.Invoke(enemy);
            enemy.HandleExitingAttackExitZone();
        }
    }
}
