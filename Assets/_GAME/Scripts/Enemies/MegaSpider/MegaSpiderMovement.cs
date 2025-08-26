using System;
using _GAME.Scripts.Enemies.MegaSpider.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider
{
    public class MegaSpiderMovement : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform _visibleBodyTransform;
        [SerializeField] private Transform _animatedTransform;
        [SerializeField] private Transform _calculatedTransform;
        [SerializeField] private Animator _animator;
        [Header("Tangle Attack Settings")]
        [SerializeField] private Transform _lampTransform; // TODO: DI
        [SerializeField] private AnimationCurve _swingCurve;
        [SerializeField] private AnimationCurve _dropCurve;
        [Header("Wire Attack Settings")]
        [SerializeField] private Transform _cameraTransform;
        [Header("Bounce Settings")]
        [SerializeField] private float _bounceSpeed;
        [Header("Climb Settings")]
        [SerializeField] private AnimationCurve _climbCurve;
        
        // States
        private MegaSpiderMovementStateFactory _stateFactory;
        private MegaSpiderEnterLState _enterLState;
        private MegaSpiderEnterRState _enterRState;
        private MegaSpiderZigzagAttackLState _zigzagAttackLState;
        private MegaSpiderZigzagAttackRState _zigzagAttackRState;
        private MegaSpiderProjectileBottomAttackLState _projectileBottomAttackLState;
        private MegaSpiderProjectileBottomAttackRState _projectileBottomAttackRState;
        private MegaSpiderProjectileDoubleUpAttackLState _projectileDoubleUpAttackLState;
        private MegaSpiderProjectileDoubleUpAttackRState _projectileDoubleUpAttackRState;
        private MegaSpiderHangAttackLState _hangAttackLState;
        private MegaSpiderHangAttackRState _hangAttackRState;
        private MegaSpiderHangJumpAttackLState _hangJumpAttackLState;
        private MegaSpiderHangJumpAttackRState _hangJumpAttackRState;
        private MegaSpiderProjectileTopAttackLState _projectileTopAttackLState;
        private MegaSpiderProjectileTopAttackRState _projectileTopAttackRState;
        private MegaSpiderProjectileDoubleDownAttackLState _projectileDoubleDownAttackLState;
        private MegaSpiderProjectileDoubleDownAttackRState _projectileDoubleDownAttackRState;
        private MegaSpiderTangleAttackLState _tangleAttackLState;
        private MegaSpiderTangleAttackRState _tangleAttackRState;
        private MegaSpiderWireAttackState _wireAttackState;
        private MegaSpiderBounceState _bounceState;
        private MegaSpiderFallState _fallState;
        private MegaSpiderSuccessFallState _successFallState;
        private MegaSpiderDropFallState _dropFallState;
        private MegaSpiderSwingLState _swingLState;
        private MegaSpiderSwingRState _swingRState;
        private MegaSpiderClimbState _climbState;
        
        
        public void Initialize()
        {
            _stateFactory = new();
            _stateFactory.SetEnemyDependencies(
                _animator, 
                _visibleBodyTransform, 
                _animatedTransform,
                _calculatedTransform,
                _lampTransform,
                _swingCurve,
                _dropCurve,
                _cameraTransform,
                _bounceSpeed,
                _climbCurve
                );
        
            _enterLState = (MegaSpiderEnterLState)_stateFactory.Create(typeof(MegaSpiderEnterLState));
            _enterRState = (MegaSpiderEnterRState)_stateFactory.Create(typeof(MegaSpiderEnterRState));
            _zigzagAttackLState = (MegaSpiderZigzagAttackLState)_stateFactory.Create(typeof(MegaSpiderZigzagAttackLState));
            _zigzagAttackRState = (MegaSpiderZigzagAttackRState)_stateFactory.Create(typeof(MegaSpiderZigzagAttackRState));
            _projectileBottomAttackLState = (MegaSpiderProjectileBottomAttackLState)_stateFactory.Create(typeof(MegaSpiderProjectileBottomAttackLState));
            _projectileBottomAttackRState = (MegaSpiderProjectileBottomAttackRState)_stateFactory.Create(typeof(MegaSpiderProjectileBottomAttackRState));
            _projectileDoubleUpAttackLState = (MegaSpiderProjectileDoubleUpAttackLState)_stateFactory.Create(typeof(MegaSpiderProjectileDoubleUpAttackLState));
            _projectileDoubleUpAttackRState = (MegaSpiderProjectileDoubleUpAttackRState)_stateFactory.Create(typeof(MegaSpiderProjectileDoubleUpAttackRState));
            _hangAttackLState = (MegaSpiderHangAttackLState)_stateFactory.Create(typeof(MegaSpiderHangAttackLState));
            _hangAttackRState = (MegaSpiderHangAttackRState)_stateFactory.Create(typeof(MegaSpiderHangAttackRState));
            _hangJumpAttackLState = (MegaSpiderHangJumpAttackLState)_stateFactory.Create(typeof(MegaSpiderHangJumpAttackLState));
            _hangJumpAttackRState = (MegaSpiderHangJumpAttackRState)_stateFactory.Create(typeof(MegaSpiderHangJumpAttackRState));
            _projectileTopAttackLState = (MegaSpiderProjectileTopAttackLState)_stateFactory.Create(typeof(MegaSpiderProjectileTopAttackLState));
            _projectileTopAttackRState = (MegaSpiderProjectileTopAttackRState)_stateFactory.Create(typeof(MegaSpiderProjectileTopAttackRState));
            _projectileDoubleDownAttackLState = (MegaSpiderProjectileDoubleDownAttackLState)_stateFactory.Create(typeof(MegaSpiderProjectileDoubleDownAttackLState));
            _projectileDoubleDownAttackRState = (MegaSpiderProjectileDoubleDownAttackRState)_stateFactory.Create(typeof(MegaSpiderProjectileDoubleDownAttackRState));
            _tangleAttackLState = (MegaSpiderTangleAttackLState)_stateFactory.Create(typeof(MegaSpiderTangleAttackLState));
            _tangleAttackRState = (MegaSpiderTangleAttackRState)_stateFactory.Create(typeof(MegaSpiderTangleAttackRState));
            _wireAttackState = (MegaSpiderWireAttackState)_stateFactory.Create(typeof(MegaSpiderWireAttackState));
            _bounceState = (MegaSpiderBounceState)_stateFactory.Create(typeof(MegaSpiderBounceState));
            _fallState = (MegaSpiderFallState)_stateFactory.Create(typeof(MegaSpiderFallState));
            _successFallState = (MegaSpiderSuccessFallState)_stateFactory.Create(typeof(MegaSpiderSuccessFallState));
            _dropFallState = (MegaSpiderDropFallState)_stateFactory.Create(typeof(MegaSpiderDropFallState));
            _swingLState = (MegaSpiderSwingLState)_stateFactory.Create(typeof(MegaSpiderSwingLState));
            _swingRState = (MegaSpiderSwingRState)_stateFactory.Create(typeof(MegaSpiderSwingRState));
            _climbState = (MegaSpiderClimbState)_stateFactory.Create(typeof(MegaSpiderClimbState));
            
            enabled = false;
        }

        public void Play()
        {
            enabled = true;
            _climbState.Enter();
        }

        private void Update()
        {
            _climbState.Tick();
        }
    }
}
