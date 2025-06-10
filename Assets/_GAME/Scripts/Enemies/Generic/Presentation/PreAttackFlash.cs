using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class PreAttackFlash : MonoBehaviour, IInitializable
    {
        private static readonly int AttackSemaphore = Shader.PropertyToID("_AttackSemaphore");
        private static readonly int EmissionMultipler = Shader.PropertyToID("_EmissionMultipler");
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        [SerializeField] private bool _enableTrailRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        private List<Material> _bodyMaterials = new ();
        private Material _trailMaterial;

        public void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _bodyMaterials.Add(material);
                material.SetFloat(AttackSemaphore, 0f);
            }

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
            for (int i=0; i < _bodyMaterials.Count; i++)
            {
                _bodyMaterials[i].SetFloat(AttackSemaphore, value);
            }
        }
    }
}
