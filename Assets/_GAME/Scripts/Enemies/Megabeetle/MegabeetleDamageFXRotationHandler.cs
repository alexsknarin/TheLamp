using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle
{
    public class MegabeetleDamageFXRotationHandler : MonoBehaviour, IInitializable
    {
        [SerializeField] private MegabeetleMovement _movement;
        [SerializeField] private Transform _damageTransform;
        
        private Vector3 _forward;
        private Vector3 _up = Vector3.up;

        // Dependencies

        private ILampPositionProviderService _lampPositionProvider;

        public void Construct(ILampPositionProviderService lampPositionProvider)
        {
            _lampPositionProvider = lampPositionProvider;
        }

        public void Initialize()
        {
            enabled = false;
        }

        public void Play()
        {
            enabled = true;
        }
        
        public void Stop()
        {
            enabled = false;
        }

        private void Update()
        {
            _forward = (_damageTransform.position - (Vector3)_lampPositionProvider.GetLampPosition()).normalized;
            _damageTransform.LookAt(transform.position + _forward, _up);
        }
    }
}
