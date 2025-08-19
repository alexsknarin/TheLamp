using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflySwarmCallFX : MonoBehaviour
    {
        [SerializeField] private GameObject _swarmWave1Object;
        [SerializeField] private float duration = 1.7f;
        [Header("Shock Wave")]
        [SerializeField] private Transform _emitterTransform;
        [SerializeField] private AnimationCurve _shockWaveCurve;
        private Material _shockWaveMaterial;
        private Material _swarmAirFxMaterial;
        private bool _isActive = false;
        private float _localTime;

        // Dependencies
        private FullScreenPassRendererFeature _fullscreenRendererFeature;
        private Camera _camera;

        public void Construct(FullScreenPassRendererFeature rendererFeature, Camera currentCamera)
        {
            _fullscreenRendererFeature = rendererFeature;
            _camera = currentCamera;
        }

        public void Initialize()
        {
            _fullscreenRendererFeature.SetActive(false);
            _shockWaveMaterial = _fullscreenRendererFeature.passMaterial;
            _swarmAirFxMaterial = _swarmWave1Object.GetComponent<MeshRenderer>().material;
            Reset();
        }

        public void Reset()
        {
            _swarmAirFxMaterial.SetFloat("_Phase", 0f);
            _swarmWave1Object.SetActive(false);
            _isActive = false;
        }
        
        public void Play()
        {
            _swarmWave1Object.SetActive(true);
            _localTime = 0f;
            _isActive = true;
            _fullscreenRendererFeature.SetActive(true);
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (_isActive)
            {
                float phase = _localTime / duration;
                if (phase > 1)
                {
                    _isActive = false;
                    _swarmAirFxMaterial.SetFloat("_Phase", 0f);
                    _swarmWave1Object.SetActive(false);
                    
                    _fullscreenRendererFeature.SetActive(false);
                    _shockWaveMaterial.SetFloat("_Phase", 0f);    
                    
                    return;
                }
                
                Vector2 screenPos = _camera.WorldToScreenPoint(_emitterTransform.position);
                screenPos.x /= Screen.width;
                screenPos.y /= Screen.height;
                _shockWaveMaterial.SetFloat("_Phase", _shockWaveCurve.Evaluate(phase));
                _shockWaveMaterial.SetVector("_Point", screenPos);
                
                _swarmAirFxMaterial.SetFloat("_Phase", -phase);
                _localTime += Time.deltaTime;
            }
        }
    }
}
