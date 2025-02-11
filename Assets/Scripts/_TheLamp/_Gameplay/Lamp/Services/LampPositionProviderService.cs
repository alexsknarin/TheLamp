using UnityEngine;

public class LampPositionProviderService : MonoBehaviour, ILampPositionProviderService
{
    private Vector2 _lampPosition;
    
    public Vector2 GetLampPosition()
    {
        return _lampPosition;
    }

    private void FixedUpdate()
    {
        _lampPosition = transform.position;
    }
}
