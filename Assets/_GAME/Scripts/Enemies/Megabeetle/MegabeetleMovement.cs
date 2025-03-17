using System;
using _GAME.Scripts.Enemies.Ladybug.MovementStates;
using _GAME.Scripts.Enemies.Megabeetle.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Megabeetle
{
    public class MegabeetleMovement : EnemyMovementBase, IPositionDirectionProvider
    {
        
        [Header("-- Movement Settings --")]
        [SerializeField] private float _speed;
        [SerializeField] private float _radius;
        [SerializeField] private float _verticalAmplitude;
        [SerializeField] private bool _isSmoothDampEnabled;
        [SerializeField] private bool _isDepthEnabled;
        [SerializeField] private Vector3 IDLE_POSITION; // For Debug
        [SerializeField] private int _sideDirection; // For Debug
        [SerializeField] private string _stateDebug; // For Debug
        private float _collisionRadius;
        
        // State Machine
        private readonly StateMachine _stateMachine = new();
        private MegabeetleMovementStateFactory _stateFactory;
        private EnemyMovementStateBase _currentState;
    
        private FMegabeetleMovementEnterStateR _enterStateR;
        private FMegabeetleMovementEnterStateL _enterStateL;
        private FMegabeetleMovementPatrolStateR _patrolStateR;
        private FMegabeetleMovementPatrolStateL _patrolStateL;
        private LadybugMovementPreAttackStateR _preAttackStateR;
        private LadybugMovementPreAttackStateL _preAttackStateL;
        private LadybugMovementAttackState _attackState;
        private FMegabeetleMovementStickState _stickState;
        private FMegabeetleMovementStickPreAttackState _stickPreAttackState;
        private FMegabeetleMovementStickPreAttackPauseState _stickPreAttackPauseState;
        private FMegabeetleMovementStickAttackState _stickAttackState;
        private FMegabeetleMovementStickLandingState _stickLandingState;
        private FMegabeetleMovementFallState _fallState;
        private FMegabeetleMovementDeathState _deathState;
    
    
        // Debug only
        private Vector3 _prevPosition;
        private Vector3 _prevPosSmooth;
        private Vector3 _velocity = Vector3.zero;
    
        public void Construct(
            MegabeetleMovementStateFactory stateFactory) 
        {
            _stateFactory = stateFactory;
        }
    
        public event Action EnteredAttackRange;
        public event Action PreAttackStarted;
        public event Action PreAttackEnded;
        public event Action DeathStateEnded;
        public event Action AttackStarted;
        public event Action StickyAttackEnded;
        public event Action FallEnded;
    
        public Vector2 Position2D { get; private set; }
        public Vector3 DepthDirection { get; private set; }
    
    
        public override void Initialize()
        {
            _stateFactory.SetEnemyDependencies(
                this,
                _speed,
                _radius, 
                _verticalAmplitude,
                _collisionRadius);
        
            _enterStateR = (FMegabeetleMovementEnterStateR)_stateFactory.Create(typeof(FMegabeetleMovementEnterStateR));
            _enterStateL = (FMegabeetleMovementEnterStateL)_stateFactory.Create(typeof(FMegabeetleMovementEnterStateL));
            _patrolStateR = (FMegabeetleMovementPatrolStateR)_stateFactory.Create(typeof(FMegabeetleMovementPatrolStateR));
            _patrolStateL = (FMegabeetleMovementPatrolStateL)_stateFactory.Create(typeof(FMegabeetleMovementPatrolStateL));
            _preAttackStateR = (LadybugMovementPreAttackStateR)_stateFactory.Create(typeof(LadybugMovementPreAttackStateR));
            _preAttackStateL = (LadybugMovementPreAttackStateL)_stateFactory.Create(typeof(LadybugMovementPreAttackStateL));
            _attackState = (LadybugMovementAttackState)_stateFactory.Create(typeof(LadybugMovementAttackState));
            _stickState = (FMegabeetleMovementStickState)_stateFactory.Create(typeof(FMegabeetleMovementStickState));
            _stickPreAttackState = (FMegabeetleMovementStickPreAttackState)_stateFactory.Create(typeof(FMegabeetleMovementStickPreAttackState));
            _stickPreAttackPauseState = (FMegabeetleMovementStickPreAttackPauseState)_stateFactory.Create(typeof(FMegabeetleMovementStickPreAttackPauseState));
            _stickAttackState = (FMegabeetleMovementStickAttackState)_stateFactory.Create(typeof(FMegabeetleMovementStickAttackState));
            _stickLandingState = (FMegabeetleMovementStickLandingState)_stateFactory.Create(typeof(FMegabeetleMovementStickLandingState));
            _fallState = (FMegabeetleMovementFallState)_stateFactory.Create(typeof(FMegabeetleMovementFallState));
            _deathState = (FMegabeetleMovementDeathState)_stateFactory.Create(typeof(FMegabeetleMovementDeathState));
        
            _enterStateR.EnteredAttackRange += OnEnteredAttackRange;
            _enterStateL.EnteredAttackRange += OnEnteredAttackRange;
            _patrolStateR.EnteredAttackRange += OnEnteredAttackRange;
            _patrolStateL.EnteredAttackRange += OnEnteredAttackRange;
            _preAttackStateR.Started += OnPreAttackStarted;
            _preAttackStateR.Ended += OnPreAttackEnded;
            _stickPreAttackPauseState.Started += OnPreAttackStarted;
            _stickPreAttackPauseState.Ended += OnPreAttackEnded;
            _attackState.Started += OnAttackStarted;
            _deathState.Ended += OnDeathStateEnded;
            _stickAttackState.Ended += OnStickAttackEnded;
            _fallState.Ended += OnFallStateEnded;
            
            At(_enterStateR, _preAttackStateR, () => _enterStateR.IsReadyToSwitch);
            At(_enterStateL, _preAttackStateL, () => _enterStateL.IsReadyToSwitch);
            At(_patrolStateR, _preAttackStateR, () => _patrolStateR.IsReadyToSwitch);
            At(_patrolStateL, _preAttackStateL, () => _patrolStateL.IsReadyToSwitch);
        
            At(_preAttackStateR, _attackState, () => _preAttackStateR.IsReadyToSwitch);
            At(_preAttackStateL, _attackState, () => _preAttackStateL.IsReadyToSwitch);
        
            At(_stickLandingState, _stickState, () => _stickLandingState.IsReadyToSwitch);
            At(_stickState, _stickPreAttackState, () => _stickState.IsReadyToSwitch);
            At(_stickPreAttackState, _stickPreAttackPauseState, () => _stickPreAttackState.IsReadyToSwitch);
            At(_stickPreAttackPauseState, _stickAttackState, () => _stickPreAttackPauseState.IsReadyToSwitch);
            At(_stickAttackState, _stickState, () => _stickAttackState.IsReadyToSwitch);
        
        
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        }



        private void OnDisable()
        {
            _enterStateR.EnteredAttackRange -= OnEnteredAttackRange;
            _enterStateL.EnteredAttackRange -= OnEnteredAttackRange;
            _patrolStateR.EnteredAttackRange -= OnEnteredAttackRange;
            _patrolStateL.EnteredAttackRange -= OnEnteredAttackRange;
            _preAttackStateR.Started -= OnPreAttackStarted;
            _preAttackStateR.Ended -= OnPreAttackEnded;
            _stickPreAttackPauseState.Started -= OnPreAttackStarted;
            _stickPreAttackPauseState.Ended -= OnPreAttackEnded;
            _attackState.Started -= OnAttackStarted;
            _deathState.Ended -= OnDeathStateEnded;
            _stickAttackState.Ended += OnStickAttackEnded;
            _fallState.Ended -= OnFallStateEnded;
        }
        
        public void SetCollisionRadius(float radius)
        {
            _collisionRadius = radius;
        }
        
        public override void Play()
        {
            StartMovement(_enterStateR, _enterStateL);
        }

        private void StartMovement(EnemyMovementStateBase newStateR, EnemyMovementStateBase newtStateL)
        {
            _sideDirection = RandomDirection.Generate();
            SideDirection = _sideDirection;
            Position2D = GenerateSpawnPosition(_radius, _sideDirection);
        
            if (_sideDirection > 0)
            {
                _currentState = newStateR;
            }
            else
            {
                _currentState = newtStateL;
            }
            _stateMachine.SetState(_currentState);
        
            Position2D = _currentState.Position2D;
            transform.position = Position2D; // TODO: include distance to camera
        
            enabled = true;
        }

        public override void TriggerAttack()
        {
            throw new System.NotImplementedException();
        }

        public override void TriggerFall()
        {
            transform.parent = null;
            SwitchToStateAndApply(_fallState);
        }

        public override void TriggerDeath()
        {
            transform.parent = null;
            SwitchToStateAndApply(_deathState);
        }

        public void TriggerStick(Transform target)
        {
            transform.parent = target;
        
            _currentState = _stickLandingState;
            _stateMachine.SetState(_currentState); // Correct sticky position on enter

            // if (_isDepthEnabled)
            // {
            //     DepthDirection = _currentState.DepthDirection;
            //     transform.localPosition = (Vector3)Position2D + DepthDirection;
            // }
            // else
            // {
            //     transform.localPosition = _currentState.Position2D;
            // }
        
            transform.localPosition = _currentState.Position2D;
        }

        private void Update()
        {
            _prevPosition = transform.position;
        
            _stateMachine.Tick();
            _currentState = (EnemyMovementStateBase)_stateMachine.CurrentState;
            _stateDebug = _currentState.GetType().Name; // Debug only
            Position2D = _currentState.Position2D;
        
            if (_isDepthEnabled)
            {
                DepthDirection = _currentState.DepthDirection;
                transform.localPosition = (Vector3)Position2D + DepthDirection;
            }
            else
            {
                transform.localPosition = _currentState.Position2D;
            }
        
            Debug.DrawLine(_prevPosition, transform.position, Color.cyan, 10f);
        }
    
    

        private Vector2 GenerateSpawnPosition(float distance, int direction)
        {
            Vector3 spawnPosition = Vector3.zero;
            spawnPosition.x = distance;
            Quaternion rotation = Quaternion.Euler(0, 0, -Random.Range(22, 49));
            spawnPosition = rotation * spawnPosition;
            spawnPosition.x *= -direction;
            return spawnPosition;
        }

        private void SwitchToStateAndApply(EnemyMovementStateBase state) // TODO: implement this in all enemies
        {
            _currentState = state;
            _stateMachine.SetState(_currentState);
            Position2D = _currentState.Position2D;
            Vector2 newPosition = Position2D;
        
            transform.position = newPosition;
            _stateDebug = _currentState.GetType().Name;
        }
    
        private void OnFallStateEnded()
        {
            StartMovement(_patrolStateR, _patrolStateL);
            FallEnded?.Invoke();
        }

        private void OnEnteredAttackRange()
        {
            EnteredAttackRange?.Invoke();
        }

        private void OnPreAttackStarted()
        {
            PreAttackStarted?.Invoke();
        }

        private void OnPreAttackEnded()
        {
            PreAttackEnded?.Invoke();
        }

        private void OnDeathStateEnded()
        {
            DeathStateEnded?.Invoke();
        }

        private void OnAttackStarted()
        {
            AttackStarted?.Invoke();
        }
    
        private void OnStickAttackEnded()
        {
            StickyAttackEnded?.Invoke();
        }
    }
}
