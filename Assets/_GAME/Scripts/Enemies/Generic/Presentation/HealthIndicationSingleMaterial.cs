using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class HealthIndicationSingleMaterial : MonoBehaviour
    {
        private static readonly int Health = Shader.PropertyToID("_Health");
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private float _remapMax = 1f;
        private Material _material;

        public void Initialize()
        {
            _material = _meshRenderer.sharedMaterial;
            
        }

        // TODO: add to other enemies???
        public void Reset()
        {
            _material.SetFloat(Health, 1f);
        }

        public void Refresh(int currentHealth, int maxHealth)
        {
            _material.SetFloat(Health, ((float)currentHealth / maxHealth) * _remapMax);
        }
    }
}
