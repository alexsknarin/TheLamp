using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflySwarmCallFX : MonoBehaviour
    {
        [SerializeField] private GameObject _swarmWave1Object;
        [SerializeField] private float duration = 1.7f;
        [SerializeField] private float _maxSize = 3f;
        [SerializeField] private AnimationCurve _sizeCurve;
        private Material _material1;
        private bool _isActive = false;
        private float _localTime;

        public void Initialize()
        {
            _material1 = _swarmWave1Object.GetComponent<MeshRenderer>().material;
            _material1.SetFloat("_Transparency", 0f);
            _swarmWave1Object.SetActive(false);
            _isActive = false;
        }

        public void Play()
        {
            _swarmWave1Object.SetActive(true);
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
                    _material1.SetFloat("_Transparency", 0f);
                    _swarmWave1Object.SetActive(false);
                    return;
                }
                _material1.SetFloat("_Transparency", 1-phase);
                Vector3 scale = Vector3.one * (_sizeCurve.Evaluate(phase) * _maxSize);
                _swarmWave1Object.transform.localScale = scale;
                _localTime += Time.deltaTime;
            }
        }
    
    
    }
}
