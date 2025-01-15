using UnityEngine;

[CreateAssetMenu(fileName = "WaspCameraShakeStrategy", menuName = "Camera Shake/WaspCameraShakeStrategy")]
public class WaspCameraShakeStrategy : BaseCameraShakeStrategy
{
    [SerializeField] private float _waspProximityShakeAmplitude;
    [SerializeField] private AnimationCurve _waspProximityCurve;
    [SerializeField] private float _waspProximityMaxDistance;
    [SerializeField] private float _waspProximityMinDistance;
    private Transform _bossTransform;
    private float _shakeDistance;
    
    public override void Construct(Transform bossTransform)
    {
        Debug.Log("Wasp Strategy Construct");
        _bossTransform = bossTransform;
    }
    
    public override void Initialize()
    {
        Debug.Log("Wasp Strategy Initialize");
        _shakeDistance = Mathf.Abs(_waspProximityMaxDistance - _waspProximityMinDistance);
    }
    
    public override Vector3 Execute()
    {
        Debug.Log("Wasp Strategy Execute");
        if (_bossTransform.position.z < _waspProximityMaxDistance)
        {
            float shakePhase = Mathf.Abs(_bossTransform.position.z - _waspProximityMaxDistance) / _shakeDistance;
            Vector3 displace = Vector3.Lerp(
                Vector3.zero,
                (Vector3)(Random.insideUnitCircle * _waspProximityShakeAmplitude), 
                shakePhase
                );
            return displace;
        }
        return Vector3.zero;
    }
}
