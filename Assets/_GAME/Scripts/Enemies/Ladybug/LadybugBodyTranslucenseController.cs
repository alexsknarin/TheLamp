using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug
{
    public class LadybugBodyTranslucenseController : MonoBehaviour, IInitializable
    {
        private static readonly int TranslucenceWrap = Shader.PropertyToID("_TranslucenceWrap");
        private static readonly int TranslucenseStrength = Shader.PropertyToID("_TranslucenseStrength");
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        private List<Material> _materials = new ();
    
        public void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _materials.Add(material);
            }
        }

        public void SetRegular()
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat(TranslucenceWrap , 0.7f);
                _materials[i].SetFloat(TranslucenseStrength , 1.0f);
            
            }
        }
    
        public void SetStick()
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat(TranslucenceWrap , 0.93f);
                _materials[i].SetFloat(TranslucenseStrength , 1.8f);
            
            }
        }
    }
}
