using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflyPreAttackFlash : MonoBehaviour, IInitializable
    {
        [SerializeField] private MeshRenderer _bodyMeshRenderer;
        [SerializeField] private MeshRenderer _wingsMeshRenderer;
        private Material _bodyMaterial;
        private Material _wingsMaterial;

        public void Initialize()
        {
            _bodyMaterial = _bodyMeshRenderer.material;
            _wingsMaterial = _wingsMeshRenderer.material;
        
            _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
            _wingsMaterial.SetFloat("_AttackSemaphore", 0f);
        }

        public void PreAttackStart()
        {
            _bodyMaterial.SetFloat("_AttackSemaphore", 1f);
            _wingsMaterial.SetFloat("_AttackSemaphore", 1f);
        }

        public void PreAttackEnd()
        {
            _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
            _wingsMaterial.SetFloat("_AttackSemaphore", 0f);
        
        }
    }
}
