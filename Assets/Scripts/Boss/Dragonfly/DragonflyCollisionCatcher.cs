using System;
using UnityEngine;

public class DragonflyCollisionCatcher : MonoBehaviour
{
    [SerializeField] private GameObject _colliders;
    
    public event Action OnCollidedEvent;
    
    public void EnableColliders()
    {
        _colliders.SetActive(true);
    }
    
    public void DisableColliders()
    {
        _colliders.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnCollidedEvent?.Invoke();
    }
}
