using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug
{
    public class LadybugDamageNoiseRandomizer : MonoBehaviour, IInitializable
    {
        [SerializeField] private List<MeshRenderer> _meshRenderer;
    
        public void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                material.SetFloat("_NoiseOffset", Random.Range(0f, 100f));
            }
        }
    }
}
