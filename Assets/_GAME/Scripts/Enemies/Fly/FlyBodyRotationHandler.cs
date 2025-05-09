using UnityEngine;

public class FlyBodyRotationHandler : MonoBehaviour
{
    [SerializeField] private Transform _bodyTransform;
    private Vector3 _previousPosition;
    
    
    public void Reset()
    {
        _previousPosition = transform.position;
        // _currentUpTarget = _enterUpTarget;
        // _isTransitionMode = false;
    }
    
    void Start()
    {
        Reset();
    }

    void Update()
    {
        Vector3 forwardVelocity = Vector3.zero;
        forwardVelocity = (transform.position - _previousPosition).normalized;
        
        Vector3 up = Vector3.up;
        
        _bodyTransform.LookAt(_bodyTransform.position + forwardVelocity, up);
        
        _previousPosition = transform.position;
    }
}
