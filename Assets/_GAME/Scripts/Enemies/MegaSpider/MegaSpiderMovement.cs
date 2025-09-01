using System;
using System.Collections;
using _GAME.Scripts.Enemies.Dragonfly;
using _GAME.Scripts.Enemies.MegaSpider.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: Add cord attack
// TODO: Add Death state
// TODO: Fix Swing State - take camera into a consideration to decide on bottom Y for Fall/Sucess/Dropfall
// TODO: Fix Swing State - move spiderweb pivot to another position

namespace _GAME.Scripts.Enemies.MegaSpider
{
    public class MegaSpiderMovement : MonoBehaviour, IInitializable
    {
        private const float CollisionThreshold = 0.0001f; // TODO: move to config
        [SerializeField] private Transform _visibleBodyTransform;
        [SerializeField] private Transform _animatedTransform;
        [SerializeField] private Transform _calculatedTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private MegaSpiderAnimationClipEventListener _animationClipEvents;
        [Header( "Movement States")]
        [SerializeField] private string _stateDebug;
        [Header("Tangle Attack Settings")]
        [SerializeField] private AnimationCurve _swingCurve;
        [SerializeField] private AnimationCurve _dropCurve;
        [Header("Bounce Settings")]
        [SerializeField] private float _bounceSpeed;
        [Header("Climb Settings")]
        [SerializeField] private AnimationCurve _climbCurve;
        
        // States
        private readonly StateMachine _stateMachine = new();
        private EnemyMovementStateBase _currentState;
        
        private MegaSpiderIdleState _idleState;
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

        private bool _isAnimClipEnded = false;
        private bool _isBounced = false;
        private AttackResult _attackResult;
        private float _collisionRadius;
        private float _fullCollisionDistance;
        [SerializeField] private bool _isAttackStateBeforeCollision = false;

        // Dependencies
        private Transform _cameraTransform;
        private Transform _lampTransform;
        private MegaSpiderMovementStateFactory _stateFactory;
        
        
        public void Construct(
            Transform cameraTransform, 
            Transform lampTransform, 
            MegaSpiderMovementStateFactory stateFactory)
        {
            _cameraTransform = cameraTransform;
            _lampTransform = lampTransform;
            _stateFactory = stateFactory;
        }
        
        public event Action AnimatedAttackStarted;

        public Vector2 Position => _visibleBodyTransform.position;

        public void SetCollisionRadius(float radius)
        {
            _collisionRadius = radius;
        }
        
        public void Initialize()
        {
            enabled = false;
            _fullCollisionDistance = _collisionRadius + 0.49f;
            Debug.Log($"Full Collision Distance: {_fullCollisionDistance}");
            _isAttackStateBeforeCollision = false;
            _stateFactory.SetEnemyDependencies(
                _animator, 
                _visibleBodyTransform, 
                _animatedTransform,
                _calculatedTransform,
                _swingCurve,
                _dropCurve,
                _bounceSpeed,
                _climbCurve
                );

            CreateMovementStates();
            CreateStateTransitions();

            _animationClipEvents.AnimClipEnded += OnAnimClipEnded;

            _zigzagAttackLState.Started += OnAnimatedAttackStarted;
            _zigzagAttackRState.Started += OnAnimatedAttackStarted;
            _hangAttackLState.Started += OnAnimatedAttackStarted;
            _hangAttackRState.Started += OnAnimatedAttackStarted;
            _hangJumpAttackLState.Started += OnAnimatedAttackStarted;
            _hangJumpAttackRState.Started += OnAnimatedAttackStarted;
            _tangleAttackLState.Started += OnAnimatedAttackStarted;
            _tangleAttackRState.Started += OnAnimatedAttackStarted;
            _wireAttackState.Started += OnAnimatedAttackStarted;

            _bounceState.Ended += OnBounceStateEnded;


        }

        private void OnDestroy()
        {
            _animationClipEvents.AnimClipEnded -= OnAnimClipEnded;
            
            _zigzagAttackLState.Started -= OnAnimatedAttackStarted;
            _zigzagAttackRState.Started -= OnAnimatedAttackStarted;
            _hangAttackLState.Started -= OnAnimatedAttackStarted;
            _hangAttackRState.Started -= OnAnimatedAttackStarted;
            _hangJumpAttackLState.Started -= OnAnimatedAttackStarted;
            _hangJumpAttackRState.Started -= OnAnimatedAttackStarted;
            _tangleAttackLState.Started -= OnAnimatedAttackStarted;
            _tangleAttackRState.Started -= OnAnimatedAttackStarted;
            _wireAttackState.Started -= OnAnimatedAttackStarted;
            
            _bounceState.Ended -= OnBounceStateEnded;
        }

