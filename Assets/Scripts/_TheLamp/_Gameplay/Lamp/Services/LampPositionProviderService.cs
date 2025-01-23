using UnityEngine;

public class LampPositionProviderService : MonoBehaviour, ILampPositionProviderService
{
    private Vector3 _lampPosition;
    
    public Vector3 GetLampPosition()
    {
        return _lampPosition;
    }

    private void FixedUpdate()
    {
        _lampPosition = transform.position;
    }
}
