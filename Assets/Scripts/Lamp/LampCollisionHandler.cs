using System;
using UnityEngine;

public class LampCollisionHandler : MonoBehaviour
{
    public event Action<Enemy> OnLampCollidedEnemy;
    public event Action<Enemy> OnExitLampCollisionEnemy;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy.ReadyToCollide)
            {
                enemy.HandleCollisionWithLamp();
                OnLampCollidedEnemy?.Invoke(other.GetComponent<Enemy>()); 
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("StickyEnemy"))
        {
            OnExitLampCollisionEnemy?.Invoke(other.GetComponent<Enemy>());
        }
    }
}
