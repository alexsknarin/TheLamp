using System;
using _GAME.Scripts.Enemies.Dragonfly.FMovementStates;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly
{
    public class DragonflyMovement : MonoBehaviour, IInitializable
    {
        [SerializeField] private string _currentStateType;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _visibleBodyTransform;
        [SerializeField] private Transform _animatedTransform;
        [SerializeField] private Transform _patrolTransform;
        [SerializeField] private Transform _spiderPatrolTransform;
        [SerializeField] private DragonflyPatrolRotator _patrolRotator;
        [SerializeField] private DragonflyPatrolRotator _spiderPatrolRotator;
        [SerializeField] private DragonflyAnimationClipEventListener _animationClipEvents;
        [SerializeField] private Transform _fallPoint;
        [Header("States")]  
        [SerializeField] private FDragonflyAttackHeadState _attackHeadState;
        [SerializeField] private FDragonflyAttackHeadSuccessState _attackHeadSuccessState; 
        [SerializeField] private FDragonflyAttackHoverState _attackHoverState;
        [SerializeField] private FDragonflyAttackTailFailStateL _attackTailFailL;
        [SerializeField] private FDragonflyAttackTailFailStateR _attackTailFailR;
        [SerializeField] private FDragonflyAttackTailStateL _attackTailStateL;
        [SerializeField] private FDragonflyAttackTailStateR _attackTailStateR;
        [SerializeField] private FDragonflyAttackTailSuccessStateL _attackTailSuccessL;
        [SerializeField] private FDragonflyAttackTailSuccessStateR _attackTailSuccessR;
        [SerializeField] private FDragonflyBounceHeadState _bounceHeadState;
        [SerializeField] private FDragonflyBounceHoverState _bounceHoverState;
        [SerializeField] private FDragonflyBounceTailStateL _bounceTailStateL;
        [SerializeField] private FDragonflyBounceTailStateR _bounceTailStateR;
        [SerializeField] private FDragonflyCatchSpiderStateL _catchSpiderStateL;
        [SerializeField] private FDragonflyCatchSpiderStateR _catchSpiderStateR;
        [SerializeField] private FDragonflyDeathHeadState _deathHeadState;
        [SerializeField] private FDragonflyDeathTailStateL _deathTailStateL;
        [SerializeField] private FDragonflyDeathTailStateR _deathTailStateR;
        [SerializeField] private FDragonflyEnterToHoverStateL _enterToHoverStateL;
        [SerializeField] private FDragonflyEnterToHoverStateR _enterToHoverStateR;
        [SerializeField] private FDragonflyEnterToPatrolStateL _enterToPatrolStateL;
        [SerializeField] private FDragonflyEnterToPatrolStateR _enterToPatrolStateR;
        [SerializeField] private FDragonflyFallHeadState _fallHeadState;
        [SerializeField] private FDragonflyHoverState _hoverState;
        [SerializeField] private FDragonflyGameoverHoverState _gameoverHoverState;
        [SerializeField] private FDragonflyIdleState _idleState;
        [SerializeField] private FDragonflyMoveToHoverState _moveToHoverState;
        [SerializeField] private FDragonflyMoveToPatrolStateL _moveToPatrolStateL;
        [SerializeField] private FDragonflyMoveToPatrolStateR _moveToPatrolStateR;
        [SerializeField] private FDragonflyPatrolStateL _patrolStateL;
        [SerializeField] private FDragonflyPatrolStateR _patrolStateR;
        [SerializeField] private FDragonflyPreAttackHeadStateL _preAttackHeadStateL;
        [SerializeField] private FDragonflyPreAttackHeadStateR _preAttackHeadStateR;
        [SerializeField] private FDragonflyPreAttackHoverState _preAttackHoverState;
        [SerializeField] private FDragonflyPreAttackTailStateL _preAttackTailStateL;
        [SerializeField] private FDragonflyPreAttackTailStateR _preAttackTailStateR;
        [SerializeField] private FDragonflyReturnHoverState _returnHoverState;
        [SerializeField] private FDragonflyReturnTransitionLRBTState _returnTransitionLRBTState;
        [SerializeField] private FDragonflyReturnTransitionLRTBState _returnTransitionLRTBState;
        [SerializeField] private FDragonflyReturnTransitionRLBTState _returnTransitionRLBTState;
        [SerializeField] private FDragonflyReturnTransitionRLTBState _returnTransitionRLTBState;
        [SerializeField] private FDragonflySpiderPatrolStateL _spiderPatrolStateL;
        [SerializeField] private FDragonflySpiderPatrolStateR _spiderPatrolStateR;
        [SerializeField] private FDragonflySpiderPreattackHeadTransitionStateL _spiderPreAttackHeadTransitionStateL;
        [SerializeField] private FDragonflySpiderPreattackHeadTransitionStateR _spiderPreAttackHeadTransitionStateR;
        [SerializeField] private FDragonflySpiderPushStateL _spiderPushStateL;
        [SerializeField] private FDragonflySpiderPushStateR _spiderPushStateR;
        private StateMachine _stateMachine = new();
        // Animation 
        private readonly int _idleHash = Animator.StringToHash("Idle");
        private readonly int _enterToPatrolLHash = Animator.StringToHash("EnterToPatrolL");
        private readonly int _enterToPatrolRHash = Animator.StringToHash("EnterToPatrolR");
        private readonly int _catchSpiderLHash = Animator.StringToHash("CatchSpiderL");
        private readonly int _catchSpiderRHash = Animator.StringToHash("CatchSpiderR");
        private readonly int _enterToHoverLHash = Animator.StringToHash("EnterToHoverL");
        private readonly int _enterToHoverRHash = Animator.StringToHash("EnterToHoverR");
        private readonly int _moveToPatrolLHash = Animator.StringToHash("MoveToPatrolL");
        private readonly int _moveToPatrolRHash = Animator.StringToHash("MoveToPatrolR");
        private readonly int _returnTransitionLRBTHash = Animator.StringToHash("ReturnTransitionLRBT");
        private readonly int _returnTransitionLRTBHash = Animator.StringToHash("ReturnTransitionLRTB");
        private readonly int _returnTransitionRLBTHash = Animator.StringToHash("ReturnTransitionRLBT");
        private readonly int _returnTransitionRLTBHash = Animator.StringToHash("ReturnTransitionRLTB");
        // Attack Modes
        [SerializeField] private bool _isAttacking;
        [SerializeField] private PatrolAttackMode _currentPatrolAttackMode;
        // Tracking previous state
        private IState _previousState;
        private ReturnMode _resolvedReturnMode;
        // Return Resolve
        private int _returnSideDirection;
        private bool _isPlaying = false;
        private bool _isAnimClipEnded = false;
        [SerializeField] private bool _isBounced = false;
        private EnterType _enterState = 0;
        [SerializeField] private int _sideDirection = 1;
        
        private AttackResult _attackResult;
        
        [SerializeField] private bool _isLampDestroyed;
    
        // Events
        public event Action<IState> ReadyHoverToAttackStateEntered; 
        public event Action<IState> ReadyToSwarmAttackStateEntered;
        public event Action<IState> AfterAttackExitEnded;
        public event Action AttackStarted;
        public event Action PreAttackStarted;
        public event Action AttackEnded;
        public event Action<int> CatchSpiderStarted;
        public event Action ReadyToSpiderAttackStateStarted;
        public event Action DeathAnimationEnded;
        public event Action SwarmCalled;
        public event Action CollisionPhaseReached;
        
        // State Events
        public event Action AttackHeadStarted;
        public event Action AttackHeadSuccessStarted;
        public event Action AttackHoverStarted;
        public event Action AttackTailFailLStarted;
        public event Action AttackTailFailRStarted;
        public event Action AttackTailLStarted;
        public event Action AttackTailRStarted;
        public event Action AttackTailSuccessLStarted;
        public event Action AttackTailSuccessRStarted;
        public event Action BounceHeadStarted;
        public event Action BounceHoverStarted;
        public event Action BounceTailLStarted;
        public event Action BounceTailRStarted;
        public event Action CatchSpiderLStarted;
        public event Action CatchSpiderRStarted;
        public event Action DeathHeadStarted;
        public event Action DeathTailLStarted;
        public event Action DeathTailRStarted;
        public event Action EnterToHoverLStarted;
        public event Action EnterToHoverRStarted;
        public event Action EnterToPatrolLStarted;
        public event Action EnterToPatrolRStarted;
        public event Action FallHeadStarted;
        public event Action HoverStarted;
        public event Action GameoverHoverStarted;
        public event Action MoveToHoverStarted;
        public event Action MoveToPatrolLStarted;
        public event Action MoveToPatrolRStarted;
        public event Action PatrolLStarted;
        public event Action PatrolRStarted;
        public event Action PreAttackHeadLStarted;
        public event Action PreAttackHeadRStarted;
        public event Action PreAttackHoverStarted;
        public event Action PreAttackTailLStarted;
        public event Action PreAttackTailRStarted;
        public event Action ReturnHoverStarted;
        public event Action ReturnTransitionLRBTStarted;
        public event Action ReturnTransitionLRTBStarted;
        public event Action ReturnTransitionRLBTStarted;
        public event Action ReturnTransitionRLTBStarted;
        public event Action SpiderPatrolLStarted;
        public event Action SpiderPatrolRStarted;
        public event Action SpiderPreattackHeadTransitionLStarted;
        public event Action SpiderPreattackHeadTransitionRStarted;
        public event Action SpiderPushLStarted;
        public event Action SpiderPushRStarted;
    
    
        public IState MovementState => _stateMachine.CurrentState;
        public int SideDirection => _sideDirection;

        public void Initialize()
        {
            _isPlaying = false;
            _isAnimClipEnded = false;
            _isBounced = false;
            
            SetMovementStatesDependencies();
            StateMachineSetup();
        
            _stateMachine.SetState(_idleState);
            _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FDragonfly", ""); // DEBUG
            
            _animationClipEvents.AnimClipEnded += OnAnimClipEnded;
            _animationClipEvents.SwarmCalled += OnSwarmCalled;
            _spiderPushStateL.Ended += OnSwarmCalled;
            _spiderPushStateR.Ended += OnSwarmCalled;
        
            _hoverState.Started += OnReadyToHoverAttackEnter;
        
            _patrolStateL.Started += OnReadyToSwarmAttackEnter;
            _patrolStateR.Started += OnReadyToSwarmAttackEnter;
        
            _spiderPatrolStateL.Started += OnReadyToSpiderAttack;
            _spiderPatrolStateR.Started += OnReadyToSpiderAttack;
        
            _catchSpiderStateL.Started += OnCatchSpiderStartedL;
            _catchSpiderStateR.Started += OnCatchSpiderStartedR;
        
            _preAttackHeadStateL.Started += OnPreAttackStarted;
            _preAttackHeadStateR.Started += OnPreAttackStarted;
            _preAttackTailStateL.Started += OnPreAttackStarted;
            _preAttackTailStateR.Started += OnPreAttackStarted;
            _preAttackHoverState.Started += OnPreAttackStarted;
        
            _attackHeadState.Started += OnAttackStarted;
            _attackTailStateL.Started += OnAttackStarted;
            _attackTailStateR.Started += OnAttackStarted;
            _attackHoverState.Started += OnAttackStarted;
        
            _deathHeadState.Started += OnAttackEnded;
            _deathTailStateL.Started += OnAttackEnded;
            _deathTailStateR.Started += OnAttackEnded;
            _attackHeadSuccessState.Started += OnAttackEnded;
            _fallHeadState.Started += OnAttackEnded;
            _attackTailFailL.Started += OnAttackEnded;
            _attackTailFailR.Started += OnAttackEnded;
            _attackTailSuccessL.Started += OnAttackEnded;
            _attackTailSuccessR.Started += OnAttackEnded;
            _returnHoverState.Started += OnAttackEnded;
        
            _attackHeadSuccessState.Ended += OnAfterAttackExitEnded;
            _attackTailSuccessL.Ended += OnAfterAttackExitEnded;
            _attackTailSuccessR.Ended += OnAfterAttackExitEnded;
            _attackTailFailL.Ended += OnAfterAttackExitEnded;
            _attackTailFailR.Ended += OnAfterAttackExitEnded;
            _fallHeadState.Ended += OnAfterAttackExitEnded;
            _returnHoverState.Ended += OnAfterAttackExitEnded;
            _deathHeadState.Ended += OnAfterAttackExitEnded;
            _deathTailStateL.Ended += OnAfterAttackExitEnded;
            _deathTailStateR.Ended += OnAfterAttackExitEnded;
        
            _deathHeadState.Ended += OnDeathAnimationEnded;
            _deathTailStateL.Ended += OnDeathAnimationEnded;
            _deathTailStateR.Ended += OnDeathAnimationEnded;
        
            // _Collision Refresh events:
            _attackTailStateL.CollisionPhaseReached += OnCollisionPhaseReached;
            _attackTailStateR.CollisionPhaseReached += OnCollisionPhaseReached;
            _attackHeadState.CollisionPhaseReached += OnCollisionPhaseReached;
            _attackHoverState.CollisionPhaseReached += OnCollisionPhaseReached;
            
            // State events
            _attackHeadState.Started += OnAttackHeadStarted;
            _attackHeadSuccessState.Started += OnAttackHeadSuccessStarted;
            _attackHoverState.Started += OnAttackHoverStarted;
            _attackTailFailL.Started += OnAttackTailFailLStarted; 
            _attackTailFailR.Started += OnAttackTailFailRStarted;
            _attackTailStateL.Started += OnAttackTailLStarted;
            _attackTailStateR.Started += OnAttackTailRStarted;
            _attackTailSuccessL.Started += OnAttackTailSuccessLStarted; 
            _attackTailSuccessR.Started += OnAttackTailSuccessRStarted; 
            _bounceHeadState.Started += OnBounceHeadStarted;
            _bounceHoverState.Started += OnBounceHoverStarted;
            _bounceTailStateL.Started += OnBounceTailLStarted;
            _bounceTailStateR.Started += OnBounceTailRStarted;
            _catchSpiderStateL.Started += OnCatchSpiderLStarted;
            _catchSpiderStateR.Started += OnCatchSpiderRStarted;
            _deathHeadState.Started += OnDeathHeadStarted;
            _deathTailStateL.Started += OnDeathTailLStarted;
            _deathTailStateR.Started += OnDeathTailRStarted;
            _enterToHoverStateL.Started += OnEnterToHoverLStarted;
            _enterToHoverStateR.Started += OnEnterToHoverRStarted;
            _enterToPatrolStateL.Started += OnEnterToPatrolLStarted;
            _enterToPatrolStateR.Started += OnEnterToPatrolRStarted;
            _fallHeadState.Started += OnFallHeadStarted;
            _hoverState.Started += OnHoverStarted;
            _moveToHoverState.Started += OnMoveToHoverStarted;
            _moveToPatrolStateL.Started += OnMoveToPatrolLStarted;
            _moveToPatrolStateR.Started += OnMoveToPatrolRStarted;
            _patrolStateL.Started += OnPatrolLStarted;
            _patrolStateR.Started += OnPatrolRStarted;
            _preAttackHeadStateL.Started += OnPreAttackHeadLStarted;
            _preAttackHeadStateR.Started += OnPreAttackHeadRStarted;
            _preAttackHoverState.Started += OnPreAttackHoverStarted;
            _preAttackTailStateL.Started += OnPreAttackTailLStarted;
            _preAttackTailStateR.Started += OnPreAttackTailRStarted;
            _returnHoverState.Started += OnReturnHoverStarted;
            _returnTransitionLRBTState.Started += OnReturnTransitionLRBTStarted;
            _returnTransitionLRTBState.Started += OnReturnTransitionLRTBStarted;
            _returnTransitionRLBTState.Started += OnReturnTransitionRLBTStarted;
            _returnTransitionRLTBState.Started += OnReturnTransitionRLTBStarted;
            _spiderPatrolStateL.Started += OnSpiderPatrolLStarted;
            _spiderPatrolStateR.Started += OnSpiderPatrolRStarted;
            _spiderPreAttackHeadTransitionStateL.Started += OnSpiderPreattackHeadTransitionLStarted;
            _spiderPreAttackHeadTransitionStateR.Started += OnSpiderPreattackHeadTransitionRStarted;
            _spiderPushStateL.Started += OnSpiderPushLStarted;
            _spiderPushStateR.Started += OnSpiderPushRStarted;
        }

        private void OnDestroy()
        {
            _animationClipEvents.AnimClipEnded -= OnAnimClipEnded;
            _animationClipEvents.SwarmCalled -= OnSwarmCalled;
            _spiderPushStateL.Ended -= OnSwarmCalled;
            _spiderPushStateR.Ended -= OnSwarmCalled;
        
            _hoverState.Started -= OnReadyToHoverAttackEnter;
        
            _patrolStateL.Started -= OnReadyToSwarmAttackEnter;
            _patrolStateR.Started -= OnReadyToSwarmAttackEnter;
        
            _spiderPatrolStateL.Started -= OnReadyToSpiderAttack;
            _spiderPatrolStateR.Started -= OnReadyToSpiderAttack;
        
            _catchSpiderStateL.Started -= OnCatchSpiderStartedL;
            _catchSpiderStateR.Started -= OnCatchSpiderStartedR;
        
            _preAttackHeadStateL.Started -= OnPreAttackStarted;
            _preAttackHeadStateR.Started -= OnPreAttackStarted;
            _preAttackTailStateL.Started -= OnPreAttackStarted;
            _preAttackTailStateR.Started -= OnPreAttackStarted;
            _preAttackHoverState.Started -= OnPreAttackStarted;
        
            _attackHeadState.Started -= OnAttackStarted;
            _attackTailStateL.Started -= OnAttackStarted;
            _attackTailStateR.Started -= OnAttackStarted;
            _attackHoverState.Started -= OnAttackStarted;
        
            _deathHeadState.Started -= OnAttackEnded;
            _deathTailStateL.Started -= OnAttackEnded;
            _deathTailStateR.Started -= OnAttackEnded;
            _attackHeadSuccessState.Started -= OnAttackEnded;
            _fallHeadState.Started -= OnAttackEnded;
            _attackTailFailL.Started -= OnAttackEnded;
            _attackTailFailR.Started -= OnAttackEnded;
            _attackTailSuccessL.Started -= OnAttackEnded;
            _attackTailSuccessR.Started -= OnAttackEnded;
            _returnHoverState.Started -= OnAttackEnded;
        
            _attackHeadSuccessState.Ended -= OnAfterAttackExitEnded;
            _attackTailSuccessL.Ended -= OnAfterAttackExitEnded;
            _attackTailSuccessR.Ended -= OnAfterAttackExitEnded;
            _attackTailFailL.Ended -= OnAfterAttackExitEnded;
            _attackTailFailR.Ended -= OnAfterAttackExitEnded;
            _fallHeadState.Ended -= OnAfterAttackExitEnded;
            _returnHoverState.Ended -= OnAfterAttackExitEnded;
            _deathHeadState.Ended -= OnAfterAttackExitEnded;
            _deathTailStateL.Ended -= OnAfterAttackExitEnded;
            _deathTailStateR.Ended -= OnAfterAttackExitEnded;
        
            _deathHeadState.Ended -= OnDeathAnimationEnded;
            _deathTailStateL.Ended -= OnDeathAnimationEnded;
            _deathTailStateR.Ended -= OnDeathAnimationEnded;
        
            // _Collision Refresh events:
            _attackTailStateL.CollisionPhaseReached -= OnCollisionPhaseReached;
            _attackTailStateR.CollisionPhaseReached -= OnCollisionPhaseReached;
            _attackHeadState.CollisionPhaseReached -= OnCollisionPhaseReached;
            _attackHoverState.CollisionPhaseReached -= OnCollisionPhaseReached;
            
            // State events
            _attackHeadState.Started -= OnAttackHeadStarted;
            _attackHeadSuccessState.Started -= OnAttackHeadSuccessStarted;
            _attackHoverState.Started -= OnAttackHoverStarted;
            _attackTailFailL.Started -= OnAttackTailFailLStarted; 
            _attackTailFailR.Started -= OnAttackTailFailRStarted;
            _attackTailStateL.Started -= OnAttackTailLStarted;
            _attackTailStateR.Started -= OnAttackTailRStarted;
            _attackTailSuccessL.Started -= OnAttackTailSuccessLStarted; 
            _attackTailSuccessR.Started -= OnAttackTailSuccessRStarted; 
            _bounceHeadState.Started -= OnBounceHeadStarted;
            _bounceHoverState.Started -= OnBounceHoverStarted;
            _bounceTailStateL.Started -= OnBounceTailLStarted;
            _bounceTailStateR.Started -= OnBounceTailRStarted;
            _catchSpiderStateL.Started -= OnCatchSpiderLStarted;
            _catchSpiderStateR.Started -= OnCatchSpiderRStarted;
            _deathHeadState.Started -= OnDeathHeadStarted;
            _deathTailStateL.Started -= OnDeathTailLStarted;
            _deathTailStateR.Started -= OnDeathTailRStarted;
            _enterToHoverStateL.Started -= OnEnterToHoverLStarted;
            _enterToHoverStateR.Started -= OnEnterToHoverRStarted;
            _enterToPatrolStateL.Started -= OnEnterToPatrolLStarted;
            _enterToPatrolStateR.Started -= OnEnterToPatrolRStarted;
            _fallHeadState.Started -= OnFallHeadStarted;
            _hoverState.Started -= OnHoverStarted;
            _moveToHoverState.Started -= OnMoveToHoverStarted;
            _moveToPatrolStateL.Started -= OnMoveToPatrolLStarted;
            _moveToPatrolStateR.Started -= OnMoveToPatrolRStarted;
            _patrolStateL.Started -= OnPatrolLStarted;
            _patrolStateR.Started -= OnPatrolRStarted;
            _preAttackHeadStateL.Started -= OnPreAttackHeadLStarted;
            _preAttackHeadStateR.Started -= OnPreAttackHeadRStarted;
            _preAttackHoverState.Started -= OnPreAttackHoverStarted;
            _preAttackTailStateL.Started -= OnPreAttackTailLStarted;
            _preAttackTailStateR.Started -= OnPreAttackTailRStarted;
            _returnHoverState.Started -= OnReturnHoverStarted;
            _returnTransitionLRBTState.Started -= OnReturnTransitionLRBTStarted;
            _returnTransitionLRTBState.Started -= OnReturnTransitionLRTBStarted;
            _returnTransitionRLBTState.Started -= OnReturnTransitionRLBTStarted;
            _returnTransitionRLTBState.Started -= OnReturnTransitionRLTBStarted;
            _spiderPatrolStateL.Started -= OnSpiderPatrolLStarted;
            _spiderPatrolStateR.Started -= OnSpiderPatrolRStarted;
            _spiderPreAttackHeadTransitionStateL.Started -= OnSpiderPreattackHeadTransitionLStarted;
            _spiderPreAttackHeadTransitionStateR.Started -= OnSpiderPreattackHeadTransitionRStarted;
            _spiderPushStateL.Started -= OnSpiderPushLStarted;
            _spiderPushStateR.Started -= OnSpiderPushRStarted;
        }

        public void Play(EnterType state, int sideDirection)
        {
            _isAnimClipEnded = false;
            _sideDirection = sideDirection;
            _enterState = state;
            _isPlaying = true;
            _isBounced = false;
            _isLampDestroyed = false;
            _isAttacking = false;
            
            _attackResult = AttackResult.None;
            
            _stateMachine.SetState(_idleState);
            _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FDragonfly", ""); // DEBUG
        }

        public void StartAttack(PatrolAttackMode mode)
        {
            _currentPatrolAttackMode = mode;
            _isAttacking = true;
        }

        public void ResolveReturnTransition(ReturnMode mode)
        {
            _previousState = _stateMachine.CurrentState;
            _resolvedReturnMode = mode;
            IState resolvedState = null;
        
            switch (mode)
            {
                case ReturnMode.PatrolL:
                    resolvedState = _moveToPatrolStateL;
                    break;
                case ReturnMode.PatrolR:
                    resolvedState = _moveToPatrolStateR;
                    break;
                case ReturnMode.SpiderL:
                    resolvedState = _catchSpiderStateL;
                    break;
                case ReturnMode.SpiderR:
                    resolvedState = _catchSpiderStateR;
                    break;
                case ReturnMode.Hover:
                    resolvedState = _moveToHoverState;
                    break;
            }
        
            // Immediately switch to the resolved state if possible
        
            if (ReferenceEquals(_stateMachine.CurrentState, _attackHeadSuccessState) && _visibleBodyTransform.position.x < 0)
            {
                if (ReferenceEquals(resolvedState, _moveToPatrolStateR) || ReferenceEquals(resolvedState, _catchSpiderStateL))
                {
                    _stateMachine.SetState(resolvedState);
                
                }
                else
                {
                    _stateMachine.SetState(_returnTransitionLRTBState);
                }
            }
            else if (ReferenceEquals(_stateMachine.CurrentState, _attackHeadSuccessState) && _visibleBodyTransform.position.x > 0)
            {
                if (ReferenceEquals(resolvedState, _moveToPatrolStateL) || ReferenceEquals(resolvedState, _catchSpiderStateR))
                {
                    _stateMachine.SetState(resolvedState);
                }else
                {
                    _stateMachine.SetState(_returnTransitionRLTBState);
                }
            } 
            else if (ReferenceEquals(_stateMachine.CurrentState, _attackTailSuccessL))
            {
                if (ReferenceEquals(resolvedState, _moveToPatrolStateR) || 
                    ReferenceEquals(resolvedState, _catchSpiderStateL) ||
                    ReferenceEquals(resolvedState, _moveToHoverState))
                {
                    _stateMachine.SetState(resolvedState);
                }
                else
                {
                    _stateMachine.SetState(_returnTransitionLRTBState);
                }
            } 
            else if (ReferenceEquals(_stateMachine.CurrentState, _attackTailSuccessR))
            {
                if (ReferenceEquals(resolvedState, _moveToPatrolStateL) || 
                    ReferenceEquals(resolvedState, _catchSpiderStateR) ||
                    ReferenceEquals(resolvedState, _moveToHoverState))
                {
                    _stateMachine.SetState(resolvedState);
                }
                else
                {
                    _stateMachine.SetState(_returnTransitionRLTBState);
                }
            } 
            else if (ReferenceEquals(_stateMachine.CurrentState, _fallHeadState) ||
                     ReferenceEquals(_stateMachine.CurrentState, _returnHoverState) ||
                     ReferenceEquals(_stateMachine.CurrentState, _attackTailFailL) ||
                     ReferenceEquals(_stateMachine.CurrentState, _attackTailFailR))
            {
                if (ReferenceEquals(resolvedState, _moveToPatrolStateL) ||
                    ReferenceEquals(resolvedState, _moveToPatrolStateR) ||
                    ReferenceEquals(resolvedState, _moveToHoverState))
                {
                    _stateMachine.SetState(resolvedState);
                }
                else if (ReferenceEquals(resolvedState, _catchSpiderStateL) && _visibleBodyTransform.position.x < 0)
                {
                    _stateMachine.SetState(resolvedState);
                }
                else if (ReferenceEquals(resolvedState, _catchSpiderStateL) && _visibleBodyTransform.position.x > 0)
                {
                    _stateMachine.SetState(_returnTransitionRLBTState);
                }
                else if (ReferenceEquals(resolvedState, _catchSpiderStateR) && _visibleBodyTransform.position.x > 0)
                {
                    _stateMachine.SetState(resolvedState);
                }
                else if (ReferenceEquals(resolvedState, _catchSpiderStateR) && _visibleBodyTransform.position.x < 0)
                {
                    _stateMachine.SetState(_returnTransitionLRBTState);
                }
            }
#if  UNITY_EDITOR
            _currentStateType = resolvedState?.ToString().Replace("FDragonfly", ""); // DEBUG
#endif 
        }
    
        public void TriggerBounce()
        {
            _isBounced = true;
        }

        public void TriggerFall(AttackResult attackResult)
        {
            _attackResult = attackResult;
        }

        public void SetLampDestroyed()
        {
            _isLampDestroyed = true;
        }
    
        private void SetMovementStatesDependencies()
        { 
            _attackHeadState.SetDependencies(_visibleBodyTransform, transform);
            _attackHeadSuccessState.SetDependencies(_visibleBodyTransform, _fallPoint); 
            _attackHoverState.SetDependencies(_visibleBodyTransform, transform);
            _attackTailFailL.SetDependencies(_visibleBodyTransform, transform);
            _attackTailFailR.SetDependencies(_visibleBodyTransform, transform);
            _attackTailStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
            _attackTailStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
            _attackTailSuccessL.SetDependencies(_visibleBodyTransform, transform);
            _attackTailSuccessR.SetDependencies(_visibleBodyTransform, transform);
            _bounceHeadState.SetDependencies(_visibleBodyTransform, transform);
            _bounceHoverState.SetDependencies(_visibleBodyTransform, transform);
            _bounceTailStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
            _bounceTailStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
            _catchSpiderStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _catchSpiderLHash);
            _catchSpiderStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _catchSpiderRHash);
            _deathHeadState.SetDependencies(_visibleBodyTransform, _fallPoint);
            _deathTailStateL.SetDependencies(_visibleBodyTransform, _fallPoint);
            _deathTailStateR.SetDependencies(_visibleBodyTransform, _fallPoint);
            _enterToHoverStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _enterToHoverLHash);
            _enterToHoverStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _enterToHoverRHash);
            _enterToPatrolStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _enterToPatrolLHash);
            _enterToPatrolStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _enterToPatrolRHash);
            _fallHeadState.SetDependencies(_visibleBodyTransform, _fallPoint);
            _hoverState.SetDependencies(_visibleBodyTransform, transform);
            _gameoverHoverState.SetDependencies(_visibleBodyTransform, transform);
            _idleState.SetDependencies(_visibleBodyTransform, transform);
            _moveToHoverState.SetDependencies(_visibleBodyTransform, transform);
            _moveToPatrolStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _moveToPatrolLHash);
            _moveToPatrolStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _moveToPatrolRHash);
            _patrolStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
            _patrolStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
            _preAttackHeadStateL.SetDependencies(_visibleBodyTransform, transform);
            _preAttackHeadStateR.SetDependencies(_visibleBodyTransform, transform);
            _preAttackHoverState.SetDependencies(_visibleBodyTransform, transform);
            _preAttackTailStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
            _preAttackTailStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
            _returnHoverState.SetDependencies(_visibleBodyTransform, transform);
            _returnTransitionRLBTState.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _returnTransitionRLBTHash);
            _returnTransitionLRBTState.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _returnTransitionLRBTHash);
            _returnTransitionRLTBState.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _returnTransitionRLTBHash);
            _returnTransitionLRTBState.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _returnTransitionLRTBHash);
            _spiderPatrolStateL.SetDependencies(_visibleBodyTransform, _spiderPatrolTransform, _spiderPatrolRotator);
            _spiderPatrolStateR.SetDependencies(_visibleBodyTransform, _spiderPatrolTransform, _spiderPatrolRotator);
            _spiderPreAttackHeadTransitionStateL.SetDependencies(
                _visibleBodyTransform, _patrolTransform, _spiderPatrolTransform, _patrolRotator, _spiderPatrolRotator);
            _spiderPreAttackHeadTransitionStateR.SetDependencies(
                _visibleBodyTransform, _patrolTransform, _spiderPatrolTransform, _patrolRotator, _spiderPatrolRotator);
            _spiderPushStateL.SetDependencies(_visibleBodyTransform, _spiderPatrolTransform, _spiderPatrolRotator);
            _spiderPushStateR.SetDependencies(_visibleBodyTransform, _spiderPatrolTransform, _spiderPatrolRotator);
        }

        private void StateMachineSetup()
        {
            // Idle -> Enter States
            At(_idleState, _enterToPatrolStateL, () => _isPlaying && _enterState == EnterType.Patrol && _sideDirection == 1);
            At(_idleState, _enterToPatrolStateR, () => _isPlaying && _enterState == EnterType.Patrol && _sideDirection == -1);
            At(_idleState, _enterToHoverStateL, () => _isPlaying && _enterState == EnterType.Hover && _sideDirection == 1);
            At(_idleState, _enterToHoverStateR, () => _isPlaying && _enterState == EnterType.Hover && _sideDirection == -1);
        
            // Enter -> Patrol
            At(_enterToPatrolStateL, _patrolStateL, IsAnimationEnded());
            At(_enterToPatrolStateR, _patrolStateR, IsAnimationEnded());
            At(_moveToPatrolStateL, _patrolStateL, IsAnimationEnded());
            At(_moveToPatrolStateR, _patrolStateR, IsAnimationEnded());
        
        
            At(_enterToHoverStateL, _hoverState, IsAnimationEnded());
            At(_enterToHoverStateR, _hoverState, IsAnimationEnded());
            At(_moveToHoverState, _hoverState, () => _moveToHoverState.ReadyToSwitch);
        
            // Patrol -> PreAttack
            At(_patrolStateL, _preAttackHeadStateL,  IsStartHeadAttack());
            At(_patrolStateR, _preAttackHeadStateR,  IsStartHeadAttack());
            At(_patrolStateL, _preAttackTailStateL,  IsStartTailAttack());
            At(_patrolStateR, _preAttackTailStateR,  IsStartTailAttack());
        
            // PreAttack -> Attack
            At(_preAttackHeadStateL, _attackHeadState, ()  => _preAttackHeadStateL.ReadyToSwitch);
            At(_preAttackHeadStateR, _attackHeadState, ()  => _preAttackHeadStateR.ReadyToSwitch);
            At(_preAttackTailStateL, _attackTailStateL, () => _preAttackTailStateL.ReadyToSwitch);
            At(_preAttackTailStateR, _attackTailStateR, () => _preAttackTailStateR.ReadyToSwitch);
        
            // Hover -> PreAttack
            At(_hoverState, _preAttackHoverState, IsStartHeadAttack());
            // PreAttackHover -> Attack
            At(_preAttackHoverState, _attackHoverState, () => _preAttackHoverState.ReadyToSwitch);
        
            // Attack -> Bounce
            At(_attackHeadState, _bounceHeadState, IsBounced());
            At(_attackTailStateL, _bounceTailStateL, IsBounced());
            At(_attackTailStateR, _bounceTailStateR, IsBounced());
            At(_attackHoverState, _bounceHoverState, IsBounced());
        
            // Bounce -> Success
            At(_bounceHeadState, _attackHeadSuccessState, IsAttackSuccess());
            At(_bounceTailStateL, _attackTailSuccessL, IsAttackSuccess());
            At(_bounceTailStateR, _attackTailSuccessR, IsAttackSuccess());
            At(_bounceHoverState, _returnHoverState, IsAttackSuccess());
        
            // Bounce -> Fail
            At(_bounceHeadState, _fallHeadState, IsAttackFail());
            At(_bounceTailStateL, _attackTailFailL, IsAttackFail());
            At(_bounceTailStateR, _attackTailFailR, IsAttackFail());
            At(_bounceHoverState, _fallHeadState, IsAttackFail());
        
            // Bounce - Death
            At(_bounceHeadState, _deathHeadState, IsDied());
            At(_bounceTailStateL, _deathTailStateL, IsDied());
            At(_bounceTailStateR, _deathTailStateR, IsDied());
            At(_bounceHoverState, _deathHeadState, IsDied());
        
            // Spider
            At(_catchSpiderStateL, _spiderPatrolStateL, IsAnimationEnded());
            At(_catchSpiderStateR, _spiderPatrolStateR, IsAnimationEnded());
        
            At(_spiderPatrolStateL, _spiderPushStateL, IsStartSpiderAttack());
            At(_spiderPatrolStateR, _spiderPushStateR, IsStartSpiderAttack());
        
            At(_spiderPushStateL, _spiderPreAttackHeadTransitionStateL, () => _spiderPushStateL.ReadyToSwitch);
            At(_spiderPushStateR, _spiderPreAttackHeadTransitionStateR, () => _spiderPushStateR.ReadyToSwitch);
        
            // Spider to patrol
            At(_spiderPreAttackHeadTransitionStateL, _patrolStateL, () => _spiderPreAttackHeadTransitionStateL.ReadyToSwitch);
            At(_spiderPreAttackHeadTransitionStateR, _patrolStateR, () => _spiderPreAttackHeadTransitionStateR.ReadyToSwitch);
        
        
            // Return Resolve Transitions
        
            At(_returnTransitionLRTBState, _moveToPatrolStateL, IsResolvedToPatrolL());
            At(_returnTransitionLRTBState, _catchSpiderStateR, IsResolvedToCatchSpiderR());
            At(_returnTransitionLRTBState, _moveToHoverState, IsResolvedToHover());
        
            At(_returnTransitionRLTBState, _moveToPatrolStateR, IsResolvedToPatrolR());
            At(_returnTransitionRLTBState, _catchSpiderStateL, IsResolvedToCatchSpiderL());
            At(_returnTransitionRLTBState, _moveToHoverState, IsResolvedToHover());
        
            At(_returnTransitionLRBTState, _catchSpiderStateL, IsResolvedToCatchSpiderL());
            At(_returnTransitionLRBTState, _catchSpiderStateR, IsResolvedToCatchSpiderR());
            At(_returnTransitionRLBTState, _catchSpiderStateL, IsResolvedToCatchSpiderL());
            At(_returnTransitionRLBTState, _catchSpiderStateR, IsResolvedToCatchSpiderR());
        
            // _stateMachine.SetState(_idleState);
        
            // Transition helper methods
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            // void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
        
            // Transition Predicates
            #region Transition Predicate Delegates
        
            Func<bool> IsAnimationEnded() => () =>
            {
                if (_isAnimClipEnded)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsStartHeadAttack() => () =>
            {
                if (_isAttacking && _currentPatrolAttackMode == PatrolAttackMode.Head && !_isLampDestroyed)
                {
                    _isAttacking = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsStartTailAttack() => () =>
            {
                if (_isAttacking && _currentPatrolAttackMode == PatrolAttackMode.Tail && !_isLampDestroyed)
                {
                    _isAttacking = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsStartSpiderAttack() => () =>
            {
                if (_isAttacking && _currentPatrolAttackMode == PatrolAttackMode.Spider && !_isLampDestroyed)
                {
                    _isAttacking = false;
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
        
            Func<bool> IsAttackSuccess() => () =>
            {
                if (_attackResult == AttackResult.Success)
                {
                    _attackResult = AttackResult.None;
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
        
            Func<bool> IsDied() => () =>
            {
                if (_attackResult == AttackResult.Death)
                {
                    _attackResult = AttackResult.None;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsResolvedToPatrolL() => () =>
            {
                if (_isAnimClipEnded && _resolvedReturnMode == ReturnMode.PatrolL)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsResolvedToPatrolR() => () =>
            {
                if (_isAnimClipEnded && _resolvedReturnMode == ReturnMode.PatrolR)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsResolvedToCatchSpiderL() => () =>
            {
                if (_isAnimClipEnded && _resolvedReturnMode == ReturnMode.SpiderL)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsResolvedToCatchSpiderR() => () =>
            {
                if (_isAnimClipEnded && _resolvedReturnMode == ReturnMode.SpiderR)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsResolvedToHover() => () =>
            {
                if (_isAnimClipEnded && _resolvedReturnMode == ReturnMode.Hover)
                {
                    _isAnimClipEnded = false;
                    return true;
                }
                return false;
            };
        
            #endregion
        }

        private void Update()
        {
            if (_isPlaying)
            {
                _stateMachine.Tick();
                _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FDragonfly", ""); // DEBUG
            }
        }

        // Event Handle Methods
        private void OnAnimClipEnded()
        {
            _isAnimClipEnded = true;
        }

        private void OnSwarmCalled()
        {
            SwarmCalled?.Invoke();
        }

        private void OnReadyToHoverAttackEnter()
        {
            ReadyHoverToAttackStateEntered?.Invoke(_stateMachine.CurrentState);
        }

        private void OnReadyToSwarmAttackEnter()
        {
            ReadyToSwarmAttackStateEntered?.Invoke(_stateMachine.CurrentState);
        }

        private void OnReadyToSpiderAttack()
        {
            ReadyToSpiderAttackStateStarted?.Invoke();
        }

        private void OnCatchSpiderStartedL()
        {
            CatchSpiderStarted?.Invoke(1);
        }

        private void OnCatchSpiderStartedR()
        {
            CatchSpiderStarted?.Invoke(-1);
        }

        private void OnPreAttackStarted()
        {
            PreAttackStarted?.Invoke();
        }

        private void OnAttackStarted()
        {
            AttackStarted?.Invoke();
        }

        private void OnAttackEnded()
        {
            AttackEnded?.Invoke();
        }

        private void OnAfterAttackExitEnded()
        {
            AfterAttackExitEnded?.Invoke(_stateMachine.CurrentState);
        }

        private void OnDeathAnimationEnded()
        {
            DeathAnimationEnded?.Invoke();
        }
    
        private void OnCollisionPhaseReached()
        {
            CollisionPhaseReached?.Invoke();
        }
        
        // State Events
        private void OnAttackHeadStarted()
        {
            AttackHeadStarted?.Invoke();
        }
        private void OnAttackHeadSuccessStarted()
        {
            AttackHeadSuccessStarted?.Invoke();
        }

        private void OnAttackHoverStarted()
        {
            AttackHoverStarted?.Invoke();
        }

        private void OnAttackTailFailLStarted()
        {
            AttackTailFailLStarted?.Invoke();
        }

        private void OnAttackTailFailRStarted()
        {
            AttackTailFailRStarted?.Invoke();
        }

        private void OnAttackTailLStarted()
        {
            AttackTailLStarted?.Invoke();
        }

        private void OnAttackTailRStarted()
        {
            AttackTailRStarted?.Invoke();
        }

        private void OnAttackTailSuccessLStarted()
        {
            AttackTailSuccessLStarted?.Invoke();
        }

        private void OnAttackTailSuccessRStarted()
        {
            AttackTailSuccessRStarted?.Invoke();
        }

        private void OnBounceHeadStarted()
        {
            BounceHeadStarted?.Invoke();
        }

        private void OnBounceHoverStarted()
        {
            BounceHoverStarted?.Invoke();
        }

        private void OnBounceTailLStarted()
        {
            BounceTailLStarted?.Invoke();
        }

        private void OnBounceTailRStarted()
        {
            BounceTailRStarted?.Invoke();
        }

        private void OnCatchSpiderLStarted()
        {
            CatchSpiderLStarted?.Invoke();
        }

        private void OnCatchSpiderRStarted()
        {
            CatchSpiderRStarted?.Invoke();
        }

        private void OnDeathHeadStarted()
        {
            DeathHeadStarted?.Invoke();
        }

        private void OnDeathTailLStarted()
        {
            DeathTailLStarted?.Invoke();
        }

        private void OnDeathTailRStarted()
        {
            DeathTailRStarted?.Invoke();
        }

        private void OnEnterToHoverLStarted()
        {
            EnterToHoverLStarted?.Invoke();
        }

        private void OnEnterToHoverRStarted()
        {
            EnterToHoverRStarted?.Invoke();
        }

        private void OnEnterToPatrolLStarted()
        {
            EnterToPatrolLStarted?.Invoke();
        }

        private void OnEnterToPatrolRStarted()
        {
            EnterToPatrolRStarted?.Invoke();
        }

        private void OnFallHeadStarted()
        {
            FallHeadStarted?.Invoke();
        }

        private void OnHoverStarted()
        {
            HoverStarted?.Invoke();
        }

        private void OnGameoverHoverStarted()
        {
            GameoverHoverStarted?.Invoke();
        }

        private void OnMoveToHoverStarted()
        {
            MoveToHoverStarted?.Invoke();
        }

        private void OnMoveToPatrolLStarted()
        {
            MoveToPatrolLStarted?.Invoke();
        }

        private void OnMoveToPatrolRStarted()
        {
            MoveToPatrolRStarted?.Invoke();
        }

        private void OnPatrolLStarted()
        {
            PatrolLStarted?.Invoke();
        }

        private void OnPatrolRStarted()
        {
            PatrolRStarted?.Invoke();
        }

        private void OnPreAttackHeadLStarted()
        {
            PreAttackHeadLStarted?.Invoke();
        }

        private void OnPreAttackHeadRStarted()
        {
            PreAttackHeadRStarted?.Invoke();
        }

        private void OnPreAttackHoverStarted()
        {
            PreAttackHoverStarted?.Invoke();
        }

        private void OnPreAttackTailLStarted()
        {
            PreAttackTailLStarted?.Invoke();
        }

        private void OnPreAttackTailRStarted()
        {
            PreAttackTailRStarted?.Invoke();
        }

        private void OnReturnHoverStarted()
        {
            ReturnHoverStarted?.Invoke();
        }

        private void OnReturnTransitionLRBTStarted()
        {
            ReturnTransitionLRBTStarted?.Invoke();
        }

        private void OnReturnTransitionLRTBStarted()
        {
            ReturnTransitionLRTBStarted?.Invoke();
        }

        private void OnReturnTransitionRLBTStarted()
        {
            ReturnTransitionRLBTStarted?.Invoke();
        }

        private void OnReturnTransitionRLTBStarted()
        {
            ReturnTransitionRLTBStarted?.Invoke();
        }

        private void OnSpiderPatrolLStarted()
        {
            SpiderPatrolLStarted?.Invoke();
        }

        private void OnSpiderPatrolRStarted()
        {
            SpiderPatrolRStarted?.Invoke();
        }

        private void OnSpiderPreattackHeadTransitionLStarted()
        {
            SpiderPreattackHeadTransitionLStarted?.Invoke();
        }

        private void OnSpiderPreattackHeadTransitionRStarted()
        {
            SpiderPreattackHeadTransitionRStarted?.Invoke();
        }

        private void OnSpiderPushLStarted()
        {
            SpiderPushLStarted?.Invoke();
        }

        private void OnSpiderPushRStarted()
        {
            SpiderPushRStarted?.Invoke();
        }
    }
}
