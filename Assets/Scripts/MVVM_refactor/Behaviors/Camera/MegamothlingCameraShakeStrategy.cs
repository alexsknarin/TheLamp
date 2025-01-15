using UnityEngine;

[CreateAssetMenu(fileName = "MegamothlingCameraShakeStrategy", menuName = "Camera Shake/MegamothlingCameraShakeStrategy")]
public class MegamothlingCameraShakeStrategy : BaseCameraShakeStrategy
{
    [SerializeField] private float _megamothlingProximityShakeAmplitude;
    [SerializeField] private AnimationCurve _megamothlingProximityCurve;
    [SerializeField] private float _megamothlingProximityMaxDistance;
    [SerializeField] private float _megamothlingProximityMinDistance;
    private Transform _bossTransform;
    private float _shakeDistance;
    
    public override void Construct(Transform bossTransform)
    {
        Debug.Log("Megamothling Strategy Construct");
        _bossTransform = bossTransform;
    }
    
    public override void Initialize()
    {
        Debug.Log("Megamothling Strategy Initialize");
        _shakeDistance = Mathf.Abs(_megamothlingProximityMaxDistance - _megamothlingProximityMinDistance);
    }

    public override Vector3 Execute()
    {
        Debug.Log("Megamothling Strategy Execute");
        if (_bossTransform.position.z < _megamothlingProximityMaxDistance)
        {
            float shakephase = Mathf.Abs(
                _bossTransform.position.z - _megamothlingProximityMaxDistance) / _shakeDistance;
            Vector3 displace = Vector3.Lerp(
                Vector3.zero, 
                (Vector3)(Random.insideUnitCircle * _megamothlingProximityShakeAmplitude), 
                shakephase
                );
            return displace;
        }
        return Vector3.zero;
    }
}
