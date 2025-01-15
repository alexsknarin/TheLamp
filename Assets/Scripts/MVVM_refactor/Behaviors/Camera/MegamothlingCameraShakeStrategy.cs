using UnityEngine;

[CreateAssetMenu(fileName = "MegamothlingCameraShakeStrategy", menuName = "Camera Shake/MegamothlingCameraShakeStrategy")]
public class MegamothlingCameraShakeStrategy : ScriptableObject, IInitializable, ICameraShakeStrategy
{
    [SerializeField] private float _megamothlingProximityShakeAmplitude;
    [SerializeField] private AnimationCurve _megamothlingProximityCurve;
    [SerializeField] private float _megamothlingProximityMaxDistance;
    [SerializeField] private float _megamothlingProximityMinDistance;
    private Transform _bossTransform;
    private float _megamothlingShakeDistance;
    
    public void Construct(Transform bossTransform)
    {
        _bossTransform = bossTransform;
    }
    
    public void Initialize()
    {
        _megamothlingShakeDistance = Mathf.Abs(_megamothlingProximityMaxDistance - _megamothlingProximityMinDistance);
    }

    public Vector3 Execute()
    {
        Debug.Log("Megamothling Strategy Execute");
        if (_bossTransform.position.z < _megamothlingProximityMaxDistance)
        {
            float shakephase = Mathf.Abs(
                _bossTransform.position.z - _megamothlingProximityMaxDistance) / _megamothlingShakeDistance;
            Vector3 displace = Vector3.Lerp(
                Vector3.zero, Vector3.zero + (Vector3)(Random.insideUnitCircle * _megamothlingProximityShakeAmplitude), 
                shakephase
                );
            return displace;
        }
        return Vector3.zero;
    }
}
