using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class PreAttackFlashSingleMaterial : MonoBehaviour, IInitializable
    {
        private static readonly int AttackSemaphore = Shader.PropertyToID("_AttackSemaphore");
        private static readonly int EmissionMultipler = Shader.PropertyToID("_EmissionMultipler");
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private bool _enableTrailRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        private Material _bodyMaterial;
        private Material _trailMaterial;

        public void Initialize()
        {
            _bodyMaterial = _meshRenderer.sharedMaterial;
            if (_enableTrailRenderer)
            {
                _trailMaterial = _trailRenderer.material;
                _trailMaterial.SetFloat(EmissionMultipler, 0f);
            }
        }

        public void PreAttackStart()
        {
            SetBodyAttackSemaphore(1f);
            if (_enableTrailRenderer)
            {
                _trailMaterial.SetFloat(EmissionMultipler, .1f);    
            }
        }

        public void PreAttackEnd()
        {
            SetBodyAttackSemaphore(0f);
            if (_enableTrailRenderer)
            {
                _trailMaterial.SetFloat(EmissionMultipler, 0f);    
            }
        
        }

        private void SetBodyAttackSemaphore(float value)
        {
            _bodyMaterial.SetFloat(AttackSemaphore, value);
        }
    }
}
