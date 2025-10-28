using System;
using _GAME.Scripts.Enemies.MegaspiderProjectileSpider.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using _GAME.Scripts.Enemies.Dragonfly;
using _GAME.Scripts.Enemies.MegaSpiderProjectileSpider.MovementStates;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaspiderProjectileSpider
{
    public class MegaspiderProjectileSpiderMovement : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform _rootTransform;
        private float _startDistance;
        private float _startZ;
        private bool _isCollided;

        private AttackResult _attackResult;
    
        private float _localTime;
        private Vector3 _prevPos;
        private float _gravityMagnitude;
        private bool _isAttacking;
        private bool _isLampDestroyed;
        
        // States
        private MegaspiderProjectileSpiderMovementStateFactory _stateFactory;
        private readonly StateMachine _stateMachine = new ();
        private MegaspiderProjectileSpiderIdleState _idleState;
        private MegaspiderProjectileSpiderPreAttackState _preAttackState;
        private MegaspiderProjectileSpiderAttackState _attackState;
        private MegaspiderProjectileSpiderBounceState _bounceState;
        private MegaspiderProjectileSpiderFallState _fallState;
        private MegaspiderProjectileSpiderFallLampDestroyedState _fallLampDestroyedState;
        
        // Dependencies
        private Transform _lampTransform;
        
        public void Construct(Transform lampTransform, MegaspiderProjectileSpiderMovementStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
            _lampTransform = lampTransform;
        }

        public event Action FallEnded;
        
        public void Initialize()
        {
            _stateFactory.SetEnemyDependencies(
                transform,
                _lampTransform,
                _rootTransform
                );
            
            CreateStates();
            ConfigureStateTransitions();

            _isAttacking = false;
            _isCollided = false;
            enabled = false;
            _attackResult = AttackResult.None;
        }

        public void Play()
        {
            _isAttacking = false;
            _isCollided = false;
            _isLampDestroyed = false;
            enabled = true;
            _stateMachine.SetState(_idleState);
            _attackResult = AttackResult.None;
        }

        public void TriggerFall(AttackResult attackResult)
        {
            _attackResult = attackResult;
        }

        public void TriggerCollide()
        {
            _isCollided = true;
        }
        
        public void SetLampDestroyed()
        {
            _isLampDestroyed = true;
        }

        private void CreateStates()
        {
            _idleState = (MegaspiderProjectileSpiderIdleState)_stateFactory.Create(typeof(MegaspiderProjectileSpiderIdleState));
            _preAttackState = (MegaspiderProjectileSpiderPreAttackState)_stateFactory.Create(typeof(MegaspiderProjectileSpiderPreAttackState));
            _attackState = (MegaspiderProjectileSpiderAttackState)_stateFactory.Create(typeof(MegaspiderProjectileSpiderAttackState));
            _bounceState = (MegaspiderProjectileSpiderBounceState)_stateFactory.Create(typeof(MegaspiderProjectileSpiderBounceState));
            _fallState = (MegaspiderProjectileSpiderFallState)_stateFactory.Create(typeof(MegaspiderProjectileSpiderFallState));
            _fallLampDestroyedState = (MegaspiderProjectileSpiderFallLampDestroyedState)_stateFactory
                .Create(typeof(MegaspiderProjectileSpiderFallLampDestroyedState));
        }

        private void ConfigureStateTransitions()
        {
            At(_idleState, _preAttackState, IsAttacked());
            At(_preAttackState, _attackState, () => _preAttackState.IsReadyToSwitch);
            At(_attackState, _fallState, IsAttackFail());
            
            At(_attackState, _bounceState, IsCollided());
            At(_bounceState, _fallState, IsAttackFail());
            At(_bounceState, _fallState, IsAttackSuccess());
            At(_fallState, _idleState, () => _fallState.IsReadyToSwitch);
            
            // Transition helper methods
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            
            // Predicates 
            Func<bool> IsAttacked() => () =>
            {
                if (_isAttacking)
                {
                    _isAttacking = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsCollided() => () =>
            {
                if (_isCollided)
                {
                    _isCollided = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAttackFail() => () =>
            {
                if (_attackResult == AttackResult.Fail)
                {
                    _attackResult = AttackResult.None;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAttackSuccess() => () =>
            {
                if (_attackResult == AttackResult.Success)
                {
                    _attackResult = AttackResult.None;
                    return true;
                }
                return false;
            };
            

            _fallState.Ended += OnFallStateEnded;
            _bounceState.Ended += OnBounceStateEnded;// TODO: remove later
        }

        private void OnDestroy()
        {
            _fallState.Ended -= OnFallStateEnded;
            _bounceState.Ended -= OnBounceStateEnded;// TODO: remove later
        }

        private void OnFallStateEnded()
        {
            enabled = false;
            FallEnded?.Invoke();
        }

        private void OnBounceStateEnded()
        {
            _isCollided = false; // TODO: remove later
        }

        public void TriggerAttack()
        {
            _isAttacking = true;            
        }

        private void Update()
        {
            _stateMachine.Tick();
        }
    }
}
