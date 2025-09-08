using System;
using _GAME.Scripts.Enemies.Dragonfly;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class Megaspider : CollidableEnemy, IAnimatedEnemy, IBoss, ILampDestroyedDependable, IProjectileShooter
    {
        [SerializeField] private string _stateDebug;
        [Header("-- Attributes --")]
        [SerializeField] private int _maxHealth = 24;
        [SerializeField] private int _currentHealth;
        [SerializeField] private float _collisionRadius = 0.325f;
        [Header("-- Movement --")]
        [SerializeField] private MegaspiderMovement _movement;
        [Header("Swarm")]
        [SerializeField] private MegaspiderSwarm _swarm;
        [SerializeField] private MegaspiderAnimationClipEventListener _animationClipEvents;
        [SerializeField] private Transform _visibleBodyTransform;

        private bool _isLampDestroyed;
        [SerializeField] private AttackResult _attackResult;

        // Dependencies
        private Transform _cameraTransform;
        
        public event Action Started;
        public event Action<CollidableEnemy> AnimatedAttackStarted;
        public event Action Damaged;
        public event Action<int, int> HealthChanged;
        public event Action Died;
        public event Action SpreadRequested;
        
        public event Action<CollidableEnemy> ProjectileShot;
        public event Action<Enemy, bool> ProjectileDeactivated;

        public override Vector2 Position => CameraProjection.ProjectPointOnXYPlane(_cameraTransform.position, _visibleBodyTransform.position);
        
        public void Construct(Transform cameraTransform)
        {
            _cameraTransform = cameraTransform;
        }
        
        public override void Initialize()
        {
            Radius = _collisionRadius;
            _movement.SetCollisionRadius(_collisionRadius);
            _movement.Initialize();
            _swarm.Initialize();
            
            _animationClipEvents.ProjectileResetCalled += _swarm.Reset;
            _animationClipEvents.ProjectileAttack01Called += OnProjectileAttack01Called;
            _animationClipEvents.ProjectileAttack02Called += OnProjectileAttack02Called;

            _movement.AnimatedAttackStarted += OnAnimatedAttackStarted;
            _movement.DeathStateEnded += OnDeathStateEnded;

            _swarm.Projecile01FallEnded += OnProjecile01FallEnded;
            _swarm.Projecile02FallEnded += OnProjecile02FallEnded;
        }

        private void OnDestroy()
        {
            _animationClipEvents.ProjectileResetCalled -= _swarm.Reset;
            _animationClipEvents.ProjectileAttack01Called -= OnProjectileAttack01Called;
            _animationClipEvents.ProjectileAttack02Called -= OnProjectileAttack02Called;
            
            _movement.AnimatedAttackStarted -= OnAnimatedAttackStarted;
            _movement.DeathStateEnded -= OnDeathStateEnded;
            
            _swarm.Projecile01FallEnded -= OnProjecile01FallEnded;
            _swarm.Projecile02FallEnded -= OnProjecile02FallEnded;
        }

        public override void Play()
        {
            IsGameOver = false;
            _isLampDestroyed = false;
            
            _currentHealth = _maxHealth;
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            _movement.Play();
            Started?.Invoke();
        }

        public override void ReceiveDamage(int damageAmount)
        {
            IsReadyForDamage = false;
            _currentHealth -= damageAmount;
            IsReceivedLampAttackDamage = true;
            
            if (_currentHealth > 0)
            {
                _attackResult = AttackResult.Fail;
                _movement.TriggerFall(_attackResult);
                Damaged?.Invoke();
                HealthChanged?.Invoke(_currentHealth, _maxHealth);
            }
            else
            {
                if (_attackResult != AttackResult.Death)
                {
                    _attackResult = AttackResult.Death;
                    _currentHealth = 0; 
                    _movement.TriggerFall(_attackResult);
                    Died?.Invoke();
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
            CollisionState = CollidableState.AfterCollision;
        }

        public override void HandleExitAttackZone()
        {
            CollisionState = CollidableState.Outside;
            IsReadyForDamage = false;
            if (_attackResult != AttackResult.Death && _attackResult != AttackResult.Fail)
            {
                _movement.TriggerFall(_attackResult);
            }
        }

        public void HandleLampDestroyed()
        {
            // _movement.SetLampDestroyed();
            // _swarm.TriggerGameover();
            
            _isLampDestroyed = true;
        }

        public override Vector3 ProvideImpactPoint()
        {
            return _visibleBodyTransform.position;
        }

        private void OnAnimatedAttackStarted()
        {
            AnimatedAttackStarted?.Invoke(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_visibleBodyTransform.position, _collisionRadius);
        }

        private void OnProjectileAttack01Called()
        {
            ProjectileShot?.Invoke(_swarm.Projectile01);
            _swarm.Attack01();
        }
        
        private void OnProjectileAttack02Called()
        {
            ProjectileShot?.Invoke(_swarm.Projectile02);
            _swarm.Attack02();
        }

        private void OnProjecile01FallEnded(bool damaged)
        {
            ProjectileDeactivated?.Invoke(_swarm.Projectile01, damaged);
        }

        private void OnProjecile02FallEnded(bool dameged)
        {
            ProjectileDeactivated?.Invoke(_swarm.Projectile02, dameged);
        }
    }
}
