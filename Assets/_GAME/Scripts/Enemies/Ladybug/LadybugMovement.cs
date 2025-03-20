using System;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.Ladybug.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Ladybug
{
    public class LadybugMovement : EnemyMovementBase, IPositionDirectionProvider, ISpreadableMovement
    {
        [Header("-- Movement Settings --")]
        
        [SerializeField] private float _speed;
        [SerializeField] private float _radius;
        [SerializeField] private float _verticalAmplitude;
        [SerializeField] private bool _isSmoothDampEnabled;
        [SerializeField] private bool _isDepthEnabled;
        // Debug
        [SerializeField] private string _stateDebug;
        [SerializeField] private int _sideDirection = 1;
        
        private float _collisionRadius;
        
        private Vector3 _position3D;
        // Debug only
        private Vector3 _prevPosition;
        private Vector3 _prevPosSmooth;
        private Vector3 _velocity = Vector3.zero;
    
    
        // State Machine
        private readonly StateMachine _stateMachine = new();
        private LadybugMovementStateFactory _stateFactory;
        private EnemyMovementStateBase _currentState;
    
        private LadybugMovementPatrolStateR _patrolStateR;
        private LadybugMovementPatrolStateL _patrolStateL;
        private LadybugMovementPreAttackStateR _preAttackStateR;
        private LadybugMovementPreAttackStateL _preAttackStateL;
        private LadybugMovementAttackState _attackState;
        private LadybugMovementStickState _stickState;
        private LadybugMovementDeathFallState _deathFallState;
        private FlyGenericMovementSpreadState _spreadState;
    
        public void Construct(
            LadybugMovementStateFactory stateFactory) 
        {
            _stateFactory = stateFactory;
        }
    
        public event Action PreAttackStarted;
        public event Action PreAttackEnded;
        public event Action DeathStateEnded;
        public event Action SpreadStateEnded;
        public event Action EnteredAttackRange;
    
        public Vector2 Position2D { get; private set; }
        public Vector3 DepthDirection { get; private set; }
    
        public override void Initialize()
        {        
            enabled = false;
            _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude, _collisionRadius);
            _patrolStateR = (LadybugMovementPatrolStateR)_stateFactory.Create(typeof(LadybugMovementPatrolStateR));
            _patrolStateL = (LadybugMovementPatrolStateL)_stateFactory.Create(typeof(LadybugMovementPatrolStateL));
            _preAttackStateR = (LadybugMovementPreAttackStateR)_stateFactory.Create(typeof(LadybugMovementPreAttackStateR));
            _preAttackStateL = (LadybugMovementPreAttackStateL)_stateFactory.Create(typeof(LadybugMovementPreAttackStateL));
            _attackState = (LadybugMovementAttackState)_stateFactory.Create(typeof(LadybugMovementAttackState));
            _stickState = (LadybugMovementStickState)_stateFactory.Create(typeof(LadybugMovementStickState));
            _deathFallState = (LadybugMovementDeathFallState)_stateFactory.Create(typeof(LadybugMovementDeathFallState));
            _spreadState = (FlyGenericMovementSpreadState)_stateFactory.Create(typeof(FlyGenericMovementSpreadState));
        
            _preAttackStateR.Started += OnPreAttackStateStarted;
            _preAttackStateR.Ended += OnPreAttackStateEnded;
            _preAttackStateL.Started += OnPreAttackStateStarted;
            _preAttackStateL.Ended += OnPreAttackStateEnded;
            _deathFallState.Ended += OnDeathFallStateEnded;
            _spreadState.Ended += OnSpreadStateEnded;
            _patrolStateR.EnteredAttackRange += OnEnteredAttackRange;
            _patrolStateL.EnteredAttackRange += OnEnteredAttackRange;
        
            At(_patrolStateR, _preAttackStateR, () => _patrolStateR.IsReadyToSwitch);
            At(_preAttackStateR, _attackState, () => _preAttackStateR.IsReadyToSwitch);
            At(_patrolStateL, _preAttackStateL, () => _patrolStateL.IsReadyToSwitch);
            At(_preAttackStateL, _attackState, () => _preAttackStateL.IsReadyToSwitch);
        
        
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        }

        private void OnDestroy()
        {
            _preAttackStateR.Started -= OnPreAttackStateStarted;
            _preAttackStateR.Ended -= OnPreAttackStateEnded;
            _preAttackStateL.Started -= OnPreAttackStateStarted;
            _preAttackStateL.Ended -= OnPreAttackStateEnded;
            _deathFallState.Ended -= OnDeathFallStateEnded;
            _spreadState.Ended -= OnSpreadStateEnded;
            _patrolStateR.EnteredAttackRange -= OnEnteredAttackRange;
            transform.parent = null;
        }
        
        public void SetCollisionRadius(float radius)
        {
            _collisionRadius = radius;
        }

        

        public override void Play()
        {
            // Spawn position
            _sideDirection = RandomDirection.Generate();
            SideDirection = _sideDirection;
            Position2D = GenerateSpawnPosition(_radius);
        
            if (_sideDirection > 0)
            {
                _currentState = _patrolStateR;
            }
            else
            {
                _currentState = _patrolStateL;
            }
        
            _stateMachine.SetState(_currentState);
        
            _position3D = _currentState.Position2D;
            transform.position = _position3D;
        
            enabled = true;

        }

        public override void TriggerAttack()
        {
            throw new System.NotImplementedException();
        }

        public override void TriggerFall()
        {
            transform.parent = null;
            SwitchToStateAndApply(_deathFallState);
            enabled = true;
        }

        public override void TriggerDeath()
        {
            transform.parent = null;
            SwitchToStateAndApply(_deathFallState);
            enabled = true;
        }

        public void TriggerSpread()
        {
            SwitchToStateAndApply(_spreadState);
        }

        private void Update()
        {
            _prevPosition = transform.position;
        
            _stateMachine.Tick();
            _currentState = (EnemyMovementStateBase)_stateMachine.CurrentState;
            _stateDebug = _currentState.GetType().Name; // Debug only
            Position2D = _currentState.Position2D;
        
        
            // Add Depth later
            if (_isDepthEnabled)
            {
                DepthDirection = _currentState.DepthDirection;
                transform.position = (Vector3)Position2D + DepthDirection;
            }
            else
            {
                transform.position = Position2D;
            }
        
            // Add Smooth?
        
            Debug.DrawLine(_prevPosition, transform.position, Color.cyan, 10f);
        }

        private Vector2 GenerateSpawnPosition(float distance)
        {
            Vector3 spawnPosition = Vector3.zero;
            spawnPosition.x = distance;
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            spawnPosition = rotation * spawnPosition;
            return spawnPosition;
        }

        private void SwitchToStateAndApply(EnemyMovementStateBase state)
        {
            _currentState = state;
            _stateMachine.SetState(_currentState);
            Position2D = _currentState.Position2D;
            Vector2 newPosition = Position2D;
        
            transform.position = newPosition;
            _stateDebug = _currentState.GetType().Name;
        }


        // Sticky specific stuff
        public void TriggerStick(Transform target)
        {
            _currentState = _stickState;
            _stateMachine.SetState(_currentState); // Correct sticky position on enter

            if (_isDepthEnabled)
            {
                DepthDirection = _currentState.DepthDirection;
                transform.position = (Vector3)Position2D + DepthDirection;
            }
            else
            {
                transform.position = _currentState.Position2D;
            }
        
            transform.parent = target;
            enabled = false;
        }

        private void OnPreAttackStateStarted()
        {
            PreAttackStarted?.Invoke();
        }

        private void OnPreAttackStateEnded()
        {
            PreAttackEnded?.Invoke();
        }

        private void OnDeathFallStateEnded()
        {
            DeathStateEnded?.Invoke();
        }

        private void OnSpreadStateEnded()
        {
            SpreadStateEnded?.Invoke();
            Play();
        }
        
        private void OnEnteredAttackRange()
        {
            EnteredAttackRange?.Invoke();
        }
    }
}
