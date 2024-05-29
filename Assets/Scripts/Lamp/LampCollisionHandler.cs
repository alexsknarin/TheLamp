using System;
using UnityEngine;

public class LampCollisionHandler : MonoBehaviour
{
    public event Action<EnemyBase> OnLampCollidedEnemy;
    public event Action<EnemyBase> OnExitLampCollisionEnemy;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if(enemy.ReadyToCollide)
            {
                enemy.HandleCollisionWithLamp();
                OnLampCollidedEnemy?.Invoke(enemy); 
            }
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            EnemyBase boss = other.GetComponent<EnemyBase>();
            boss.HandleCollisionWithLamp();
            OnLampCollidedEnemy?.Invoke(boss); 
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("StickyEnemy"))
        {
            OnExitLampCollisionEnemy?.Invoke(other.GetComponent<EnemyBase>());
        }
    }
}
