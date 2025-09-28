using System;
using _GAME.Scripts.Enemies.Dragonfly; // TODO: move to library
using _GAME.Scripts.Enemies.Megaspider.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderMovement : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform _visibleBodyTransform;
        [SerializeField] private Transform _animatedTransform;
        [SerializeField] private Transform _calculatedTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private MegaspiderAnimationClipEventListener _animationClipEvents;
        [Header( "Movement States")]
        [SerializeField] private string _stateDebug;
        [Header("Tangle Attack Settings")]
        [SerializeField] private AnimationCurve _swingCurve;
        [SerializeField] private AnimationCurve _dropCurve;
        [Header("Bounce Settings")]
        [SerializeField] private float _bounceSpeed;
        [Header("Climb Settings")]
        [SerializeField] private AnimationCurve _climbCurve;
        [Header("Swing Settings")] 
        
        
        // States
        private readonly StateMachine _stateMachine = new();
        private EnemyMovementStateBase _currentState;
        
        private MegaspiderIdleState _idleState;
        private MegaspiderEnterLState _enterLState;
        private MegaspiderEnterRState _enterRState;
        private MegaspiderZigzagAttackLState _zigzagAttackLState;
        private MegaspiderZigzagAttackRState _zigzagAttackRState;
        private MegaspiderProjectileBottomAttackLState _projectileBottomAttackLState;
        private MegaspiderProjectileBottomAttackRState _projectileBottomAttackRState;
        private MegaspiderProjectileDoubleUpAttackLState _projectileDoubleUpAttackLState;
        private MegaspiderProjectileDoubleUpAttackRState _projectileDoubleUpAttackRState;
        private MegaspiderHangAttackLState _hangAttackLState;
        private MegaspiderHangAttackRState _hangAttackRState;
        private MegaspiderHangJumpAttackLState _hangJumpAttackLState;
        private MegaspiderHangJumpAttackRState _hangJumpAttackRState;
        private MegaspiderProjectileTopAttackLState _projectileTopAttackLState;
        private MegaspiderProjectileTopAttackRState _projectileTopAttackRState;
        private MegaspiderProjectileDoubleDownAttackLState _projectileDoubleDownAttackLState;
        private MegaspiderProjectileDoubleDownAttackRState _projectileDoubleDownAttackRState;
        private MegaspiderTangleAttackLState _tangleAttackLState;
        private MegaspiderTangleAttackRState _tangleAttackRState;
        private MegaspiderWireAttackState _wireAttackState;
        private MegaspiderBounceState _bounceState;
        private MegaspiderFallState _fallState;
        private MegaspiderSuccessFallState _successFallState;
        private MegaspiderDropFallState _dropFallState;
        private MegaspiderSwingLState _swingLState;
        private MegaspiderSwingRState _swingRState;
        private MegaspiderClimbState _climbState;
        private MegaspiderDeathFallState _deathFallState;

        private bool _isAnimClipEnded = false;
        private bool _isCollided = false;
        [SerializeField] private AttackResult _attackResult;
        private float _collisionRadius;
        private float _fullCollisionDistance;
        private bool _isAttackStateBeforeCollision = false;

        // Dependencies
        private Transform _cameraTransform; // TODO: remove???
        private Transform _lampTransform;
        private MegaspiderMovementStateFactory _stateFactory;
        private float _collisionThreshold;
        private float _swingZoneSize;

        public void Construct(
            Transform cameraTransform, 
            Transform lampTransform, 
            MegaspiderMovementStateFactory stateFactory,
            IGameConfigService gameConfigService
            )
        {
            _cameraTransform = cameraTransform;
            _lampTransform = lampTransform;
            _stateFactory = stateFactory;
            _collisionThreshold = gameConfigService.PlayerConfig.CollisionThreshold;
            _swingZoneSize = gameConfigService.GameConfig.MegaspiderSwingZoneSize;
        }
        
        public event Action AnimatedAttackStarted;
        public event Action DeathStateEnded;
        public event Action<Type> StaticBridge1Called;
        public event Action StaticBridge1Broken;
        public event Action<Type> StaticBridge2Called;
        public event Action StaticBridge2Broken;
        public event Action<Type> StaticBridge3Called;
        public event Action StaticBridge3Broken;
        public event Action<Type, IPositionProvider> HangStartRequested;
        public event Action HangStopRequested;
        public event Action HangBreakRequested;
        public event Action<ITangledWireProvider> TangleAttackStarted;
        public event Action TangleAttackEnded;
        public event Action<IPositionProvider> SuccessFallOutForceCancelled;
        public event Action ClimbStateEnded;
        public event Action<IPositionProvider> FailFallOutForceCancelled;
        public event Action SwingStateEnded;
        public event Action<IPositionProvider> FallStateEnded;
        public event Action<IAttackWiresProvider> WireAttackStateEntered;

        public void SetCollisionRadius(float radius)
        {
            _collisionRadius = radius;
        }
        
        public void Initialize()
        {
            enabled = false;
            _fullCollisionDistance = _collisionRadius + 0.49f;
            _isAttackStateBeforeCollision = false;
            _stateFactory.SetEnemyDependencies(
                _animator, 
                _visibleBodyTransform, 
                _animatedTransform,
                _calculatedTransform,
                transform,
                _swingCurve,
                _dropCurve,
                _bounceSpeed,
                _climbCurve,
                _collisionRadius
                );

            CreateMovementStates();
            CreateStateTransitions();

            _animationClipEvents.AnimClipEnded += OnAnimClipEnded;
            _animationClipEvents.StaticBridge1Called += OnStaticBridge1Called;
            _animationClipEvents.StaticBridge1Broken += OnStaticBridge1Broken;
            _animationClipEvents.StaticBridge2Called += OnStaticBridge2Called;
            _animationClipEvents.StaticBridge2Broken += OnStaticBridge2Broken;
            _animationClipEvents.StaticBridge3Called += OnStaticBridge3Called;
            _animationClipEvents.StaticBridge3Broken += OnStaticBridge3Broken;
            _animationClipEvents.HangStartRequested += OnHangStartRequested;
            _animationClipEvents.HangStopRequested += OnHangStopRequested;
            _animationClipEvents.HangBreakRequested += OnHangBreakRequested;
            
            _zigzagAttackLState.Started += OnAnimatedAttackStarted;
            _zigzagAttackRState.Started += OnAnimatedAttackStarted;
            _hangAttackLState.Started += OnAnimatedAttackStarted;
            _hangAttackRState.Started += OnAnimatedAttackStarted;
            _hangJumpAttackLState.Started += OnAnimatedAttackStarted;
            _hangJumpAttackRState.Started += OnAnimatedAttackStarted;
            _tangleAttackLState.Started += OnAnimatedAttackStarted;
            _tangleAttackRState.Started += OnAnimatedAttackStarted;
            _wireAttackState.Started += OnAnimatedAttackStarted;

            _tangleAttackLState.Started += OnTangleAttackLStarted;
            _tangleAttackRState.Started += OnTangleAttackRStarted;
            _tangleAttackLState.Ended += OnTangleAttackEnded;
            _tangleAttackRState.Ended += OnTangleAttackEnded;
            
            _wireAttackState.Entered += OnWireAttackStateEntered;

            _swingLState.Ended += OnSwingEnded;
            _swingRState.Ended += OnSwingEnded;
            
            _successFallState.OutForceCancelled += OnSuccessFallOutForceCancelled;
            _fallState.OutForceCancelled += OnFailFallOutForceCancelled;
            _successFallState.Ended += OnSuccessFallStateEnded;
            _fallState.Ended += OnFailFallStateEnded;
            
            _climbState.Ended += OnClimbStateEnded;

            _bounceState.Ended += OnBounceStateEnded;
            _deathFallState.Ended += OnDeathStateEnded;


        }

        private void OnDestroy()
        {
            _animationClipEvents.AnimClipEnded -= OnAnimClipEnded;
            _animationClipEvents.StaticBridge1Called -= OnStaticBridge1Called;
            _animationClipEvents.StaticBridge1Broken -= OnStaticBridge1Broken;
            _animationClipEvents.StaticBridge2Called -= OnStaticBridge2Called;
            _animationClipEvents.StaticBridge2Broken -= OnStaticBridge2Broken;
            _animationClipEvents.StaticBridge3Called -= OnStaticBridge3Called;
            _animationClipEvents.StaticBridge3Broken -= OnStaticBridge3Broken;
            _animationClipEvents.HangStartRequested -= OnHangStartRequested;
            _animationClipEvents.HangStopRequested -= OnHangStopRequested;
            _animationClipEvents.HangBreakRequested -= OnHangBreakRequested;
            
            _zigzagAttackLState.Started -= OnAnimatedAttackStarted;
            _zigzagAttackRState.Started -= OnAnimatedAttackStarted;
            _hangAttackLState.Started -= OnAnimatedAttackStarted;
            _hangAttackRState.Started -= OnAnimatedAttackStarted;
            _hangJumpAttackLState.Started -= OnAnimatedAttackStarted;
            _hangJumpAttackRState.Started -= OnAnimatedAttackStarted;
            _tangleAttackLState.Started -= OnAnimatedAttackStarted;
            _tangleAttackRState.Started -= OnAnimatedAttackStarted;
            _wireAttackState.Started -= OnAnimatedAttackStarted;
            
            _tangleAttackLState.Started -= OnTangleAttackLStarted;
            _tangleAttackRState.Started -= OnTangleAttackRStarted;
            _tangleAttackLState.Ended -= OnTangleAttackEnded;
            _tangleAttackRState.Ended -= OnTangleAttackEnded;

            _wireAttackState.Entered -= OnWireAttackStateEntered;
            
            _swingLState.Ended -= OnSwingEnded;
            _swingRState.Ended -= OnSwingEnded;

            _successFallState.OutForceCancelled -= OnSuccessFallOutForceCancelled;
            _fallState.OutForceCancelled -= OnFailFallOutForceCancelled;
            _successFallState.Ended -= OnSuccessFallStateEnded;
            _fallState.Ended -= OnFailFallStateEnded;

            _climbState.Ended -= OnClimbStateEnded;
            
            _bounceState.Ended -= OnBounceStateEnded;
            _deathFallState.Ended -= OnDeathStateEnded;
        }

        private void OnAnimatedAttackStarted()
        {
            _isAttackStateBeforeCollision = true;
            _isCollided = false;
            AnimatedAttackStarted?.Invoke();
        }

        private void CreateMovementStates()
        {
            _idleState = (MegaspiderIdleState)_stateFactory.Create(typeof(MegaspiderIdleState));
            _enterLState = (MegaspiderEnterLState)_stateFactory.Create(typeof(MegaspiderEnterLState));
            _enterRState = (MegaspiderEnterRState)_stateFactory.Create(typeof(MegaspiderEnterRState));
            _zigzagAttackLState = (MegaspiderZigzagAttackLState)_stateFactory.Create(typeof(MegaspiderZigzagAttackLState));
            _zigzagAttackRState = (MegaspiderZigzagAttackRState)_stateFactory.Create(typeof(MegaspiderZigzagAttackRState));
            _projectileBottomAttackLState = (MegaspiderProjectileBottomAttackLState)_stateFactory.Create(typeof(MegaspiderProjectileBottomAttackLState));
            _projectileBottomAttackRState = (MegaspiderProjectileBottomAttackRState)_stateFactory.Create(typeof(MegaspiderProjectileBottomAttackRState));
            _projectileDoubleUpAttackLState = (MegaspiderProjectileDoubleUpAttackLState)_stateFactory.Create(typeof(MegaspiderProjectileDoubleUpAttackLState));
            _projectileDoubleUpAttackRState = (MegaspiderProjectileDoubleUpAttackRState)_stateFactory.Create(typeof(MegaspiderProjectileDoubleUpAttackRState));
            _hangAttackLState = (MegaspiderHangAttackLState)_stateFactory.Create(typeof(MegaspiderHangAttackLState));
            _hangAttackRState = (MegaspiderHangAttackRState)_stateFactory.Create(typeof(MegaspiderHangAttackRState));
            _hangJumpAttackLState = (MegaspiderHangJumpAttackLState)_stateFactory.Create(typeof(MegaspiderHangJumpAttackLState));
            _hangJumpAttackRState = (MegaspiderHangJumpAttackRState)_stateFactory.Create(typeof(MegaspiderHangJumpAttackRState));
            _projectileTopAttackLState = (MegaspiderProjectileTopAttackLState)_stateFactory.Create(typeof(MegaspiderProjectileTopAttackLState));
            _projectileTopAttackRState = (MegaspiderProjectileTopAttackRState)_stateFactory.Create(typeof(MegaspiderProjectileTopAttackRState));
            _projectileDoubleDownAttackLState = (MegaspiderProjectileDoubleDownAttackLState)_stateFactory.Create(typeof(MegaspiderProjectileDoubleDownAttackLState));
            _projectileDoubleDownAttackRState = (MegaspiderProjectileDoubleDownAttackRState)_stateFactory.Create(typeof(MegaspiderProjectileDoubleDownAttackRState));
            _tangleAttackLState = (MegaspiderTangleAttackLState)_stateFactory.Create(typeof(MegaspiderTangleAttackLState));
            _tangleAttackRState = (MegaspiderTangleAttackRState)_stateFactory.Create(typeof(MegaspiderTangleAttackRState));
            _wireAttackState = (MegaspiderWireAttackState)_stateFactory.Create(typeof(MegaspiderWireAttackState));
            _bounceState = (MegaspiderBounceState)_stateFactory.Create(typeof(MegaspiderBounceState));
            _fallState = (MegaspiderFallState)_stateFactory.Create(typeof(MegaspiderFallState));
            _successFallState = (MegaspiderSuccessFallState)_stateFactory.Create(typeof(MegaspiderSuccessFallState));
            _dropFallState = (MegaspiderDropFallState)_stateFactory.Create(typeof(MegaspiderDropFallState));
            _swingLState = (MegaspiderSwingLState)_stateFactory.Create(typeof(MegaspiderSwingLState));
            _swingRState = (MegaspiderSwingRState)_stateFactory.Create(typeof(MegaspiderSwingRState));
            _climbState = (MegaspiderClimbState)_stateFactory.Create(typeof(MegaspiderClimbState));
            _deathFallState = (MegaspiderDeathFallState)_stateFactory.Create(typeof(MegaspiderDeathFallState));
        }

        private void CreateStateTransitions()
        {
            // Initialize StateMachine 
            // Enter to Attacks
            // At(_enterLState, _wireAttackState, IsAnimationEndedRandom0Of4());
            // At(_enterLState, _zigzagAttackLState, IsAnimationEndedRandom1Of4());
            // At(_enterLState, _projectileBottomAttackLState, IsAnimationEndedRandom2Of4());
            // At(_enterLState, _projectileDoubleUpAttackLState, IsAnimationEndedRandom3Of4());
            //
            // At(_enterRState, _wireAttackState, IsAnimationEndedRandom0Of4());
            // At(_enterRState, _zigzagAttackRState, IsAnimationEndedRandom1Of4());
            // At(_enterRState, _projectileBottomAttackRState, IsAnimationEndedRandom2Of4());
            // At(_enterRState, _projectileDoubleUpAttackRState, IsAnimationEndedRandom3Of4());
            
            At(_enterLState, _wireAttackState, IsAnimationEndedRandom0Of4());
            At(_enterLState, _wireAttackState, IsAnimationEndedRandom1Of4());
            At(_enterLState, _wireAttackState, IsAnimationEndedRandom2Of4());
            At(_enterLState, _wireAttackState, IsAnimationEndedRandom3Of4());
            
            At(_enterRState, _wireAttackState, IsAnimationEndedRandom0Of4());
            At(_enterRState, _wireAttackState, IsAnimationEndedRandom1Of4());
            At(_enterRState, _wireAttackState, IsAnimationEndedRandom2Of4());
            At(_enterRState, _wireAttackState, IsAnimationEndedRandom3Of4());
            
            // Wire Attack Transitions
            At(_wireAttackState, _bounceState, IsCollided());
            At(_wireAttackState, _fallState, IsAttackEndedFail());
            At(_wireAttackState, _deathFallState, IsAttackEndedDeath());
            At(_wireAttackState, _dropFallState, () => _wireAttackState.IsDropped); 
            
            // Zigzag Attack Transitions
            At(_zigzagAttackLState, _bounceState, IsCollided());
            At(_zigzagAttackLState, _fallState, IsAttackEndedFail());          
            At(_zigzagAttackLState, _deathFallState, IsAttackEndedDeath());          
            At(_zigzagAttackRState, _bounceState, IsCollided());
            At(_zigzagAttackRState, _fallState, IsAttackEndedFail());
            At(_zigzagAttackRState, _deathFallState, IsAttackEndedDeath());   
            
            // Projectile Bottom Transitions
            At(_projectileBottomAttackLState, _wireAttackState, IsAnimationEndedRandom0Of4());
            At(_projectileBottomAttackRState, _wireAttackState, IsAnimationEndedRandom0Of4());
            At(_projectileBottomAttackLState, _projectileBottomAttackRState, IsAnimationEndedRandom1Of4());
            At(_projectileBottomAttackRState, _projectileBottomAttackLState, IsAnimationEndedRandom1Of4());
            At(_projectileBottomAttackLState, _projectileTopAttackRState, IsAnimationEndedRandom2Of4());
            At(_projectileBottomAttackRState, _projectileTopAttackLState, IsAnimationEndedRandom2Of4());
            At(_projectileBottomAttackLState, _hangJumpAttackRState, IsAnimationEndedRandom3Of4());
            At(_projectileBottomAttackRState, _hangJumpAttackLState, IsAnimationEndedRandom3Of4());
            
            // Projectile Double Up Transitions
            At(_projectileDoubleUpAttackLState, _wireAttackState, IsAnimationEndedRandom0Of6());
            At(_projectileDoubleUpAttackRState, _wireAttackState, IsAnimationEndedRandom0Of6());
            At(_projectileDoubleUpAttackLState, _hangAttackLState, IsAnimationEndedRandom1Of6());
            At(_projectileDoubleUpAttackRState, _hangAttackRState, IsAnimationEndedRandom1Of6());
            At(_projectileDoubleUpAttackLState, _hangJumpAttackLState, IsAnimationEndedRandom2Of6());
            At(_projectileDoubleUpAttackRState, _hangJumpAttackRState, IsAnimationEndedRandom2Of6());
            At(_projectileDoubleUpAttackLState, _tangleAttackLState, IsAnimationEndedRandom3Of6());
            At(_projectileDoubleUpAttackRState, _tangleAttackRState, IsAnimationEndedRandom3Of6());
            At(_projectileDoubleUpAttackLState, _projectileTopAttackLState, IsAnimationEndedRandom4Of6());
            At(_projectileDoubleUpAttackRState, _projectileTopAttackRState, IsAnimationEndedRandom4Of6());
            At(_projectileDoubleUpAttackLState, _projectileDoubleDownAttackLState, IsAnimationEndedRandom5Of6());
            At(_projectileDoubleUpAttackRState, _projectileDoubleDownAttackRState, IsAnimationEndedRandom5Of6());
            
            // Hang Attack Transitions
            At(_hangAttackLState, _bounceState, IsCollided());
            At(_hangAttackLState, _fallState, IsAttackEndedFail());
            At(_hangAttackLState, _deathFallState, IsAttackEndedDeath());
            At(_hangAttackRState, _bounceState, IsCollided());
            At(_hangAttackRState, _fallState, IsAttackEndedFail());    
            At(_hangAttackRState, _deathFallState, IsAttackEndedDeath());
            
            // Hang Jump Attack Transitions
            At(_hangJumpAttackLState, _bounceState, IsCollided());
            At(_hangJumpAttackLState, _fallState, IsAttackEndedFail());    
            At(_hangJumpAttackLState, _deathFallState, IsAttackEndedDeath());    
            At(_hangJumpAttackRState, _bounceState, IsCollided());
            At(_hangJumpAttackRState, _fallState, IsAttackEndedFail());    
            At(_hangJumpAttackRState, _deathFallState, IsAttackEndedDeath());
            
            // Tangle Attack Transitions
            At(_tangleAttackLState, _bounceState, IsCollided());
            At(_tangleAttackLState, _fallState, IsAttackEndedFail());    
            At(_tangleAttackLState, _deathFallState, IsAttackEndedDeath());    
            At(_tangleAttackRState, _bounceState, IsCollided());
            At(_tangleAttackRState, _fallState, IsAttackEndedFail());
            At(_tangleAttackRState, _deathFallState, IsAttackEndedDeath());
            
            // Projectile Top Attack Transitions
            At(_projectileTopAttackLState, _projectileDoubleDownAttackRState, IsAnimationEndedRandom0Of4());
            At(_projectileTopAttackRState, _projectileDoubleDownAttackLState, IsAnimationEndedRandom0Of4());
            At(_projectileTopAttackLState, _projectileTopAttackRState, IsAnimationEndedRandom1Of4());
            At(_projectileTopAttackRState, _projectileTopAttackLState, IsAnimationEndedRandom1Of4());
            At(_projectileTopAttackLState, _hangJumpAttackRState, IsAnimationEndedRandom2Of4());
            At(_projectileTopAttackRState, _hangJumpAttackLState, IsAnimationEndedRandom2Of4());
            At(_projectileTopAttackLState, _hangAttackRState, IsAnimationEndedRandom3Of4());
            At(_projectileTopAttackRState, _hangAttackLState, IsAnimationEndedRandom3Of4());
            
            // Projectile Double Down Attack Transitions
            At(_projectileDoubleDownAttackLState, _wireAttackState, IsAnimationEndedRandom0Of4());
            At(_projectileDoubleDownAttackRState, _wireAttackState, IsAnimationEndedRandom0Of4());
            At(_projectileDoubleDownAttackLState, _zigzagAttackLState, IsAnimationEndedRandom1Of4());
            At(_projectileDoubleDownAttackRState, _zigzagAttackRState, IsAnimationEndedRandom1Of4());
            At(_projectileDoubleDownAttackLState, _projectileBottomAttackLState, IsAnimationEndedRandom2Of4());
            At(_projectileDoubleDownAttackRState, _projectileBottomAttackRState, IsAnimationEndedRandom2Of4());
            At(_projectileDoubleDownAttackLState, _projectileDoubleUpAttackLState, IsAnimationEndedRandom3Of4());
            At(_projectileDoubleDownAttackRState, _projectileDoubleUpAttackRState, IsAnimationEndedRandom3Of4());
            
            // Bounce Transitions
            At(_bounceState, _fallState, IsAttackEndedFail());
            At(_bounceState, _successFallState, IsAttackEndedSuccess());
            At(_bounceState, _deathFallState, IsAttackEndedDeath());
            
            // Fall Transitions
            At(_fallState, _swingLState, () => _fallState.IsReadyToSwitch && (
                _calculatedTransform.position.x > -_swingZoneSize
                && _calculatedTransform.position.x < 0f)); //+
            At(_fallState, _swingRState, () => _fallState.IsReadyToSwitch && (
                _calculatedTransform.position.x > 0f 
                && _calculatedTransform.position.x < _swingZoneSize));
            At(_fallState, _climbState, () => _fallState.IsReadyToSwitch && (
                _calculatedTransform.position.x < -_swingZoneSize 
                || _calculatedTransform.position.x > _swingZoneSize)); //+
            
            // Success Fall Transitions
            At(_successFallState, _swingLState, () => _successFallState.IsReadyToSwitch && (
                _calculatedTransform.position.x > -_swingZoneSize 
                && _calculatedTransform.position.x < 0f));
            At(_successFallState, _swingRState, () => _successFallState.IsReadyToSwitch && (
                _calculatedTransform.position.x > 0f 
                && _calculatedTransform.position.x < _swingZoneSize));
            At(_successFallState, _climbState, () => _successFallState.IsReadyToSwitch && (
                _calculatedTransform.position.x < -_swingZoneSize 
                || _calculatedTransform.position.x > _swingZoneSize));
            
            // Drop Fall Transitions
            At(_dropFallState, _swingLState, () => _dropFallState.IsReadyToSwitch && (
                _calculatedTransform.position.x > -_swingZoneSize 
                && _calculatedTransform.position.x < 0f));
            At(_dropFallState, _swingRState, () => _dropFallState.IsReadyToSwitch && (
                _calculatedTransform.position.x > 0f 
                && _calculatedTransform.position.x < _swingZoneSize));
            At(_dropFallState, _climbState, () => _dropFallState.IsReadyToSwitch && (
                _calculatedTransform.position.x < -_swingZoneSize 
                || _calculatedTransform.position.x > _swingZoneSize));
            
            // Swing Transitions
            At(_swingLState, _wireAttackState, () => _swingLState.IsReadyToSwitch && Random.Range(0,4) == 0);
            At(_swingRState, _wireAttackState, () => _swingRState.IsReadyToSwitch && Random.Range(0,4) == 0);
            At(_swingLState, _zigzagAttackLState, () => _swingLState.IsReadyToSwitch && Random.Range(0,4) == 1);
            At(_swingRState, _zigzagAttackRState, () => _swingRState.IsReadyToSwitch && Random.Range(0,4) == 1);
            At(_swingLState, _projectileBottomAttackLState, () => _swingLState.IsReadyToSwitch && Random.Range(0,4) == 2);
            At(_swingRState, _projectileBottomAttackRState, () => _swingRState.IsReadyToSwitch && Random.Range(0,4) == 2);
            At(_swingLState, _projectileDoubleUpAttackLState, () => _swingLState.IsReadyToSwitch && Random.Range(0,4) == 3);
            At(_swingRState, _projectileDoubleUpAttackRState, () => _swingRState.IsReadyToSwitch && Random.Range(0,4) == 3);
            
            // Climb Transitions
            At(_climbState, _hangAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 0);
            At(_climbState, _hangAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 0);
            At(_climbState, _hangJumpAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 1);
            At(_climbState, _hangJumpAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 1);
            At(_climbState, _tangleAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 2);
            At(_climbState, _tangleAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 3);
            At(_climbState, _projectileTopAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 3);
            At(_climbState, _projectileTopAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 3);
            At(_climbState, _projectileDoubleDownAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 4);
            At(_climbState, _projectileDoubleDownAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 4);

            // Death Fall Transitions
            At(_deathFallState, _idleState, () => _deathFallState.IsReadyToSwitch); //+
            
            
            // Transition helper methods
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            
            // Predicates 
            Func<bool> IsAnimationEndedRandom0Of4() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 4) == 0)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAnimationEndedRandom1Of4() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 4) == 1)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAnimationEndedRandom2Of4() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 4) == 2)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAnimationEndedRandom3Of4() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 4) == 3)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAnimationEndedRandom0Of6() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 6) == 0)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAnimationEndedRandom1Of6() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 6) == 1)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAnimationEndedRandom2Of6() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 6) == 2)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAnimationEndedRandom3Of6() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 6) == 3)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAnimationEndedRandom4Of6() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 6) == 4)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAnimationEndedRandom5Of6() => () =>
            {
                if (_isAnimClipEnded && Random.Range(0, 6) == 5)
                {
                    _isAnimClipEnded = false;
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
            
            Func<bool> IsAttackEndedSuccess() => () =>
            {
                if (_attackResult == AttackResult.Success)
                {
                    _attackResult = AttackResult.None;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAttackEndedFail() => () =>
            {
                if (_attackResult == AttackResult.Fail)
                {
                    _attackResult = AttackResult.None;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsAttackEndedDeath() => () =>
            {
                if (_attackResult == AttackResult.Death)
                {
                    _attackResult = AttackResult.None;
                    return true;
                }
                return false;
            };
        }

        public void Play()
        {
            _isCollided = false;
            _attackResult = AttackResult.None;
            enabled = true;
            OnEnterStarted();
        }

        public void TriggerFall(AttackResult attackResult)
        {
            _attackResult = attackResult;
        }

        public void TriggerBounce()
        {
            _isCollided = true;
        }

        private void OnEnterStarted()
        {
            int side = Random.Range(0, 2);
            
            if (side == 0)
                _stateMachine.SetState(_enterLState);
            else
                _stateMachine.SetState(_enterRState);
            
            // _stateMachine.SetState(_projectileTopAttackRState);
        }

        private void Update()
        {
            _stateMachine.Tick();
            _currentState = (EnemyMovementStateBase)_stateMachine.CurrentState;
            // Debug.Log(_stateMachine.CurrentState + " : _isBounced = " + _isBounced);
            _stateDebug = _currentState.GetType().Name;
        }

        private void LateUpdate()
        {
            // Fix Movement Penetrations
            // Check Only in Attack State
            if (_isAttackStateBeforeCollision)
            {
                Vector3 currentToLamp = _visibleBodyTransform.position - _lampTransform.position;
                if (currentToLamp.magnitude < _fullCollisionDistance)
                {
                    _visibleBodyTransform.position = 
                        _lampTransform.position 
                        + currentToLamp.normalized 
                        * (_fullCollisionDistance - _collisionThreshold);
                }
            }
        }

        private void OnAnimClipEnded()
        {
            _isAnimClipEnded = true;
        }

        private void OnBounceStateEnded()
        {
            _isAttackStateBeforeCollision = false;
        }

        private void OnDeathStateEnded()
        {
            DeathStateEnded?.Invoke();
        }

        private void OnStaticBridge1Called()
        {
            StaticBridge1Called?.Invoke(_stateMachine.CurrentState.GetType());
        }

        private void OnStaticBridge1Broken()
        {
            StaticBridge1Broken?.Invoke();
        }

        private void OnStaticBridge2Called()
        {
            StaticBridge2Called?.Invoke(_stateMachine.CurrentState.GetType());
        }

        private void OnStaticBridge2Broken()
        {
            StaticBridge2Broken?.Invoke();
        }

        private void OnStaticBridge3Called()
        {
            StaticBridge3Called?.Invoke(_stateMachine.CurrentState.GetType());
        }

        private void OnStaticBridge3Broken()
        {
            StaticBridge3Broken?.Invoke();
        }

        private void OnHangStartRequested()
        {
            if (_stateMachine.CurrentState is MegaspiderEnterLState) 
                HangStartRequested?.Invoke(_stateMachine.CurrentState.GetType(), (MegaspiderEnterLState)_stateMachine.CurrentState);
            if (_stateMachine.CurrentState is MegaspiderEnterRState) 
                HangStartRequested?.Invoke(_stateMachine.CurrentState.GetType(), (MegaspiderEnterRState)_stateMachine.CurrentState);
            if (_stateMachine.CurrentState is MegaspiderHangAttackLState) 
                HangStartRequested?.Invoke(_stateMachine.CurrentState.GetType(), (MegaspiderHangAttackLState)_stateMachine.CurrentState);
            if (_stateMachine.CurrentState is MegaspiderHangAttackRState) 
                HangStartRequested?.Invoke(_stateMachine.CurrentState.GetType(), (MegaspiderHangAttackRState)_stateMachine.CurrentState);
            if (_stateMachine.CurrentState is MegaspiderHangJumpAttackLState) 
                HangStartRequested?.Invoke(_stateMachine.CurrentState.GetType(), (MegaspiderHangJumpAttackLState)_stateMachine.CurrentState);
            if (_stateMachine.CurrentState is MegaspiderHangJumpAttackRState) 
                HangStartRequested?.Invoke(_stateMachine.CurrentState.GetType(), (MegaspiderHangJumpAttackRState)_stateMachine.CurrentState);
        }

        private void OnHangStopRequested()
        {
            HangStopRequested?.Invoke();
        }

        private void OnHangBreakRequested()
        {
            HangBreakRequested?.Invoke();
        }

        private void OnTangleAttackLStarted()
        {
            TangleAttackStarted?.Invoke(_tangleAttackLState);
        }

        private void OnTangleAttackRStarted()
        {
            TangleAttackStarted?.Invoke(_tangleAttackRState);
        }

        private void OnTangleAttackEnded()
        {
            TangleAttackEnded?.Invoke();
        }

        private void OnSuccessFallOutForceCancelled()
        {
            SuccessFallOutForceCancelled?.Invoke(_successFallState);
        }

        private void OnClimbStateEnded()
        {
            ClimbStateEnded?.Invoke();
        }

        private void OnFailFallOutForceCancelled()
        {
            FailFallOutForceCancelled?.Invoke(_fallState);
        }

        private void OnSwingEnded()
        {
            SwingStateEnded?.Invoke();
        }

        private void OnSuccessFallStateEnded()
        {
            FallStateEnded?.Invoke(_successFallState);
        }

        private void OnFailFallStateEnded()
        {
            FallStateEnded?.Invoke(_fallState);
        }

        private void OnWireAttackStateEntered()
        {
            WireAttackStateEntered?.Invoke(_wireAttackState);
        }
    }
}
