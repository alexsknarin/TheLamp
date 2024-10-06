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
        Debug.Log("Disabling colliders");
        foreach (var col in _colliders)
        {
            col.enabled = false;
        }
    }

    public void SoloCollider(Collider2D collider)
    {
        Debug.Log("Trying to solo a collider");
        if (!_isCollided)
        {
            foreach (var col in _colliders)
            {
                if (col == collider)
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
}
