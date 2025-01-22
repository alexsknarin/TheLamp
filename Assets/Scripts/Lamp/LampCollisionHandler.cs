using System;
using UnityEngine;

public class LampCollisionHandler : MonoBehaviour
{
    public event Action<EnemyBase> LampCollidedWithEnemy;
    public event Action<EnemyBase> EnemyExitedLampCollision;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if(enemy.ReadyToCollide)
            {
                enemy.HandleCollisionWithLamp();
                LampCollidedWithEnemy?.Invoke(enemy); 
            }
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            EnemyBase boss = other.GetComponent<EnemyBase>();
            boss.HandleCollisionWithLamp();
            LampCollidedWithEnemy?.Invoke(boss); 
        }
        if (other.gameObject.CompareTag("Dragonfly"))
        {
            EnemyBase dragonfly = other.attachedRigidbody.gameObject.GetComponent<EnemyBase>();
            dragonfly.HandleCollisionWithLamp();
            LampCollidedWithEnemy?.Invoke(dragonfly);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("StickyEnemy"))
        {
            EnemyExitedLampCollision?.Invoke(other.GetComponent<EnemyBase>());
        }
        
        if (other.gameObject.CompareTag("Dragonfly"))
        {
            other.attachedRigidbody.gameObject.GetComponent<Dragonfly>().HandleExitingLampCollisionZone();
        }
    }
}
