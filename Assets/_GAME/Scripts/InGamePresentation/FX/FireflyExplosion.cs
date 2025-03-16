using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.InGamePresentation.FX
{
    public class FireflyExplosion : MonoBehaviour, IInitializable
    {
        [SerializeField] private AnimationCurve _explosionCurve;
        [SerializeField] private AnimationCurve _explosionMaterialCurve;
        [SerializeField] private MeshRenderer _meshRenderer;
        private Material _material;
        private float _phase;
        private float _baseScale = 1;
        private float _localTime;
        private float _duration;
    
        public void Counstruct(float explosionRadius, float duration)
        {
            _baseScale = explosionRadius * 2;
            _duration = duration;
        }

        public void Initialize()
        {
            _material = _meshRenderer.material; // TODO: move to Initialize
            enabled = false;
        }

        public void Play(Vector2 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
            _material.SetFloat("_ExplosionPhase", 0);
            _localTime = 0;
            enabled = true;
        }

        private void Update()
        {
            _phase = _localTime / _duration;
            if (_phase >= 1)
            {
                enabled = false;
                _material.SetFloat("_ExplosionPhase", 0);
                gameObject.SetActive(false);
            }
            else
            {
                float curveValue = _explosionCurve.Evaluate(_phase);
                Vector3 scale = Vector3.one * (curveValue * _baseScale);
                transform.localScale = scale;
                _material.SetFloat("_ExplosionPhase", _explosionMaterialCurve.Evaluate(_phase));
            }
            _localTime += Time.deltaTime;
        }
    }
}
