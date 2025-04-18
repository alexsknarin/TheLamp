using System;
using _GAME.Scripts.Enemies.Dragonfly.BehaviourStates;
using _GAME.Scripts.Enemies.Dragonfly.FMovementStates;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Dragonfly
{
    public class Dragonfly : CollidableEnemy, IAnimatedEnemy, IProjectileShooter, IBoss, ILampDestroyedDependable
    {
        private readonly DragonflyReturnMode[] _returnModes = new DragonflyReturnMode[]
        {
            DragonflyReturnMode.PatrolL,
            DragonflyReturnMode.PatrolR,
            DragonflyReturnMode.SpiderL,
            DragonflyReturnMode.SpiderR,
            DragonflyReturnMode.Hover,
            DragonflyReturnMode.Hover
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
        [Header("Hover")]
        [SerializeField] private float _hoverWaitMin;
        [SerializeField] private float _hoverWaitMax;
        [Header("Patrol")]
        [Header("Head")]
        [SerializeField] private DragonflySwarm _swarm;
        [SerializeField] private float _swarmAttackDuration;
        [SerializeField] private float _patrolWaitMin;
        [SerializeField] private float _patrolWaitMax;
        [SerializeField] private DragonflyPatrolAttackZoneRanges _patrolAttackZonesL;
        [SerializeField] private DragonflyPatrolAttackZoneRanges _patrolAttackZonesR;
        [Header("Tail")]
        [SerializeField] private Vector3 _tailAttackPositionBase;
        [SerializeField] private float _patrolTailWaitMin;
        [SerializeField] private float _patrolTailWaitMax;
        [Header("Spider")]
        [SerializeField] private Vector3 _spiderAttackPositionBase;
        [SerializeField] private float _spiderPatrolWaitMin;
        [SerializeField] private float _spiderPatrolWaitMax;
        [SerializeField] private DragonflyProjectileSpider.DragonflyProjectileSpider _spider;
        // Serialized for debug
        [SerializeField] private bool _isInAttackExitZone = false;
        [SerializeField] private bool _isCollidedWithLamp = false;
    
        private DragonflyPatrolAttackPositionProvider _patrolAttackPositionProvider;
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
        private bool _isActivated = false;
        private DragonflyEnterType _enterType = 0;
        private DragonflyPatrolAttackMode _patrolAttackMode = DragonflyPatrolAttackMode.Head;
        private bool _isReadyToPreAttackWait = false;
        private bool _isReadyToAttackWait = false;
        [SerializeField] private bool _isAttacked = false;
        private bool _isDead = false;
        private bool _isLampDestroyed = false;
        private DragonflyReturnMode _returnMode;

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
        
        public override void Initialize()
        {
            _patrolAttackPositionProvider = new DragonflyPatrolAttackPositionProvider(
                _patrolAttackZonesL, 
                _patrolAttackZonesR, 
                _tailAttackPositionBase
            );
        
            CreateStates();
            CreateStateTransitions();
        
            enabled = false;
            _isDead = false;
            _isActivated = false;
            _isReadyToPreAttackWait = false;
            _isReadyToAttackWait = false;
            _isAttacked = false;
            _spider.Initialize();
            _swarm.Initialize();
            _swarm.SetDuration(_swarmAttackDuration);
            _movement.Initialize();
        
            _patrolHeadState.Ended += GenerateAttackPosition;
            _patrolTailState.Ended += GenerateAttackPosition;
            _patrolSpiderState.Ended += GenerateAttackPosition;
            _waitHeadAttackState.Ended += StartAttack;
            _waitTailAttackState.Ended += StartAttack;
            _waitHoverAttackState.Ended += StartAttack;
            _waitSpiderAttackState.GotReadyToPreAttack += StartSpiderPreAttack;
            _waitSpiderAttackState.Ended += StartSpiderAttack;
        
            _movement.ReadyToAttackStateEntered += OnReadyToAttackStateEntered;
            _movement.ReadyToSwarmAttackStateEntered += OnReadyToSwarmAttackStateEntered;
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.AttackStarted += OnAttackStarted;
            _movement.AttackEnded += OnAttackEnded;
            _movement.SwarmCalled += OnSwarmCalled;
            _movement.AfterAttackExitEnded += OnAfterAttackExitEnded;
            _movement.ReadyToSpiderAttackStateStarted += OnReadyToSpiderAttackStateStarted;
            _movement.CatchSpiderStarted += OnCatchSpiderStarted;
            _movement.DeathAnimationEnded += OnDeathAnimationEnded;
            _movement.CollisionPhaseReached += OnCollisionPhaseReached;
            
            _spider.EnterAnimationEnded += OnSpiderEnterAnimationEnded;
            _waitForBounceState.Ended += OnWaitForBounceStateEnded;

            _swarm.MothAttackStarted += OnMothAttackStarted;
            _spider.Deactivated += OnProjectileDeactivated;
            _swarm.MothDeactivated += OnProjectileDeactivated;
        
        }
    
        private void OnDestroy()
        {
            _patrolHeadState.Ended -= GenerateAttackPosition;
            _patrolTailState.Ended -= GenerateAttackPosition;
            _patrolSpiderState.Ended -= GenerateAttackPosition;
            _waitHeadAttackState.Ended -= StartAttack;
            _waitTailAttackState.Ended -= StartAttack;
            _waitHoverAttackState.Ended -= StartAttack;
            _waitSpiderAttackState.GotReadyToPreAttack -= StartSpiderPreAttack;
            _waitSpiderAttackState.Ended -= StartSpiderAttack;
        
            _movement.ReadyToAttackStateEntered -= OnReadyToAttackStateEntered;
            _movement.ReadyToSwarmAttackStateEntered -= OnReadyToSwarmAttackStateEntered;
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.AttackStarted -= OnAttackStarted;
            _movement.AttackEnded -= OnAttackEnded;
            _movement.SwarmCalled -= OnSwarmCalled;
            _movement.AfterAttackExitEnded -= OnAfterAttackExitEnded;
            _movement.ReadyToSpiderAttackStateStarted -= OnReadyToSpiderAttackStateStarted;
            _movement.CatchSpiderStarted -= OnCatchSpiderStarted;
            _movement.DeathAnimationEnded -= OnDeathAnimationEnded;
            _movement.CollisionPhaseReached -= OnCollisionPhaseReached;
            _waitForBounceState.Ended -= OnWaitForBounceStateEnded;
            _spider.EnterAnimationEnded -= OnSpiderEnterAnimationEnded;


            _swarm.MothAttackStarted -= OnMothAttackStarted;
            _spider.Deactivated -= OnProjectileDeactivated;
            _swarm.MothDeactivated -= OnProjectileDeactivated;
        }


        public override void Play()
        {
            IsGameOver = false;
            enabled = true;
            _isLampDestroyed = false;
            _currentHealth = _maxHealth;
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
            _enterType = (DragonflyEnterType)Random.Range(0, 2);
            int sideDirection = RandomDirection.Generate();
            
            _spider.gameObject.transform.SetParent(transform);
            _spider.gameObject.SetActive(false);
            _spider.Initialize();
            _swarm.Initialize();
            
            _movement.Play(_enterType, sideDirection);
            _isActivated = true;
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
            _isInAttackExitZone = true;
        }

        public override void HandleExitAttackZone()
        {
            _isInAttackExitZone = false;
            CollisionState = CollidableState.Outside;
            IsReadyForDamage = false;
            _isCollidedWithLamp = false;
            _movement.TriggerFall(false);
        }

        private void Update()
        {
            _stateMachine.Tick();
            _stateDebug = _stateMachine.CurrentState.ToString();
        }

        private void CreateStates()
        {
            // Initialize the states
            _inactiveState = new DragonflyInactiveState();
            _passiveState = new DragonflyPassiveState();
            _patrolState = new DragonflyPatrolState();
            _hoverState = new DragonflyHoverState();
            _patrolHeadState = new DragonflyPatrolHeadState(_patrolWaitMin, _patrolWaitMax);
            _patrolTailState = new DragonflyPatrolTailState(_patrolTailWaitMin, _patrolTailWaitMax);
            _waitHeadAttackState = new DragonflyWaitHeadAttackState(_visibleBodyTransform, _patrolAttackPositionProvider, _movement);
            _waitTailAttackState = new DragonflyWaitTailAttackState(_visibleBodyTransform, _patrolAttackPositionProvider, _movement);
            _waitHoverAttackState = new DragonflyWaitHoverAttackState(_hoverWaitMin, _hoverWaitMax);
            _spiderEnterState = new DragonflySpiderEnterState();
            _patrolSpiderState = new DragonflyPatrolSpiderState(_spiderPatrolWaitMin, _spiderPatrolWaitMax);
            _waitSpiderAttackState = new DragonflyWaitSpiderAttackState(_visibleBodyTransform, _spiderAttackPositionBase);
            _swarmAttackState = new DragonflySwarmAttackState(_swarmAttackDuration);
            _waitForBounceState = new DragonflyWaitForBounceState();
        }

        private void CreateStateTransitions()
        {
            // Enter
            At(_inactiveState, _patrolState, () => _isActivated && _enterType == DragonflyEnterType.Patrol);
            At(_inactiveState, _hoverState, () => _isActivated && _enterType == DragonflyEnterType.Hover);
            // Patrol to Head/Tail attack through the swarm attack state
            At(_patrolState, _swarmAttackState, IsReadyToPatrolHead());
            At(_patrolState, _swarmAttackState, IsReadyToPatrolTail());
            At(_swarmAttackState, _patrolHeadState, () => _swarmAttackState.ReadyToSwitch 
                                                          && _patrolAttackMode == DragonflyPatrolAttackMode.Head);
            At(_swarmAttackState, _patrolTailState, () => _swarmAttackState.ReadyToSwitch 
                                                          && _patrolAttackMode == DragonflyPatrolAttackMode.Tail);
            At(_patrolHeadState, _waitHeadAttackState, IsReadyToAttackWait());
            At(_patrolTailState, _waitTailAttackState, IsReadyToAttackWait());
            // Hover to Attack        
            At(_hoverState, _waitHoverAttackState, IsReadyToPreAttackWait());
            // Exit from attacks to passive state
            At(_waitHeadAttackState, _waitForBounceState, IsAttacked());
            At(_waitTailAttackState, _waitForBounceState, IsAttacked());
            At(_waitHoverAttackState, _waitForBounceState, IsAttacked());
        
            At(_waitForBounceState, _passiveState, () => _isInAttackExitZone && _isCollidedWithLamp);
        
            // Return to patrol/hover
            At(_passiveState, _patrolState, IsReturnToPatrol());
            At(_passiveState, _hoverState, IsReturnToHover());
            At(_passiveState, _spiderEnterState, IsReturnToSpider());
            // Spider Attack
            At(_spiderEnterState, _patrolSpiderState, IsReadyToAttackWait());
            At(_patrolSpiderState, _waitSpiderAttackState, IsReadyToAttackWait());
            At(_waitSpiderAttackState, _patrolState, IsAttacked());

            _stateMachine.SetState(_inactiveState);
            _isActivated = false;
        
        
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        
        
            Func<bool> IsReadyToPatrolHead() => () =>
            {
                if (_isReadyToPreAttackWait && _patrolAttackMode == DragonflyPatrolAttackMode.Head)
                {
                    _isReadyToPreAttackWait = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsReadyToPatrolTail() => () =>
            {
                if (_isReadyToPreAttackWait && _patrolAttackMode == DragonflyPatrolAttackMode.Tail)
                {
                    _isReadyToPreAttackWait = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsReadyToPreAttackWait() => () =>
            {
                if (_isReadyToPreAttackWait)
                {
                    _isReadyToPreAttackWait = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsReadyToAttackWait() => () =>
            {
                if(_isReadyToAttackWait)
                {
                    _isReadyToAttackWait = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsAttacked() => () =>
            {
                if(_isAttacked)
                {
                    _isAttacked = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsReturnToPatrol() => () =>
            {
                if (_isReadyToPreAttackWait && (_returnMode == DragonflyReturnMode.PatrolL || _returnMode == DragonflyReturnMode.PatrolR))
                {
                    _isReadyToPreAttackWait = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsReturnToHover() => () =>
            {
                if (_isReadyToPreAttackWait && _returnMode == DragonflyReturnMode.Hover)
                {
                    _isReadyToPreAttackWait = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsReturnToSpider() => () =>
            {
                if (_isReadyToPreAttackWait && (_returnMode == DragonflyReturnMode.SpiderR || _returnMode == DragonflyReturnMode.SpiderL))
                {
                    _isReadyToPreAttackWait = false;
                    return true;
                }
                return false;
            };
        }


        // State Event Handle Methods

        private void GenerateAttackPosition()
        {
            _isReadyToAttackWait = true;
        }

        private void StartAttack(DragonflyPatrolAttackMode mode)
        {
            _movement.StartAttack(mode);
            _isAttacked = true;
            _isCollidedWithLamp = false;
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
                _movement.StartAttack(DragonflyPatrolAttackMode.Spider);
                _isAttacked = true;
            }
        }

        // Event Handle Methods
        private void OnCollisionPhaseReached()
        {
            _collisionProvider.FindClosestPointIndex();
            Radius = _collisionProvider.CurrentCollisionRadius;
        }

        private void OnReadyToAttackStateEntered(IState movementState)
        {
            _patrolAttackMode = (DragonflyPatrolAttackMode)Random.Range(0, 2);
            _isReadyToPreAttackWait = true;
        }

        private void OnReadyToSwarmAttackStateEntered(IState movementState)
        {
            if (!_isLampDestroyed)
            {
                if (movementState.GetType() == typeof(FDragonflyPatrolStateL))
                {
                    _swarm.PlayAttack(1);
                }
                else if (movementState.GetType() == typeof(FDragonflyPatrolStateR))
                {
                    _swarm.PlayAttack(-1);
                }    
            }
        }

        private void OnPreAttackStarted()
        {
            IsReceivedLampAttackDamage = false;
        }

        private void OnAttackStarted()
        {
            AnimatedAttackStarted?.Invoke(this);
        }

        private void OnAttackEnded()
        {
            // TODO: remove???
        }

        private void OnSwarmCalled()
        {
            SwarmCalled?.Invoke();
        }

        private void OnWaitForBounceStateEnded()
        {
            if (IsReceivedLampAttackDamage)
            {
            
            }
        }

        private void OnAfterAttackExitEnded(IState movementState)
        {
            _returnMode = _returnModes[Random.Range(0, 6)];
            _movement.ResolveReturnTransition(_returnMode);
            _isReadyToPreAttackWait = true;
        }

        private void OnReadyToSpiderAttackStateStarted()
        {
            _isReadyToAttackWait = true;
        }

        private void OnCatchSpiderStarted(int direction)
        {
            _spider.gameObject.SetActive(true);
            _spider.SetDirection(direction);
            _spider.Play();
        }

        private void OnProjectileDeactivated(Enemy spider, bool damaged)
        {
            ProjectileDeactivated?.Invoke(spider, damaged);
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

        private void OnDeathAnimationEnded()
        {
            OnDeathStateEnded();
            gameObject.SetActive(false);
            enabled = false;
        
        }

        private void OnMothAttackStarted(CollidableEnemy enemy)
        {
            ProjectileShot?.Invoke(enemy);
        }

        public void HandleLampDestroyed()
        {
            _movement.SetLampDestroyed();
            _swarm.TriggerGameover();
            _isLampDestroyed = true;
        }
    }
}
