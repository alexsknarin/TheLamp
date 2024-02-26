using System;
using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    [SerializeField] private Collider2D _collider2D;
    public event Action OnCollidedWithLamp;
    
    public void DisableCollider()
    {
        _collider2D.enabled = false;
    }
    
    public void EnableCollider()
    {
        _collider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lamp"))
        {
            OnCollidedWithLamp?.Invoke();
        }
    }
}
