using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyCameraShakeStrategy", menuName = "Camera Shake/DragonflyCameraShakeStrategy")]
public class DragonflyCameraShakeStrategy: BaseCameraShakeStrategy
{
    [SerializeField] private float _dragonflyProximityShakeAmplitude;
    [SerializeField] private AnimationCurve _dragonflyProximityCurve;
    [SerializeField] private float _dragonflyProximityMaxDistance;
    [SerializeField] private float _dragonflyProximityMinDistance;
    private Transform _bossTransform;
    private float _shakeDistance;
    
    public override void Construct(Transform bossTransform)
    {
        Debug.Log("Dragonfly Strategy Construct");
        _bossTransform = bossTransform;
    }

    public override void Initialize()
    {
        Debug.Log("Dragonfly Strategy Initialize");
        _shakeDistance = Mathf.Abs(_dragonflyProximityMaxDistance - _dragonflyProximityMinDistance);
    }

    public override Vector3 Execute()
    {
        Debug.Log("Dragonfly Strategy Execute");
        if (_bossTransform.position.z < _dragonflyProximityMaxDistance)
        {
            float shakePhase = Mathf.Abs(_bossTransform.position.z - _dragonflyProximityMaxDistance) / _shakeDistance;
            Vector3 displace = Vector3.Lerp(
                Vector3.zero,
                (Vector3)(Random.insideUnitCircle * _dragonflyProximityShakeAmplitude), 
                shakePhase
                );
            return displace;
        }
        return Vector3.zero;
    }
}
