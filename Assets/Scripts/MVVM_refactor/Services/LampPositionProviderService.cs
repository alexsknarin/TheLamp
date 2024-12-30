using UnityEngine;

public class LampPositionProviderService : MonoBehaviour, ILampPositionProviderService
{
    public Vector3 GetLampPosition()
    {
        return transform.position;
    }
}
