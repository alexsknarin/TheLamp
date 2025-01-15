using UnityEngine;

[CreateAssetMenu(fileName = "WaspCameraShakeStrategy", menuName = "Camera Shake/WaspCameraShakeStrategy")]
public class WaspCameraShakeStrategy : ScriptableObject, ICameraShakeStrategy, IInitializable
{
    [SerializeField] private float _waspProximityShakeAmplitude;
    [SerializeField] private AnimationCurve _waspProximityCurve;
    [SerializeField] private float _waspProximityMaxDistance;
    [SerializeField] private float _waspProximityMinDistance;
    private Transform _bossTransform;
    private float _waspShakeDistance;
    
    public void Construct(Transform bossTransform)
    {
        _bossTransform = bossTransform;
    }
    
    public void Initialize()
    {
        _waspShakeDistance = Mathf.Abs(_waspProximityMaxDistance - _waspProximityMinDistance);
    }
    
    public Vector3 Execute()
    {
        Debug.Log("Wasp Strategy Execute");
        if (_bossTransform.position.z < _waspProximityMaxDistance)
        {
            float shakePhase = Mathf.Abs(_bossTransform.position.z - _waspProximityMaxDistance) / _waspShakeDistance;
            Vector3 displace = Vector3.Lerp(
                Vector3.zero, Vector3.zero + (Vector3)(Random.insideUnitCircle * _waspProximityShakeAmplitude), 
                shakePhase
                );
            return displace;
        }
        return Vector3.zero;
    }
}
