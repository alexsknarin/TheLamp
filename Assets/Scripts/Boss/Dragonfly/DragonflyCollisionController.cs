using System;
using UnityEngine;

public class DragonflyCollisionController : MonoBehaviour
{
    [SerializeField] private CircleCollider2D[] _colliders; 
    private bool _isCollided = false; 
        
    public void EnableColliders()
    {
        foreach (var col in _colliders)
        {
            col.enabled = true;
        }
        _isCollided = false;
    }
    
    public void DisableColliders()
    {
        foreach (var col in _colliders)
        {
            col.enabled = false;
        }
    }

    public void SoloCollider()
    {
        if (!_isCollided)
        {
            var closesCollider = _colliders[0];
            float closestDistance = closesCollider.transform.position.magnitude;

            foreach (var col in _colliders)
            {
                float distance = col.transform.position.magnitude;
                if (distance < closestDistance)
                {
                    closesCollider = col;
                    closestDistance = distance;
                }
            }

            foreach (var col in _colliders)
            {
                if (col == closesCollider)
                {
                    col.enabled = true;
                }
                else
                {
                    col.enabled = false;
                }
            }
            _isCollided = true;
        }
    }
    
    public Vector3 GetFirstActiveColliderPosition()
    {
        foreach (var col in _colliders)
        {
            if (col.enabled)
            {
                return col.transform.position;
            }
        }
        return Vector3.zero;
    }
    
    public Transform GetFirstActiveColliderTransform()
    {
        foreach (var col in _colliders)
        {
            if (col.enabled)
            {
                return col.transform;
            }
        }
        return null;
    }

    private void Update()
    {

    }
}
