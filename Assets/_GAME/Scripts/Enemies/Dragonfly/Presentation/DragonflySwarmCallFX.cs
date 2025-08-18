using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflySwarmCallFX : MonoBehaviour
    {
        [SerializeField] private GameObject _swarmWave1Object;
        [SerializeField] private float duration = 1.7f;
        [Header("Shock Wave")]
        [SerializeField] private FullScreenPassRendererFeature _rendererFeature;
        [SerializeField] private Transform _emitterTransform;
        [SerializeField] private AnimationCurve _shockWaveCurve;
        private Material _material1;
        private Material _shockWaveMaterial;
        private bool _isActive = false;
        private float _localTime;
        
        // Dependencies
        private Camera _camera;

        public void Initialize()
        {
            _material1 = _swarmWave1Object.GetComponent<MeshRenderer>().material;
            _shockWaveMaterial = _rendererFeature.passMaterial;
            _rendererFeature.SetActive(false);
            
            _camera = Camera.main;

            Reset();
        }

        public void Reset()
        {
            _material1.SetFloat("_Phase", 0f);
            _swarmWave1Object.SetActive(false);
            _isActive = false;
        }
        
        public void Play()
        {
            _swarmWave1Object.SetActive(true);
            _rendererFeature.SetActive(true);
            _localTime = 0f;
            _isActive = true;
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.B))
            {
                Play();
            }
        
            if (_isActive)
            {
                float phase = _localTime / duration;
                if (phase > 1)
                {
                    _isActive = false;
                    _material1.SetFloat("_Phase", 0f);
                    _shockWaveMaterial.SetFloat("_Phase", 0f);
                    _swarmWave1Object.SetActive(false);
                    _rendererFeature.SetActive(false);
                    return;
                }
                
                Vector2 screenPos = _camera.WorldToScreenPoint(_emitterTransform.position);
                screenPos.x /= Screen.width;
                screenPos.y /= Screen.height;
                
                _shockWaveMaterial.SetFloat("_Phase", _shockWaveCurve.Evaluate(phase));
                _shockWaveMaterial.SetVector("_Point", screenPos);
                
                
                _material1.SetFloat("_Phase", -phase);
                _localTime += Time.deltaTime;
            }
        }
    
    
    }
}
