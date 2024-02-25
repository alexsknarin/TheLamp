using System;
using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    public event Action OnCollidedWithLamp;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lamp"))
        {
            OnCollidedWithLamp?.Invoke();
        }
    }
}
