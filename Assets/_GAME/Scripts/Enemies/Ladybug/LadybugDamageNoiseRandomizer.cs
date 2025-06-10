using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug
{
    public class LadybugDamageNoiseRandomizer : MonoBehaviour, IInitializable
    {
        private static readonly int NoiseOffset = Shader.PropertyToID("_NoiseOffset");
        [SerializeField] private List<MeshRenderer> _meshRenderer;
    
        public void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                material.SetFloat(NoiseOffset, Random.Range(0f, 100f));
            }
        }
    }
}
