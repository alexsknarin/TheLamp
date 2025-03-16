using UnityEngine;

namespace _GAME.Scripts.InGamePresentation.CameraShake
{
    [CreateAssetMenu(fileName = "MegabeetleCameraShakeStrategy", menuName = "Camera Shake/MegabeetleCameraShakeStrategy")]
    public class MegabeetleCameraShakeStrategy: BaseCameraShakeStrategy
    {
        [SerializeField] private float _megabeetleProximityShakeAmplitude;
        [SerializeField] private AnimationCurve _megabeetleProximityCurve;
        [SerializeField] private float _megabeetleProximityMaxDistance;
        [SerializeField] private float _megabeetleProximityMinDistance;
        private Transform _bossTransform;
        private float _shakeDistance;

        public override void Construct(Transform bossTransform)
        {
            _bossTransform = bossTransform;
        }

        public override void Initialize()
        {
            _shakeDistance = Mathf.Abs(_megabeetleProximityMaxDistance - _megabeetleProximityMinDistance);
        }

        public override Vector3 Execute()
        {
            if (_bossTransform.position.z < _megabeetleProximityMaxDistance)
            {
                float shakephase = Mathf.Abs(_bossTransform.position.z - _megabeetleProximityMaxDistance) / _shakeDistance;
                Vector3 displace = Vector3.Lerp(
                    Vector3.zero, 
                    (Vector3)(Random.insideUnitCircle * _megabeetleProximityShakeAmplitude), 
                    shakephase
                );
                return displace;
            }
            return Vector3.zero;
        }
    }
}
