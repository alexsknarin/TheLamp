using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderPresentation : MonoBehaviour, IInitializable
    {
        [SerializeField] private Megaspider _megaspider;
        [SerializeField] private MegaspiderMovement _movement;
        [SerializeField] private MegaspiderAnimationClipEventListener _animationClipEvents;
        [SerializeField] private MegaspiderSpiderwebController _spiderwebController;
        [SerializeField] private DamageFlashSingleMaterial _damageFlash;
        
        public void Initialize()
        {
            _spiderwebController.Initialize();
            _damageFlash.Initialize();
            
            _megaspider.Damaged += _damageFlash.Play;
            
            Debug.Log("Megaspider initialized");
        }

        private void OnDestroy()
        {
            _megaspider.Damaged -= _damageFlash.Play;
        }
    }
}
