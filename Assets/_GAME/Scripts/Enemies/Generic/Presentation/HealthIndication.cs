using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class HealthIndication : MonoBehaviour, IInitializable
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        private Material _material;

        public void Initialize()
        {
            _material = _meshRenderer.material;
            _material.SetFloat("_Health", 1f);
        }

        public void Refresh(int currentHealth, int maxHealth)
        {
            _material.SetFloat("_Health", (float)currentHealth / maxHealth);
        }
    }
}
