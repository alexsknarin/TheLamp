using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class HealthIndication : MonoBehaviour, IInitializable
    {
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        [SerializeField] private float _remapMax = 1f;
        private List<Material> _materials = new ();

        public void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _materials.Add(material);
                material.SetFloat("_Health", 1f);
            }
        }

        public void Refresh(int currentHealth, int maxHealth)
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat("_Health", ((float)currentHealth / maxHealth) * _remapMax);
            }
        }
    }
}
