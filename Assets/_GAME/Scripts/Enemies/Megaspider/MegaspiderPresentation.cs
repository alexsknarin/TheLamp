using System.Collections;
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
        [SerializeField] private PreAttackFlashSingleMaterial _preAttackFlash;
        [SerializeField] private float _preattackDuration = 0.15f;
        
        private WaitForSeconds _preattackDelay;  
        
        public void Initialize()
        {
            _preattackDelay = new (_preattackDuration);
            
            _spiderwebController.Initialize();
            _damageFlash.Initialize();
            _preAttackFlash.Initialize();
            
            _megaspider.Damaged += _damageFlash.Play;
            _movement.PreAttackStarted += StartPreattack;

        }

        private void OnDestroy()
        {
            _megaspider.Damaged -= _damageFlash.Play;
            _movement.PreAttackStarted -= StartPreattack;
        }

        private void StartPreattack()
        {
            _preAttackFlash.PreAttackStart();
            StartCoroutine(StopPreattackDelay());
        }

        private IEnumerator StopPreattackDelay()
        {
            yield return _preattackDelay;
            _preAttackFlash.PreAttackEnd();
        }
    }
}
