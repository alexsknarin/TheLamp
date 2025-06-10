using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megamothling
{
    public class MegamothlingFurMovement : MonoBehaviour, IInitializable
    {
        private static readonly int FurWindOffset = Shader.PropertyToID("_FurWindOffset");
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private AnimationCurve _hitFurAnimationCurve;
        [SerializeField] private float _hitFurAnimationDuration;
        private Material _material;
        
        private float _localTime;
        
        public void Initialize()
        {
            _material = _meshRenderer.material;
            enabled = false;
        }

        public void Reset()
        {
            _material.SetFloat(FurWindOffset, 0f);
        }
        
        public void Attack()
        {
            _material.SetFloat(FurWindOffset, -1f);
        }
        
        public void Hit()
        {
            _material.SetFloat(FurWindOffset, 1f);
            _localTime = 0f;
            enabled = true;
        }

        private void Update()
        {
            float phase = _localTime / _hitFurAnimationDuration;
            _material.SetFloat(FurWindOffset, _hitFurAnimationCurve.Evaluate(phase));
            _localTime += Time.deltaTime;
            
            if (phase > 1)
            {
                enabled = false;
            }
        }
    }
}