        private void OnAnimatedAttackStarted()
        {
            _isAttackStateBeforeCollision = true;
            _isBounced = false;
            AnimatedAttackStarted?.Invoke();
        }

        private void CreateMovementStates()
        {
            _idleState = (MegaSpiderIdleState)_stateFactory.Create(typeof(MegaSpiderIdleState));
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
        }

        private void CreateStateTransitions()
        {
            // Initialize StateMachine 
            // Enter to Attacks
            At(_enterLState, _wireAttackState, IsAnimationEndedRandom0Of4()); //++
            At(_enterLState, _zigzagAttackLState, IsAnimationEndedRandom1Of4());//+
            At(_enterLState, _projectileBottomAttackLState, IsAnimationEndedRandom2Of4());//+
            At(_enterLState, _projectileDoubleUpAttackLState, IsAnimationEndedRandom3Of4());//++
            
            At(_enterRState, _wireAttackState, IsAnimationEndedRandom0Of4());//++
            At(_enterRState, _zigzagAttackRState, IsAnimationEndedRandom1Of4());//+
            At(_enterRState, _projectileBottomAttackRState, IsAnimationEndedRandom2Of4());//+
            At(_enterRState, _projectileDoubleUpAttackRState, IsAnimationEndedRandom3Of4());//+
            
            // Wire Attack Transitions
            At(_wireAttackState, _bounceState, () => _wireAttackState.IsReadyToSwitch); //+
            At(_wireAttackState, _fallState, IsAttackEndedFail()); //+
            At(_wireAttackState, _dropFallState, () => _wireAttackState.IsDropped); 
            
            // Zigzag Attack Transitions
            At(_zigzagAttackLState, _bounceState, IsBounced()); //+
            At(_zigzagAttackLState, _fallState, IsAttackEndedFail());          
            At(_zigzagAttackRState, _bounceState, IsBounced()); //+
            At(_zigzagAttackRState, _fallState, IsAttackEndedFail());         
            
            // Projectile Bottom Transitions
            At(_projectileBottomAttackLState, _wireAttackState, IsAnimationEndedRandom0Of4()); //+
            At(_projectileBottomAttackRState, _wireAttackState, IsAnimationEndedRandom0Of4()); //+
            At(_projectileBottomAttackLState, _projectileBottomAttackRState, IsAnimationEndedRandom1Of4()); //+
            At(_projectileBottomAttackRState, _projectileBottomAttackLState, IsAnimationEndedRandom1Of4()); //+
            At(_projectileBottomAttackLState, _projectileTopAttackRState, IsAnimationEndedRandom2Of4()); //+
            At(_projectileBottomAttackRState, _projectileTopAttackLState, IsAnimationEndedRandom2Of4()); //+
            At(_projectileBottomAttackLState, _hangJumpAttackRState, IsAnimationEndedRandom3Of4()); //+
            At(_projectileBottomAttackRState, _hangJumpAttackLState, IsAnimationEndedRandom3Of4()); //+
            
            // Projectile Double Up Transitions
            At(_projectileDoubleUpAttackLState, _wireAttackState, IsAnimationEndedRandom0Of6()); //+
            At(_projectileDoubleUpAttackRState, _wireAttackState, IsAnimationEndedRandom0Of6()); //+
            At(_projectileDoubleUpAttackLState, _hangAttackLState, IsAnimationEndedRandom1Of6()); //+
            At(_projectileDoubleUpAttackRState, _hangAttackRState, IsAnimationEndedRandom1Of6()); //+
            At(_projectileDoubleUpAttackLState, _hangJumpAttackLState, IsAnimationEndedRandom2Of6()); //+
            At(_projectileDoubleUpAttackRState, _hangJumpAttackRState, IsAnimationEndedRandom2Of6()); //+
            At(_projectileDoubleUpAttackLState, _tangleAttackLState, IsAnimationEndedRandom3Of6()); //+
            At(_projectileDoubleUpAttackRState, _tangleAttackRState, IsAnimationEndedRandom3Of6()); //+
            At(_projectileDoubleUpAttackLState, _projectileTopAttackLState, IsAnimationEndedRandom4Of6()); //+
            At(_projectileDoubleUpAttackRState, _projectileTopAttackRState, IsAnimationEndedRandom4Of6()); //+
            At(_projectileDoubleUpAttackLState, _projectileDoubleDownAttackLState, IsAnimationEndedRandom5Of6()); //+
            At(_projectileDoubleUpAttackRState, _projectileDoubleDownAttackRState, IsAnimationEndedRandom5Of6()); //+
            
            // Hang Attack Transitions
            At(_hangAttackLState, _bounceState, IsBounced()); //+
            At(_hangAttackLState, _fallState, IsAttackEndedFail());         
            At(_hangAttackRState, _bounceState, IsBounced()); //+
            At(_hangAttackRState, _fallState, IsAttackEndedFail());    
            
            // Hang Jump Attack Transitions
            At(_hangJumpAttackLState, _bounceState, IsBounced()); //+
            At(_hangJumpAttackLState, _fallState, IsAttackEndedFail());    
            At(_hangJumpAttackRState, _bounceState, IsBounced()); //+
            At(_hangJumpAttackRState, _fallState, IsAttackEndedFail());    
            
            // Tangle Attack Transitions
            At(_tangleAttackLState, _bounceState, IsBounced()); //+
            At(_tangleAttackLState, _fallState, IsAttackEndedFail());    
            At(_tangleAttackRState, _bounceState, IsBounced()); //+
            At(_tangleAttackRState, _fallState, IsAttackEndedFail());
            
            // Projectile Top Attack Transitions
            At(_projectileTopAttackLState, _projectileDoubleDownAttackRState, IsAnimationEndedRandom0Of4()); //+
            At(_projectileTopAttackRState, _projectileDoubleDownAttackLState, IsAnimationEndedRandom0Of4()); //+
            At(_projectileTopAttackLState, _projectileTopAttackRState, IsAnimationEndedRandom1Of4()); //+
            At(_projectileTopAttackRState, _projectileTopAttackLState, IsAnimationEndedRandom1Of4()); //+
            At(_projectileTopAttackLState, _hangJumpAttackRState, IsAnimationEndedRandom2Of4()); //+
            At(_projectileTopAttackRState, _hangJumpAttackLState, IsAnimationEndedRandom2Of4()); //+
            At(_projectileTopAttackLState, _hangAttackRState, IsAnimationEndedRandom3Of4()); //+
            At(_projectileTopAttackRState, _hangAttackLState, IsAnimationEndedRandom3Of4()); //+
            
            // Projectile Double Down Attack Transitions
            At(_projectileDoubleDownAttackLState, _wireAttackState, IsAnimationEndedRandom0Of4()); //+
            At(_projectileDoubleDownAttackRState, _wireAttackState, IsAnimationEndedRandom0Of4()); //+
            At(_projectileDoubleDownAttackLState, _zigzagAttackLState, IsAnimationEndedRandom1Of4()); //+
            At(_projectileDoubleDownAttackRState, _zigzagAttackRState, IsAnimationEndedRandom1Of4()); //+
            At(_projectileDoubleDownAttackLState, _projectileBottomAttackLState, IsAnimationEndedRandom2Of4());//+
            At(_projectileDoubleDownAttackRState, _projectileBottomAttackRState, IsAnimationEndedRandom2Of4()); //+
            At(_projectileDoubleDownAttackLState, _projectileDoubleUpAttackLState, IsAnimationEndedRandom3Of4()); //+
            At(_projectileDoubleDownAttackRState, _projectileDoubleUpAttackRState, IsAnimationEndedRandom3Of4()); //+
            
            // Bounce Transitions
            At(_bounceState, _fallState, IsAttackEndedFail()); //+
            At(_bounceState, _successFallState, IsAttackEndedSuccess());
            
            // Fall Transitions
            // TODO: fix magic numbers - tune number to make swing more often
            At(_fallState, _swingLState, () => _fallState.IsReadyToSwitch && (_calculatedTransform.position.x > -0.9f && _calculatedTransform.position.x < 0f)); //+
            At(_fallState, _swingRState, () => _fallState.IsReadyToSwitch && (_calculatedTransform.position.x > 0f && _calculatedTransform.position.x < 0.9f));
            At(_fallState, _climbState, () => _fallState.IsReadyToSwitch && (_calculatedTransform.position.x < -0.9f || _calculatedTransform.position.x > 0.9f)); //+
            
            // Success Fall Transitions
            At(_successFallState, _swingLState, () => _successFallState.IsReadyToSwitch && (_calculatedTransform.position.x > -0.9f && _calculatedTransform.position.x < 0f));
            At(_successFallState, _swingRState, () => _successFallState.IsReadyToSwitch && (_calculatedTransform.position.x > 0f && _calculatedTransform.position.x < 0.9f));
            At(_successFallState, _climbState, () => _successFallState.IsReadyToSwitch && (_calculatedTransform.position.x < -0.9f || _calculatedTransform.position.x > 0.9f));
            
            // Drop Fall Transitions
            At(_dropFallState, _swingLState, () => _dropFallState.IsReadyToSwitch && (_calculatedTransform.position.x > -0.9f && _calculatedTransform.position.x < 0f));
            At(_dropFallState, _swingRState, () => _dropFallState.IsReadyToSwitch && (_calculatedTransform.position.x > 0f && _calculatedTransform.position.x < 0.9f));
            At(_dropFallState, _climbState, () => _dropFallState.IsReadyToSwitch && (_calculatedTransform.position.x < -0.9f || _calculatedTransform.position.x > 0.9f));
            
            // Swing Transitions
            At(_swingLState, _wireAttackState, () => _swingLState.IsReadyToSwitch && Random.Range(0,4) == 0); //+
            At(_swingRState, _wireAttackState, () => _swingRState.IsReadyToSwitch && Random.Range(0,4) == 0); //+
            At(_swingLState, _zigzagAttackLState, () => _swingLState.IsReadyToSwitch && Random.Range(0,4) == 1); //+
            At(_swingRState, _zigzagAttackRState, () => _swingRState.IsReadyToSwitch && Random.Range(0,4) == 1); //+
            At(_swingLState, _projectileBottomAttackLState, () => _swingLState.IsReadyToSwitch && Random.Range(0,4) == 2); //+
            At(_swingRState, _projectileBottomAttackRState, () => _swingRState.IsReadyToSwitch && Random.Range(0,4) == 2); //+
            At(_swingLState, _projectileDoubleUpAttackLState, () => _swingLState.IsReadyToSwitch && Random.Range(0,4) == 3); //+
            At(_swingRState, _projectileDoubleUpAttackRState, () => _swingRState.IsReadyToSwitch && Random.Range(0,4) == 3); //+
            
            // Climb Transitions
            At(_climbState, _hangAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 0); //+
            At(_climbState, _hangAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 0); //+
            At(_climbState, _hangJumpAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 1); //+
            At(_climbState, _hangJumpAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 1);
            At(_climbState, _tangleAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 2); //+
            At(_climbState, _tangleAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 3); //+
            At(_climbState, _projectileTopAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 3); //+
            At(_climbState, _projectileTopAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 3); //+
            At(_climbState, _projectileDoubleDownAttackLState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x < 0 && Random.Range(0,5) == 4); //+
            At(_climbState, _projectileDoubleDownAttackRState, () => _climbState.IsReadyToSwitch && _calculatedTransform.position.x > 0 && Random.Range(0,5) == 4); //+

            
            
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
            
            Func<bool> IsBounced() => () =>
            {
                if (_isBounced)
                {
                    _isBounced = false;
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
            _isBounced = false;
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
            _isBounced = true;
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
            // Fix Movement Penetrations TODO: extract to a separate class?
            // Check Only in Attack State
            if (_isAttackStateBeforeCollision)
            {
                Vector3 currentToLamp = _visibleBodyTransform.position - _lampTransform.position;
                if (currentToLamp.magnitude < _fullCollisionDistance)
                {
                    _visibleBodyTransform.position = 
                        _lampTransform.position 
                        + currentToLamp.normalized 
                        * (_fullCollisionDistance - CollisionThreshold);
                }
            }
        }

        private void OnAnimClipEnded()
        {
            Debug.Log("AnimClipEnded");
            _isAnimClipEnded = true;
        }

        private void OnBounceStateEnded()
        {
            _isAttackStateBeforeCollision = false;
        }
    }
}
