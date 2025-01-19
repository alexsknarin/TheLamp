using UnityEngine;

public class LampPositionProviderService : ILampPositionProviderService
{
    private Transform _lampTransform;
    public LampPositionProviderService(Transform lampTransform)
    {
        _lampTransform = lampTransform;
    }
    
    public Vector3 GetLampPosition()
    {
        return _lampTransform.position;
    }
}
