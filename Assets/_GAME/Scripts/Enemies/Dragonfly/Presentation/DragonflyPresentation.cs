using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflyPresentation : MonoBehaviour, IInitializable
    {
        [SerializeField] private Dragonfly _dragonfly;
        [SerializeField] private DragonflyMovement _dragonflyMovement;
        [SerializeField] private DragonflyDamageFlash _damageIndication;
        [SerializeField] private DragonflyHealthIndication _healthIndication;
        [SerializeField] private DragonflyDeathFlash _deathFlash;
        [SerializeField] private DragonflyPreAttackFlash _preAttackFlash;
        [SerializeField] private DragonflySwarmCallFX _swarmCallFX;
        
        [Header("------ Animation ------")]
        [SerializeField] private Animator _animatorBody;
        
        public void Initialize()
        {
            _damageIndication.Initialize();
            _healthIndication.Initialize();
            _deathFlash.Initialize();
            _preAttackFlash.Initialize();
            _swarmCallFX.Initialize();
            
            _dragonfly.Started += OnDragonflyStarted;
            _dragonflyMovement.PreAttackStarted += OnPreAttackStarted;
            _dragonflyMovement.AttackStarted += OnAttackStarted;
            _dragonfly.Damaged += OnDamaged;
            _dragonfly.Died += OnDied;
            _dragonfly.HealthChanged += OnHealthChanged;
            _dragonfly.SwarmCalled += OnSwarmCalled;
            _dragonfly.ColliderTransformChanged += OnColliderTransformChanged;
            
            // Anim Events
            _dragonflyMovement.EnterToPatrolLStarted += OnEnterToPatrolLStarted;
            _dragonflyMovement.EnterToPatrolRStarted += OnEnterToPatrolRStarted;
            _dragonflyMovement.PatrolLRStarted += OnToPatrolLRStarted;
            _dragonflyMovement.PreAttackHeadLRStarted += OnPreAttackHeadLRStarted;
            _dragonflyMovement.BounceStarted += OnBounceStarted;
            _dragonflyMovement.PreAttackTailLRStarted += OnPreAttackTailLRStarted;

            _dragonflyMovement.EnterToHoverLStarted += OnEnterToHoverLStarted;
            _dragonflyMovement.EnterToHoverRStarted += OnEnterToHoverRStarted;
            _dragonflyMovement.HoverStarted += OnHoverStarted;
            _dragonflyMovement.PreAttackHoverStarted += OnPreAttackHoverStarted;
            _dragonflyMovement.ReturnHoverStarted += OnReturnHoverStarted;
        }

        private void OnDestroy()
        {
            _dragonfly.Started += OnDragonflyStarted;
            _dragonflyMovement.PreAttackStarted -= OnPreAttackStarted;
            _dragonflyMovement.AttackStarted -= OnAttackStarted;
            _dragonfly.Damaged -= OnDamaged;
            _dragonfly.Died -= OnDied;
            _dragonfly.HealthChanged -= OnHealthChanged;
            _dragonfly.SwarmCalled -= OnSwarmCalled;
            _dragonfly.ColliderTransformChanged -= OnColliderTransformChanged;
            
            // Anim Events
            _dragonflyMovement.EnterToPatrolLStarted -= OnEnterToPatrolLStarted;
            _dragonflyMovement.EnterToPatrolRStarted -= OnEnterToPatrolRStarted;
            _dragonflyMovement.PatrolLRStarted -= OnToPatrolLRStarted;
            _dragonflyMovement.PreAttackHeadLRStarted -= OnPreAttackHeadLRStarted;
            _dragonflyMovement.BounceStarted -= OnBounceStarted;
            _dragonflyMovement.PreAttackTailLRStarted -= OnPreAttackTailLRStarted;
            
            _dragonflyMovement.EnterToHoverLStarted -= OnEnterToHoverLStarted;
            _dragonflyMovement.EnterToHoverRStarted -= OnEnterToHoverRStarted;
            _dragonflyMovement.HoverStarted -= OnHoverStarted;
            _dragonflyMovement.PreAttackHoverStarted -= OnPreAttackHoverStarted;
            _dragonflyMovement.ReturnHoverStarted -= OnReturnHoverStarted;
        }

        private void OnDragonflyStarted()
        {
            _damageIndication.Reset();
            _healthIndication.Reset();
            _deathFlash.Reset();
            _preAttackFlash.Reset();
            _swarmCallFX.Reset();

            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("Reset");

        }

        private void ResetBodyAnimationTriggers()
        {
            _animatorBody.ResetTrigger("Reset");
            _animatorBody.ResetTrigger("ToAttack");
            _animatorBody.ResetTrigger("ToEnterToPatrolL");
            _animatorBody.ResetTrigger("ToEnterToPatrolR");
            _animatorBody.ResetTrigger("ToPatrol");
            _animatorBody.ResetTrigger("ToPreAttackHead");
            _animatorBody.ResetTrigger("ToBounce");
            _animatorBody.ResetTrigger("ToPreAttackTail");
            _animatorBody.ResetTrigger("ToEnterToHoverL");
            _animatorBody.ResetTrigger("ToEnterToHoverR");
            _animatorBody.ResetTrigger("ToHover");
            _animatorBody.ResetTrigger("ToPreAttackHover");
            _animatorBody.ResetTrigger("ToHoverReturn");
        }

        private void OnPreAttackStarted()
        {
            _preAttackFlash.PreAttackStart();
        }

        private void OnAttackStarted()
        {
            _preAttackFlash.PreAttackEnd();
            
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToAttack");
        }

        private void OnColliderTransformChanged(Transform transform)
        {
            _damageIndication.SetContactCollisionTransform(transform);
        }

        private void OnDamaged()
        {
            _damageIndication.Play();
        }

        private void OnDied()
        {
            _deathFlash.Play();
        }

        private void OnHealthChanged(int currentHealth, int maxHealth) 
        {
            _healthIndication.Refresh(currentHealth, maxHealth);
        }

        private void OnSwarmCalled() 
        {
            _swarmCallFX.Play();
        }


        // Anim Events

        private void OnEnterToPatrolLStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToEnterToPatrolL");
        }

        private void OnEnterToPatrolRStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToEnterToPatrolR");
        }

        private void OnToPatrolLRStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToPatrol");
        }

        private void OnPreAttackHeadLRStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToPreAttackHead");
        }

        private void OnBounceStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToBounce");
        }

        private void OnPreAttackTailLRStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToPreAttackTail");
        }

        private void OnEnterToHoverLStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToEnterToHoverL");
        }

        private void OnEnterToHoverRStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToEnterToHoverR");
        }

        private void OnHoverStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToHover");
        }

        private void OnPreAttackHoverStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToPreAttackHover");
        }

        private void OnReturnHoverStarted()
        {
            ResetBodyAnimationTriggers();
            _animatorBody.SetTrigger("ToHoverReturn");
        }
    }
}
