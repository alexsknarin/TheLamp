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
        [SerializeField] private float _speed;
        [SerializeField] private float _transitionDuration;
        [SerializeField] private float _gravity;

        [SerializeField] private Transform _lampTransform;
        private Vector3 _lampPosition = Vector3.zero;
        private float _startDistance;
        private float _startZ;
        private bool _isCollided;
        private bool _isAttackZoneExited = false;
        private bool _isAttackSuccess = false;
    
        private float _localTime;
        private Vector3 prevPos;
        private float _gravityMagnitude;
        
        
        // States
        private MegaSpiderProjectileSpiderMovementStateFactory _stateFactory; // DI it later
        private readonly StateMachine _stateMachine = new ();
        private MegaSpiderProjectileSpiderIdleState _idleState;
        private MegaSpiderProjectileSpiderPreAttackState _preAttackState;
        private MegaSpiderProjectileSpiderAttackState _attackState;
        private MegaSpiderProjectileSpiderBounceState _bounceState;
        private MegaSpiderProjectileSpiderFallState _fallState;

        
        private bool _isAttacking;
        
        public void Initialize()
        {
            Debug.Log("Initialize Movement");
            _stateFactory = new();
            _stateFactory.SetEnemyDependencies(
                transform,
                _lampTransform
                );
            _idleState = (MegaSpiderProjectileSpiderIdleState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderIdleState));
            _preAttackState = (MegaSpiderProjectileSpiderPreAttackState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderPreAttackState));
            _attackState =  (MegaSpiderProjectileSpiderAttackState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderAttackState));
            _bounceState =  (MegaSpiderProjectileSpiderBounceState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderBounceState));
            _fallState =  (MegaSpiderProjectileSpiderFallState)_stateFactory.Create(typeof(MegaSpiderProjectileSpiderFallState));
            
            
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
            
            
            
            
            
            _isAttacking = false;
            enabled = false;
        }

        public void Play()
        {
            Debug.Log("movement play");
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


        void Start2()
        {
            _localTime = 0;
            _gravityMagnitude = 0;
            _startDistance = Vector3.Distance(transform.position, _lampPosition) - 0.5f;
            _startZ = transform.position.z;
        
            // Shift Attack Aim Center
        
            float side = Mathf.Sign(transform.position.x);
            float sideFraction = Mathf.Abs(transform.position.x) / 1.4f; // TODO: take camera into consideration - should be in the screen space
        
            float lampShift = Mathf.Lerp(0.0f, 0.36f, sideFraction) * side;
        
            _lampPosition.x += lampShift;
        
            Debug.DrawLine(Vector3.zero, _lampPosition, Color.red, 55f);
       
        }

        void Update2()
        {
            prevPos = transform.position;
            Vector3 direction = (_lampPosition - transform.position).normalized;
            float phase = _localTime / _transitionDuration;
        
            if (phase > 1)
                phase = 1;
        
            phase = Mathf.Pow(phase, .65f);
            direction = Vector3.Lerp(Vector3.up, direction, phase);
        
            Vector3 newPosition = transform.position;
            newPosition += direction * (_speed * Time.deltaTime) + Vector3.down * (_gravityMagnitude * Time.deltaTime);
            _gravityMagnitude += _gravity * Time.deltaTime;
        
            // Use Distance to collision and Lerp
            float distance = Vector3.Distance(newPosition, _lampPosition) - 0.5f;
            float zPhase = Mathf.Clamp01(1 - distance / _startDistance);
            newPosition.z = Mathf.Lerp(transform.position.z, 0f, zPhase);
        
            transform.position = newPosition;
        
            _localTime += Time.deltaTime;
        
            Debug.DrawLine(prevPos, transform.position, Color.cyan, 5f);
        }
    }
}
