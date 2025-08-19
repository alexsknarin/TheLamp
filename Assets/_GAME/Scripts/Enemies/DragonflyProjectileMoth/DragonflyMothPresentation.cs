using System;
using _GAME.Scripts.Enemies.Generic.Presentation;
using UnityEngine;

namespace _GAME.Scripts.Enemies.DragonflyProjectileMoth
{
    public class DragonflyMothPresentation : MonoBehaviour
    {
        private static readonly int ToFly = Animator.StringToHash("ToFly");
        private static readonly int ToStatic = Animator.StringToHash("ToStatic");
        [SerializeField] private DeathFlash _deathFlash;
        [SerializeField] private DragonflyMothBodyRotationHandler _bodyRotation;
        [SerializeField] private DragonflyProjectileMovementMoth _movement;
        [SerializeField] private Animator _wingsAnimator;
    
        public void Initialize()
        {
            _deathFlash.Initialize();
            _bodyRotation.Initialize();
            
            _movement.AttackStarted += OnAttackStarted;
            _movement.FallStarted += OnFallStarted;
        }

        private void OnDestroy()
        {
            _movement.AttackStarted -= OnAttackStarted;
            _movement.FallStarted -= OnFallStarted;

        }

        public void DeathFlash()
        {
            _deathFlash.Play();
        }

        private void OnAttackStarted()
        {
            _wingsAnimator.SetTrigger(ToFly);
        }

        private void OnFallStarted()
        {
            _wingsAnimator.SetTrigger(ToStatic);
        }
    }
}
