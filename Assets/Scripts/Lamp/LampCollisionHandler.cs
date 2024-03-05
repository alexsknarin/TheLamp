using System;
using UnityEngine;

public class LampCollisionHandler : MonoBehaviour
{
    public event Action OnLampCollidedEnemy;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            OnLampCollidedEnemy?.Invoke();
        }
    }
}
