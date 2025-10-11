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
        private Vector2 _projectedPosition;
        private bool _isLampDestroyed;
        [SerializeField] private AttackResult _attackResult;

        // Dependencies
        private Vector3 _cameraPosition;
        
        public event Action Started;
        public event Action<CollidableEnemy> AnimatedAttackStarted;
        public event Action Damaged;
        public event Action<int, int> HealthChanged;
        public event Action Dead;
        public event Action SpreadRequested;
        
        public event Action<CollidableEnemy> ProjectileShot;
        public event Action<Enemy, bool> ProjectileDeactivated;

        public override Vector2 Position => GetProjectedPosition(_cameraPosition, _visibleBodyTransform.position);
        public override float Radius => GetProjectedRadius(_cameraPosition, _visibleBodyTransform.position, _collisionRadius);
        public override string CollidableName => gameObject.name;

        public void Construct(Transform cameraTransform)
        {
            _cameraPosition = cameraTransform.position;
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
            _attackResult = AttackResult.Success;
            
            _currentHealth = _maxHealth;
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            _movement.Play();
            Started?.Invoke();
            
            _swarm.HideProjectiles();
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
                    Dead?.Invoke();
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
            _attackResult = AttackResult.Success;
            IsReceivedLampAttackDamage = false;
            _swarm.HideProjectiles();
            AnimatedAttackStarted?.Invoke(this);
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

        private Vector2 GetProjectedPosition(Vector3 cameraPosition, Vector3 targetPosition)
        {
            _projectedPosition = CameraProjection.ProjectPointOnXYPlane(cameraPosition, targetPosition);
            return _projectedPosition;
        }
        
        private float GetProjectedRadius(Vector3 cameraPosition, Vector3 targetPosition, float radius)
        {
            Vector3 targetPositionTop = targetPosition;
            targetPositionTop.y += radius;
            Vector2 projectedTargetPosition = CameraProjection.ProjectPointOnXYPlane(cameraPosition, targetPositionTop);
            return Vector2.Distance(projectedTargetPosition, _projectedPosition);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_visibleBodyTransform.position, _collisionRadius);
            Gizmos.color = Color.aquamarine;
            Vector3 projectedPosition = Position;
            float radius = Radius;
            Gizmos.DrawWireSphere(projectedPosition, radius);
        }
    }
}
