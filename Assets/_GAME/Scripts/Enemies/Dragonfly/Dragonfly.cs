using System;
using _GAME.Scripts.Enemies.Dragonfly.BehaviourStates;
using _GAME.Scripts.Enemies.Dragonfly.FMovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Dragonfly
{
    public class Dragonfly : CollidableEnemy, IAnimatedEnemy, IProjectileShooter, IBoss, ILampDestroyedDependable
    {
        private readonly ReturnMode[] _returnModes =
        {
            ReturnMode.PatrolL,
            ReturnMode.PatrolR,
            ReturnMode.SpiderL,
            ReturnMode.SpiderR,
            ReturnMode.Hover,
            ReturnMode.Hover
        };
    
        [SerializeField] private string _stateDebug;
        [Header("-- Attributes --")]
        [SerializeField] private int _maxHealth = 24;
        [SerializeField] private int _currentHealth;
        [Header("-- Movement --")]
        [SerializeField] private DragonflyMovement _movement;
        [SerializeField] private Transform _visibleBodyTransform;
        [Header("-- Collision --")]
        [SerializeField] private DragonflyCollisionProvider _collisionProvider;
        [Header("Swarm")]
        [SerializeField] private DragonflySwarm _swarm;
        [SerializeField] private float _swarmAttackDuration;
        [Header("Spider")]
        [SerializeField] private DragonflyProjectileSpider.DragonflyProjectileSpider _spider;
        // Serialized for debug
        [SerializeField] private bool _isCollidedWithLamp;
        
        private DragonflyBehaviourStateFactory _stateFactory; 

        private Vector3 _patrolAttackPosition;
        private Vector3 _patrolSpiderAttackPosition;
        // STATE MACHINE 
        private readonly StateMachine _stateMachine = new StateMachine();
        private DragonflyInactiveState _inactiveState;
        private DragonflyPassiveState _passiveState;
        private DragonflyPatrolState _patrolState;
        private DragonflyHoverState _hoverState;
        private DragonflyPatrolHeadState _patrolHeadState;
        private DragonflyPatrolTailState _patrolTailState;
        private DragonflyWaitHeadAttackState _waitHeadAttackState;
        private DragonflyWaitTailAttackState _waitTailAttackState;
        private DragonflyWaitHoverAttackState _waitHoverAttackState;
        private DragonflySpiderEnterState _spiderEnterState;
        private DragonflyPatrolSpiderState _patrolSpiderState;
        private DragonflyWaitSpiderAttackState _waitSpiderAttackState;
        private DragonflySwarmAttackState _swarmAttackState;
        private DragonflyWaitForBounceState _waitForBounceState;
        // Parameters
        [SerializeField] private EnterType _enterType;
        [SerializeField] private PatrolAttackMode _patrolAttackMode;
        [SerializeField] private PostSpiderAttackMode _postSpiderAttackMode;
        private bool _isReadyToHoverAttackWait;
        private bool _isReadyToSpiderAttackWait;
        private bool _isDead;
        private bool _isLampDestroyed;
        private IState _movementState;
        private ReturnMode _returnMode;

        public event Action Started;
        public event Action<CollidableEnemy> AnimatedAttackStarted;
        public event Action<CollidableEnemy> ProjectileShot;
        public event Action<Enemy, bool> ProjectileDeactivated;
        public event Action SpreadRequested;
        public event Action Damaged;
        public event Action<int, int> HealthChanged;
        public event Action Died;
        public event Action SwarmCalled;
        public event Action<Transform> ColliderTransformChanged; 
        
        public override Vector2 Position => _collisionProvider.CurrentCollisionPoint;
        public Transform MovementTransform => _visibleBodyTransform;

        public void Construct(DragonflyBehaviourStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }
        
        public override void Initialize()
        {
            _stateFactory.SetEnemyDependencies(
                _visibleBodyTransform,
                _movement,
                _swarmAttackDuration
                );
        
            CreateStates();
            CreateStateTransitions();
            
            _enterType = EnterType.None;
            _stateMachine.SetState(_inactiveState);
            
            _swarm.SetDuration(_swarmAttackDuration);
            _spider.Initialize();
            _swarm.Initialize();
            _movement.Initialize();

            enabled = false;
            _isDead = false;
            
            _patrolState.Started += OnPatrolStateStarted;
            _hoverState.Started += OnHoverStateStarted;
            _patrolHeadState.Started += OnPatrolHeadStateStarted;
            _patrolTailState.Started += OnPatrolTailStateStarted;
            _swarmAttackState.Started += OnSwarmAttackStateStarted;
            _waitSpiderAttackState.GotReadyToPreAttack += StartSpiderPreAttack;
            _waitSpiderAttackState.Ended += StartSpiderAttack;
        
            _movement.ReadyHoverToAttackStateEntered += OnReadyHoverToAttackStateEntered;
            _movement.ReadyToSwarmAttackStateEntered += OnReadyToSwarmAttackStateEntered;
            _movement.ReadyToSpiderAttackStateStarted += OnReadyToSpiderAttackStateStarted;
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.AttackStarted += OnAttackStarted;
            _movement.AfterAttackExitEnded += OnAfterAttackExitEnded;
            _movement.CatchSpiderStarted += OnCatchSpiderStarted;
            _movement.DeathAnimationEnded += OnDeathAnimationEnded;
            _movement.CollisionPhaseReached += OnCollisionPhaseReached;
            _movement.SwarmCalled += OnSwarmCalled;
            
            _spider.EnterAnimationEnded += OnSpiderEnterAnimationEnded;
            _swarm.MothAttackStarted += OnMothAttackStarted;
            _spider.Deactivated += OnSpiderDeactivated;
            _swarm.MothDeactivated += OnMothDeactivated;
        }

        private void OnDestroy()
        {
            _patrolState.Started -= OnPatrolStateStarted;
            _hoverState.Started -= OnHoverStateStarted;
            _patrolHeadState.Started -= OnPatrolHeadStateStarted;
            _patrolTailState.Started -= OnPatrolTailStateStarted;
            _swarmAttackState.Started -= OnSwarmAttackStateStarted;
            _waitSpiderAttackState.GotReadyToPreAttack -= StartSpiderPreAttack;
            _waitSpiderAttackState.Ended -= StartSpiderAttack;
        
            _movement.ReadyHoverToAttackStateEntered -= OnReadyHoverToAttackStateEntered;
            _movement.ReadyToSwarmAttackStateEntered -= OnReadyToSwarmAttackStateEntered;
            _movement.ReadyToSpiderAttackStateStarted -= OnReadyToSpiderAttackStateStarted;
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.AttackStarted -= OnAttackStarted;
            _movement.AfterAttackExitEnded -= OnAfterAttackExitEnded;
            _movement.CatchSpiderStarted -= OnCatchSpiderStarted;
            _movement.DeathAnimationEnded -= OnDeathAnimationEnded;
            _movement.CollisionPhaseReached -= OnCollisionPhaseReached;
            _movement.SwarmCalled -= OnSwarmCalled;

            _spider.EnterAnimationEnded -= OnSpiderEnterAnimationEnded;
            _swarm.MothAttackStarted -= OnMothAttackStarted;
            _spider.Deactivated -= OnSpiderDeactivated;
            _swarm.MothDeactivated -= OnMothDeactivated;
        }

        public override void Play()
        {
            enabled = true;
            
            _enterType = EnterType.None;
            _patrolAttackMode = PatrolAttackMode.None;
            _returnMode = ReturnMode.None;
            _postSpiderAttackMode = PostSpiderAttackMode.None;
            _stateMachine.SetState(_inactiveState);
            
            _isDead = false;
            _isReadyToHoverAttackWait = false;
            _isReadyToSpiderAttackWait = false;
            IsGameOver = false;
            _isLampDestroyed = false;
            
            _spider.gameObject.transform.SetParent(transform);
            _spider.gameObject.SetActive(false);
            _spider.Reset();
            _swarm.Reset();
            
            _currentHealth = _maxHealth;

            HealthChanged?.Invoke(_currentHealth, _maxHealth);

            _enterType = (EnterType)Random.Range(0, 2);
            int sideDirection = RandomDirection.Generate();

            _movement.Play(_enterType, sideDirection);
            Started?.Invoke();
        }

        public override void ReceiveDamage(int damageAmount)
        {
            IsReadyForDamage = false;
            _currentHealth -= damageAmount;
            IsReceivedLampAttackDamage = true;

            if (_currentHealth > 0)
            {
                _movement.TriggerFall(true);
                Damaged?.Invoke();
                HealthChanged?.Invoke(_currentHealth, _maxHealth);
                ColliderTransformChanged?.Invoke(_collisionProvider.CurrentCollisionTransform);
            }
            else
            {
                if (!_isDead)
                {
                    _currentHealth = 0; 
                    _movement.TriggerDeath(); 
                    Died?.Invoke();
                    _isDead = true;
                }
            }
        }

        public override void Attack()
        {
            throw new NotImplementedException();
        }

        public override void DoDeath()
        {
            throw new NotImplementedException();
        }

        public override void HandleCollision()
        {
            _movement.TriggerBounce();
            _isCollidedWithLamp = true;
            CollisionState = CollidableState.AfterCollision;
        }

        public override Vector3 ProvideImpactPoint()
        {
            return _collisionProvider.CurrentCollisionPoint;
        }

        public override void HandleEnterAttackZone()
        {
            CollisionState = CollidableState.InAttackZone;
            IsReadyForDamage = true;
        }

        public override void HandleExitAttackZone()
        {
            CollisionState = CollidableState.Outside;
            IsReadyForDamage = false;
            _movement.TriggerFall(false);
        }

        public void HandleLampDestroyed()
        {
            _movement.SetLampDestroyed();
            _swarm.TriggerGameover();
            _isLampDestroyed = true;
        }
        
        private void CreateStates()
        {
            _inactiveState = (DragonflyInactiveState)_stateFactory.Create(typeof(DragonflyInactiveState));
            _passiveState = (DragonflyPassiveState)_stateFactory.Create(typeof(DragonflyPassiveState));
            _patrolState = (DragonflyPatrolState)_stateFactory.Create(typeof(DragonflyPatrolState));
            _hoverState = (DragonflyHoverState)_stateFactory.Create(typeof(DragonflyHoverState));
            _patrolHeadState = (DragonflyPatrolHeadState)_stateFactory.Create(typeof(DragonflyPatrolHeadState));
            _patrolTailState = (DragonflyPatrolTailState)_stateFactory.Create(typeof(DragonflyPatrolTailState));
            _waitHeadAttackState = (DragonflyWaitHeadAttackState)_stateFactory.Create(typeof(DragonflyWaitHeadAttackState));
            _waitTailAttackState = (DragonflyWaitTailAttackState)_stateFactory.Create(typeof(DragonflyWaitTailAttackState));
            _waitHoverAttackState = (DragonflyWaitHoverAttackState)_stateFactory.Create(typeof(DragonflyWaitHoverAttackState));
            _waitSpiderAttackState = (DragonflyWaitSpiderAttackState)_stateFactory.Create(typeof(DragonflyWaitSpiderAttackState));
            _spiderEnterState = (DragonflySpiderEnterState)_stateFactory.Create(typeof(DragonflySpiderEnterState));
            _patrolSpiderState = (DragonflyPatrolSpiderState)_stateFactory.Create(typeof(DragonflyPatrolSpiderState));
            _swarmAttackState = (DragonflySwarmAttackState)_stateFactory.Create(typeof(DragonflySwarmAttackState));
            _waitForBounceState = (DragonflyWaitForBounceState)_stateFactory.Create(typeof(DragonflyWaitForBounceState));
        }

        private void CreateStateTransitions()
        {
            // Enter
            At(_inactiveState, _patrolState, () => _enterType == EnterType.Patrol);
            At(_inactiveState, _hoverState, () => _enterType == EnterType.Hover);
            // Patrol to Head/Tail attack through the swarm attack state
            At(_patrolState, _swarmAttackState, () => _patrolAttackMode != PatrolAttackMode.None);
            At(_swarmAttackState, _patrolHeadState, 
                () => 
                    _swarmAttackState.ReadyToSwitch 
                    && _patrolAttackMode == PatrolAttackMode.Head);
            At(_swarmAttackState, _patrolTailState, 
                () => 
                    _swarmAttackState.ReadyToSwitch
                    && _patrolAttackMode == PatrolAttackMode.Tail);
            At(_patrolHeadState, _waitHeadAttackState, () => _patrolHeadState.IsReadyToSwitch);
            At(_patrolTailState, _waitTailAttackState, () => _patrolTailState.IsReadyToSwitch);
            // Hover to Attack        
            At(_hoverState, _waitHoverAttackState, IsReadyToHoverAttackWait());
            // Exit from attacks to passive state
            At(_waitHeadAttackState, _waitForBounceState, () => _waitHeadAttackState.IsReadyToSwitch);
            At(_waitTailAttackState, _waitForBounceState, () => _waitTailAttackState.IsReadyToSwitch);
            At(_waitHoverAttackState, _waitForBounceState, () => _waitHoverAttackState.IsReadyToSwitch);
        
            At(_waitForBounceState, _passiveState, IsCollidedWithLamp());
        
            // Return to patrol/hover
            At(_passiveState, _patrolState, IsReturnToPatrol());
            At(_passiveState, _hoverState, IsReturnToHover());
            At(_passiveState, _spiderEnterState, IsReturnToSpider());
            // Spider Attack
            At(_spiderEnterState, _patrolSpiderState, IsReadyToSpiderAttackWait());
            At(_patrolSpiderState, _waitSpiderAttackState, () => _patrolSpiderState.ReadyToSwitch);
            // Post spider modes           
            At(_waitSpiderAttackState, _patrolState, IsPostSpiderPatrol());
            At(_waitSpiderAttackState, _patrolHeadState, IsPostSpiderHead());
        
        
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

            Func<bool> IsCollidedWithLamp() => () =>
            {
                if (_isCollidedWithLamp)
                {
                    _isCollidedWithLamp = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsReadyToHoverAttackWait() => () =>
            {
                if (_isReadyToHoverAttackWait)
                {
                    _isReadyToHoverAttackWait = false;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsReadyToSpiderAttackWait() => () =>
            {
                if (_isReadyToSpiderAttackWait)
                {
                    _isReadyToSpiderAttackWait = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsReturnToPatrol() => () =>
            {
                if (_returnMode == ReturnMode.PatrolL || _returnMode == ReturnMode.PatrolR)
                {
                    _returnMode = ReturnMode.None;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsReturnToHover() => () =>
            {
                if (_returnMode == ReturnMode.Hover)
                {
                    _returnMode = ReturnMode.None;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsReturnToSpider() => () =>
            {
                if (_returnMode == ReturnMode.SpiderR || _returnMode == ReturnMode.SpiderL)
                {
                    _returnMode = ReturnMode.None;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsPostSpiderPatrol() => () =>
            {
                if (_postSpiderAttackMode == PostSpiderAttackMode.Patrol)
                {
                    _postSpiderAttackMode = PostSpiderAttackMode.None;
                    return true;
                }
                return false;
            };
            
            Func<bool> IsPostSpiderHead() => () =>
            {
                if (_postSpiderAttackMode == PostSpiderAttackMode.Head)
                {
                    _postSpiderAttackMode = PostSpiderAttackMode.None;
                    return true;
                }
                return false;
            };
        }

        private void Update()
        {
            _stateMachine.Tick();
            _stateDebug = _stateMachine.CurrentState.ToString();
        }

        // Event Handle Methods
        private void OnHoverStateStarted()
        {
            _enterType = EnterType.None;
        }

        private void OnPatrolStateStarted()
        {
            _enterType = EnterType.None;
            _patrolAttackMode = PatrolAttackMode.None;
        }

        private void OnPatrolHeadStateStarted()
        {
            _patrolAttackMode = PatrolAttackMode.None;
        }

        private void OnPatrolTailStateStarted()
        {
            _patrolAttackMode = PatrolAttackMode.None;
        }

        private void OnSwarmAttackStateStarted()
        {
            if (!_isLampDestroyed)
            {
                if (_movementState.GetType() == typeof(FDragonflyPatrolStateL))
                {
                    _swarm.PlayAttack(1);
                }
                else if (_movementState.GetType() == typeof(FDragonflyPatrolStateR))
                {
                    _swarm.PlayAttack(-1);
                }    
            }
        }

        private void StartSpiderPreAttack()
        {
            if (!_isLampDestroyed)
            {
                _spider.StartPreAttack();
            }
        }

        private void StartSpiderAttack()
        {
            if (!_isLampDestroyed)
            {
                _spider.gameObject.transform.SetParent(transform);
                _spider.Attack();
                ProjectileShot?.Invoke(_spider);
                _movement.StartAttack(PatrolAttackMode.Spider);
                _postSpiderAttackMode = (PostSpiderAttackMode)Random.Range(0, 2);
            }
        }

        private void OnReadyHoverToAttackStateEntered(IState movementState)
        {
            _isReadyToHoverAttackWait = true;
        }

        private void OnReadyToSwarmAttackStateEntered(IState movementState)
        {
            _patrolAttackMode = (PatrolAttackMode)Random.Range(0, 2);
            _movementState = movementState;
        }

        private void OnReadyToSpiderAttackStateStarted()
        {
            _isReadyToSpiderAttackWait = true;
        }

        private void OnPreAttackStarted()
        {
            IsReceivedLampAttackDamage = false;
        }

        private void OnAttackStarted()
        {
            AnimatedAttackStarted?.Invoke(this);
        }

        private void OnAfterAttackExitEnded(IState movementState)
        {
            _returnMode = _returnModes[Random.Range(0, 6)];
            _movement.ResolveReturnTransition(_returnMode);
        }

        private void OnCatchSpiderStarted(int direction)
        {
            _spider.gameObject.SetActive(true);
            _spider.SetDirection(direction);
            _spider.Play();
        }

        private void OnDeathAnimationEnded()
        {
            OnDeathStateEnded();
            gameObject.SetActive(false);
            enabled = false;
        
        }

        private void OnCollisionPhaseReached()
        {
            _collisionProvider.FindClosestPointIndex();
            Radius = _collisionProvider.CurrentCollisionRadius;
        }

        private void OnSpiderEnterAnimationEnded()
        {
            _spider.gameObject.transform.SetParent(_visibleBodyTransform);
            Vector3 pos = Vector3.zero;
            pos.x = 0.012f;
            pos.y = -0.286f;
            pos.z = 0.082f;
            _spider.gameObject.transform.localPosition = pos;
        }

        private void OnSwarmCalled()
        {
            if (_stateMachine.CurrentState.GetType() != typeof(DragonflyPatrolHeadState)
                &&_stateMachine.CurrentState.GetType() != typeof(DragonflyWaitHeadAttackState))
            {
                SwarmCalled?.Invoke();    
            }
        }

        private void OnMothAttackStarted(CollidableEnemy enemy)
        {
            ProjectileShot?.Invoke(enemy);
        }

        private void OnSpiderDeactivated(Enemy enemy, bool damaged)
        {
            ProjectileDeactivated?.Invoke(enemy, damaged);
        }

        private void OnMothDeactivated(Enemy enemy, bool damaged)
        {
            ProjectileDeactivated?.Invoke(enemy, damaged);
        }
    }
}
