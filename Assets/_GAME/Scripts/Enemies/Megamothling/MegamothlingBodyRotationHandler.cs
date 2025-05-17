using UnityEngine;

public class MegamothlingBodyRotationHandler : MonoBehaviour
{
    [SerializeField] private Transform _bodyTransform;
    private Vector3 _previousPosition = Vector3.zero;
    private Vector3 _currentForwardVelocity;
    
    void Update()
    {
        _currentForwardVelocity = (transform.position - _previousPosition).normalized;  
        _previousPosition = transform.position;
        
        Vector3 up = Vector3.up;
        _bodyTransform.LookAt(_bodyTransform.position + _currentForwardVelocity, up);
    }
}
