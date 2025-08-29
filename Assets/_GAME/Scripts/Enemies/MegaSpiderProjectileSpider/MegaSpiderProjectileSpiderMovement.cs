using System;
using _GAME.Scripts.Enemies.MegaSpiderProjectileSpider.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

// TODO: Add gravity acceleration

namespace _GAME.Scripts.Enemies.MegaSpiderProjectileSpider
{
    public class MegaSpiderProjectileSpiderMovement : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform _lampTransform;
        private float _startDistance;
        private float _startZ;
        private bool _isCollided;
        private bool _isAttackZoneExited = false;
        private bool _isAttackSuccess = false;
    
        private float _localTime;
        private Vector3 prevPos;
        private float _gravityMagnitude;
        private bool _isAttacking;
        
        // States
        private MegaSpiderProjectileSpiderMovementStateFactory _stateFactory; // DI it later
        private readonly StateMachine _stateMachine = new ();
        private MegaSpiderProjectileSpiderIdleState _idleState;
        private MegaSpiderProjectileSpiderPreAttackState _preAttackState;
        private MegaSpiderProjectileSpiderAttackState _attackState;
        private MegaSpiderProjectileSpiderBounceState _bounceState;
        private MegaSpiderProjectileSpiderFallState _fallState;

        public event Action FallEnded;
        
        public void Initialize()
        {
            Debug.Log("Initialize Movement");
            _stateFactory = new();
            _stateFactory.SetEnemyDependencies(
                transform,
                _lampTransform
                );
            
            CreateStates();
            ConfigureStateTransitions();

            _isAttacking = false;
            _isCollided = false;
            enabled = false;
        }

        private void CreateStates()
        {
            _idleState = (MegaSpiderProjectileSpiderIdleState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderIdleState));
            _preAttackState = (MegaSpiderProjectileSpiderPreAttackState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderPreAttackState));
            _attackState =  (MegaSpiderProjectileSpiderAttackState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderAttackState));
            _bounceState =  (MegaSpiderProjectileSpiderBounceState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderBounceState));
            _fallState =  (MegaSpiderProjectileSpiderFallState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderFallState));
        }

        private void ConfigureStateTransitions()
        {
            At(_idleState, _preAttackState, IsAttacked());
            At(_preAttackState, _attackState, () => _preAttackState.IsReadyToSwitch);
            At(_attackState, _bounceState, IsCollided());
            At(_bounceState, _fallState, IsAttackEndedFail());
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
            
            Func<bool> IsAttackEndedFail() => () =>
            {
                if (_isAttackZoneExited && !_isAttackSuccess)
                {
                    _isAttackZoneExited = false;
                    _isAttackSuccess = false;
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

        public void Play()
        {
            enabled = true;
            _stateMachine.SetState(_idleState);
        }

        public void StartAttack()
        {
            _isAttacking = true;            
        }
        
        public void Collide()
        {
            _isCollided = true;
        }
        
        public void OnAttackZoneExit()
        {
            _isAttackZoneExited = true;
            _isAttackSuccess = false;
        }

        private void Update()
        {
            _stateMachine.Tick();
        }
    }
}
