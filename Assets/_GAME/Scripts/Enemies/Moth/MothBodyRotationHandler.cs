using UnityEngine;

public class MothBodyRotationHandler : MonoBehaviour
{
    [SerializeField] private Transform _bodyTransform;

    void Update()
    {
        Vector3 forwardDirection = -transform.position.normalized;
        Vector3 up = Vector3.up;
        
        _bodyTransform.LookAt(transform.position + forwardDirection, up);
    }
}
