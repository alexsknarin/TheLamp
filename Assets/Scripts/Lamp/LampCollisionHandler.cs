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
            OnLampCollidedEnemy?.Invoke(other.GetComponent<Enemy>());
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            OnExitLampCollisionEnemy?.Invoke(other.GetComponent<Enemy>());
        }
    }
}
