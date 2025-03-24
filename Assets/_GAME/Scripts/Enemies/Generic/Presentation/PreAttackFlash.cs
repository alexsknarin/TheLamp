using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class PreAttackFlash : MonoBehaviour, IInitializable
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private bool _enableTrailRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        private Material _bodyMaterial;
        private Material _trailMaterial;

        public void Initialize()
        {
            _bodyMaterial = _meshRenderer.material;
            if (_enableTrailRenderer)
            {
                _trailMaterial = _trailRenderer.material;
                _trailMaterial.SetFloat("_EmissionMultipler", 0f);
            }
            _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
        }

        public void PreAttackStart()
        {
            _bodyMaterial.SetFloat("_AttackSemaphore", 1f);
            if (_enableTrailRenderer)
            {
                _trailMaterial.SetFloat("_EmissionMultipler", .1f);    
            }
        }

        public void PreAttackEnd()
        {
            _bodyMaterial.SetFloat("_AttackSemaphore", 0f);   
            if (_enableTrailRenderer)
            {
                _trailMaterial.SetFloat("_EmissionMultipler", 0f);    
            }
        
        }
    }
}
