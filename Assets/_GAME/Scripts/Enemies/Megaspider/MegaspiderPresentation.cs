using System;
using System.Collections;
using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib.Interfaces;
using TMPro;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderPresentation : MonoBehaviour, IInitializable
    {
        private static readonly int Speed = Animator.StringToHash("speed");
        private static readonly int ToWalk = Animator.StringToHash("ToWalk");
        private static readonly int ToClimbDown = Animator.StringToHash("ToClimbDown");
        private static readonly int ToAttack = Animator.StringToHash("ToAttack");
        private static readonly int ToJump = Animator.StringToHash("ToJump");
        private static readonly int ToDive = Animator.StringToHash("ToDive");
        private static readonly int ToBounce = Animator.StringToHash("ToBounce");
        private static readonly int ToSwingShoot = Animator.StringToHash("ToSwingShoot");
        private static readonly int ToSwing = Animator.StringToHash("ToSwing");
        private static readonly int ToShoot = Animator.StringToHash("ToShoot");
        private static readonly int ToClimbShoot = Animator.StringToHash("ToClimbShoot");
        private static readonly int ToClimbUp = Animator.StringToHash("ToClimbUp");
        private static readonly int ToFail = Animator.StringToHash("ToFail");
        [SerializeField] private Megaspider _megaspider;
        [SerializeField] private MegaspiderMovement _movement;
        [SerializeField] private MegaspiderAnimationClipEventListener _animationClipEvents;
        [SerializeField] private MegaspiderSpiderwebController _spiderwebController;
        [SerializeField] private DamageFlashSingleMaterial _damageFlash;
        [SerializeField] private PreAttackFlashSingleMaterial _preAttackFlash;
        [SerializeField] private float _preattackDuration = 0.15f;
        [SerializeField] private HealthIndicationSingleMaterial _healthIndication;
        [SerializeField] private DamageEmitParticles _damageEmitParticles;
        [SerializeField] private DeathFlashSingleMaterial _deathFlash;
        [Header("Animation")]
        [SerializeField] private Animator _animator;
        
        private WaitForSeconds _preattackDelay;
        private float _localTime;
        [SerializeField] private bool _isRegularShootStarted;
        private WaitForSeconds _fallShootDelay = new WaitForSeconds(0.25f);

        public void Initialize()
        {
            _preattackDelay = new (_preattackDuration);
            
            _spiderwebController.Initialize();
            _damageFlash.Initialize();
            _preAttackFlash.Initialize();
            _healthIndication.Initialize();
            _deathFlash.Initialize();

            _isRegularShootStarted = false;
            _megaspider.Started += OnMegaspiderStarted;
            _megaspider.Damaged += _damageFlash.Play;
            _megaspider.HealthChanged += _healthIndication.Refresh;
            _megaspider.HealthChanged += _damageEmitParticles.HandleHealthChanged;
            _megaspider.Dead += _deathFlash.Play;
            _megaspider.Dead += _damageEmitParticles.HandleDead;

            _movement.EnterStateStarted += OnEnterStateStarted;
            _movement.HangStartRequested += OnHangStartRequested;
            _movement.ZigzagAttackStateStarted += OnZigzagAttackStateStarted;
            _animationClipEvents.ZigZag3Called += OnZigZag3Called; 
            _animationClipEvents.AttackCalled += OnAttackCalled;
            _movement.ProjectileBottomAttackStateStarted += OnProjectileBottomAttackStateStarted;
            _movement.ProjectileDoubleUpAttackStateStarted += OnProjectileDoubleUpAttackStateStarted;
            _animationClipEvents.ProjectileDoubleUp2Called += OnProjectileDoubleUp2Called;
            _movement.ProjectileTopAttackStateStarted += OnProjectileTopAttackStateStarted;
            _movement.ProjectileDoubleDownAttackStateStarted += OnProjectileDoubleDownAttackStateStarted;
            _animationClipEvents.ProjectileDoubleDown2Called += OnProjectileDoubleDown2Called;
            _movement.WireAttackStateStarted += OnWireAttackStateStarted;
            _movement.WireCollisionProximityEntered += OnWireCollisionProximityEntered;
            _movement.HangJumpAttackStateStarted += OnHangJumpAttackStateStarted;
            _animationClipEvents.HangJumpCalled += OnHangJumpCalled;
            _animationClipEvents.HangJumpDiveCalled += OnHangJumpDiveCalled;
            _movement.HangAttackStateStarted += OnHangAttackStateStarted;
            _movement.TangleAttackStarted += OnTangleAttackStarted;
            _movement.BounceStateStarted += OnBounceStateStarted;
            
            _movement.SuccessFallOutForceCancelled += OnSuccessFallOutForceCancelled;
            _movement.FailFallOutForceCancelled += OnFailFallOutForceCancelled;
            
            _movement.SwingStateStarted += OnSwingStateStarted;
            _movement.ClimbStateStarted += OnClimbStateStarted;
            _movement.ClimbStateEnded += OnClimbStateEnded;
            _movement.SwingStateEnded += OnSwingStateEnded;
            _movement.FailFallStarted += OnFailFallStarted;

            _movement.PreAttackStarted += StartPreattack;
            _movement.DeathStateEnded += _damageEmitParticles.HandleDeathEnd;
        }

        private void OnDestroy()
        {
            _megaspider.Started -= OnMegaspiderStarted;
            _megaspider.Damaged -= _damageFlash.Play;
            _megaspider.HealthChanged -= _healthIndication.Refresh;
            _megaspider.HealthChanged -= _damageEmitParticles.HandleHealthChanged;
            _megaspider.Dead -= _deathFlash.Play;
            _megaspider.Dead -= _damageEmitParticles.HandleDead;
            
            _movement.EnterStateStarted -= OnEnterStateStarted;
            _movement.HangStartRequested -= OnHangStartRequested;
            _movement.ZigzagAttackStateStarted -= OnZigzagAttackStateStarted;
            _animationClipEvents.ZigZag3Called -= OnZigZag3Called;
            _animationClipEvents.AttackCalled -= OnAttackCalled;
            _movement.ProjectileBottomAttackStateStarted -= OnProjectileBottomAttackStateStarted;
            _movement.ProjectileDoubleUpAttackStateStarted -= OnProjectileDoubleUpAttackStateStarted;
            _animationClipEvents.ProjectileDoubleUp2Called -= OnProjectileDoubleUp2Called;
            _movement.ProjectileTopAttackStateStarted -= OnProjectileTopAttackStateStarted;
            _movement.ProjectileDoubleDownAttackStateStarted -= OnProjectileDoubleDownAttackStateStarted;
            _animationClipEvents.ProjectileDoubleDown2Called -= OnProjectileDoubleDown2Called;
            _movement.WireAttackStateStarted -= OnWireAttackStateStarted;
            _movement.WireCollisionProximityEntered -= OnWireCollisionProximityEntered;
            _movement.HangJumpAttackStateStarted -= OnHangJumpAttackStateStarted;
            _animationClipEvents.HangJumpCalled -= OnHangJumpCalled;
            _animationClipEvents.HangJumpDiveCalled -= OnHangJumpDiveCalled;
            _movement.HangAttackStateStarted -= OnHangAttackStateStarted;
            _movement.TangleAttackStarted -= OnTangleAttackStarted;
            _movement.BounceStateStarted -= OnBounceStateStarted;
            
            _movement.SuccessFallOutForceCancelled -= OnSuccessFallOutForceCancelled;
            _movement.FailFallOutForceCancelled -= OnFailFallOutForceCancelled;
            
            _movement.SwingStateStarted -= OnSwingStateStarted;
            _movement.ClimbStateStarted -= OnClimbStateStarted;
            _movement.ClimbStateEnded -= OnClimbStateEnded;
            _movement.SwingStateEnded -= OnSwingStateEnded;
            _movement.FailFallStarted -= OnFailFallStarted;

            
            _movement.PreAttackStarted -= StartPreattack;
            _movement.DeathStateEnded -= _damageEmitParticles.HandleDeathEnd;
        }

        private void ResetBodyAnimationTriggers()
        {
            _animator.ResetTrigger("ToWalk");
            _animator.ResetTrigger("ToClimbDown");
            _animator.ResetTrigger("ToAttack");
            _animator.ResetTrigger("ToJump");
            _animator.ResetTrigger("ToDive");
            _animator.ResetTrigger("ToBounce");
            _animator.ResetTrigger("ToSwingShoot");
            _animator.ResetTrigger("ToSwing");
            _animator.ResetTrigger("ToShoot");
            _animator.ResetTrigger("ToClimbShoot");
            _animator.ResetTrigger("ToClimbUp");
            _animator.ResetTrigger("ToFail");
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

        private void OnMegaspiderStarted()
        {
            _healthIndication.Reset();
            _damageEmitParticles.Initialize();
            _deathFlash.Initialize();
        }

        private void OnEnterStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetFloat(Speed, 2);
            _animator.SetTrigger(ToWalk);
        }

        private void OnHangStartRequested(Type arg1, IPositionProvider arg2)
        {
            ResetBodyAnimationTriggers();
            _animator.SetFloat(Speed, 1);
            _animator.SetTrigger(ToClimbDown);
        }

        private void OnZigzagAttackStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetFloat(Speed, 2);
            _animator.SetTrigger(ToWalk);
        }

        private void OnZigZag3Called()
        {
            _animator.SetFloat(Speed, 1.56f);
        }

        private void OnAttackCalled()
        {
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToAttack);       
        }

        private void OnProjectileBottomAttackStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetFloat(Speed, 0.74f);
            _animator.SetTrigger(ToWalk);
        }

        private void OnProjectileDoubleUpAttackStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetFloat(Speed, 0.74f);
            _animator.SetTrigger(ToWalk);
        }

        private void OnProjectileDoubleUp2Called()
        {
            _animator.SetFloat(Speed, 0.776f);
        }

        private void OnProjectileTopAttackStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetFloat(Speed, 0.9f);
            _animator.SetTrigger(ToWalk);       
        }

        private void OnProjectileDoubleDownAttackStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetFloat(Speed, 0.95f);
            _animator.SetTrigger(ToWalk);
        }

        private void OnProjectileDoubleDown2Called()
        {
            _animator.SetFloat(Speed, 0.72f);
        }

        private void OnWireAttackStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetFloat(Speed, 1.05f);
            _animator.SetTrigger(ToWalk);
            _localTime = 0;
            StartCoroutine(SpeedUpWireAttack());
        }

        private IEnumerator SpeedUpWireAttack()
        {
            while (_localTime < 1.2f) // TODO: make it configurable
            {
                _localTime += Time.deltaTime;
                yield return null;
                float phase = _localTime / 1.2f;
                _animator.SetFloat(Speed, Mathf.Lerp(0.95f, 1.8f, phase));
            }
        }

        private void OnHangJumpAttackStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetFloat(Speed, 1.4f);
            _animator.SetTrigger(ToWalk);
        }

        private void OnHangJumpCalled()
        {
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToJump);
        }

        private void OnHangJumpDiveCalled()
        {
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToDive);
        }

        private void OnHangAttackStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToDive);
        }

        private void OnTangleAttackStarted(ITangledWireProvider obj)
        {
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToDive);
        }

        private void OnBounceStateStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToBounce);       
        }

        private void OnSwingStateStarted()
        {
            ResetBodyAnimationTriggers();
            if (_isRegularShootStarted)
                _animator.SetTrigger(ToSwing);
            else
                _animator.SetTrigger(ToSwingShoot);
        }

        private void OnSuccessFallOutForceCancelled(IPositionProvider obj)
        {
            _isRegularShootStarted = true;
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToShoot);
        }

        private void OnFailFallOutForceCancelled(IPositionProvider obj)
        {
            _isRegularShootStarted = true;
            StartCoroutine(DelayFallSpiderwebShoot());
        }

        private IEnumerator DelayFallSpiderwebShoot()
        {
            yield return _fallShootDelay;
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToShoot);  
        }

        private void OnClimbStateStarted()
        {
            ResetBodyAnimationTriggers();
            if (_isRegularShootStarted)
                _animator.SetTrigger(ToClimbUp);
            else
                _animator.SetTrigger(ToClimbShoot);
        }

        private void OnClimbStateEnded()
        {
            _isRegularShootStarted = false;
        }

        private void OnSwingStateEnded()
        {
            _isRegularShootStarted = false;
        }

        private void OnFailFallStarted()
        {
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToFail);
        }

        private void OnWireCollisionProximityEntered()
        {
            ResetBodyAnimationTriggers();
            _animator.SetTrigger(ToAttack);
        }
    }
}
