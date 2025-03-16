using UnityEngine;

namespace _GAME.Scripts.Enemies.DragonflyProjectileSpider
{
    public class DragonflySpiderWebController : MonoBehaviour
    {
        [SerializeField] private LineRenderer _spiderWebLineRenderer;
        [SerializeField] private float _shrinkDuration;
        private Material _lineMaterial;
        private Transform _emitterTransform;
        private Vector3 _startPoint;
        private Vector3 _endPoint;
        private Vector3 _lastPoint;
        private bool _isShrinking = false;
        private bool _isActive = false; 
        private float _localTime;
    
        public void Initialize()
        {
            _lineMaterial = _spiderWebLineRenderer.material;
            _lineMaterial.SetFloat("_Damage", 0f);
            _spiderWebLineRenderer.positionCount = 2;

            _isShrinking = false;
            _isActive = false;
            _spiderWebLineRenderer.enabled = false;
        }

        public void Play(Transform emitterTransform)
        {
            _spiderWebLineRenderer.enabled = true;
            _spiderWebLineRenderer.positionCount = 2;
            _emitterTransform = emitterTransform;
            _startPoint = _emitterTransform.position;
            _endPoint = _emitterTransform.position;
            _isActive = true;
        }
    
        public void StartShrink()
        {
            _isShrinking = true;
            _lastPoint = _emitterTransform.position;
            _spiderWebLineRenderer.positionCount = 10;
            for(int i=0; i<10; i++)
            {
                _spiderWebLineRenderer.SetPosition(i, Vector3.Lerp(_startPoint, _endPoint, i/9f));
            }
            _localTime = 0;
        }

        private void Update()
        {
            if (_isActive)
            {
                if (!_isShrinking)
                {
                    _endPoint = _emitterTransform.position;
                    _spiderWebLineRenderer.SetPosition(0, _startPoint);
                    _spiderWebLineRenderer.SetPosition(1, _endPoint);    
                }
                else
                {
                    float phase = _localTime / _shrinkDuration;
                    if (phase > 1)
                    {
                        _isActive = false;
                        _isShrinking = false;
                        _spiderWebLineRenderer.positionCount = 0;
                        _spiderWebLineRenderer.enabled = false;
                        return;
                    }
                    _endPoint = Vector3.Lerp(_lastPoint, _startPoint, phase);
                
                    for(int i=0; i<10; i++)
                    {
                        Vector3 newPos = Vector3.Lerp(_startPoint, _endPoint, i/9f);
                        newPos.x += Mathf.PerlinNoise1D(newPos.y) * phase * 3;
                        _spiderWebLineRenderer.SetPosition(i, newPos);
                    }
                    _localTime += Time.deltaTime;
                }
            }
        }
    }
}
