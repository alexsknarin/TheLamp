using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class WingMotionblurMaterialController : MonoBehaviour, IInitializable
    {
        [SerializeField] private MeshRenderer _wingRenderer;
        [Range(0, 5)]
        [SerializeField] private float _verticalMotionBlurShift;
        private Material _motionBlurMaterial;

        public void Initialize()
        {
            _motionBlurMaterial = _wingRenderer.material;
        }

        void Update()
        {
            _motionBlurMaterial.SetFloat("_VerticalBlurShift", _verticalMotionBlurShift);
        }

        private void OnValidate()
        {
            _motionBlurMaterial.SetFloat("_VerticalBlurShift", _verticalMotionBlurShift);
        }
    }
}
