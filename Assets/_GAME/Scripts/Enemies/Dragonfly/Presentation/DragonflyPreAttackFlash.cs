using System;
using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflyPreAttackFlash : MonoBehaviour, IInitializable
    {
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        private List<Material> _materials = new ();

        public void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _materials.Add(material);
                material.SetFloat("_AttackSemaphore", 0f);
            }
        
            Reset();
        }

        public void Reset()
        {
            SetAttackSemaphore(0);
        }

        public void PreAttackStart()
        {
            SetAttackSemaphore(1f);
        }

        public void PreAttackEnd()
        {
            SetAttackSemaphore(0);
        
        }
        
        private void SetAttackSemaphore(float value)
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat("_AttackSemaphore", value);
            }
        }
    }
}
