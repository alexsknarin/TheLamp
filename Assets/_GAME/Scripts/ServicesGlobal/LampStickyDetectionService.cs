using System;
using System.Collections.Generic;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.ServicesGlobal
{
    public class LampStickyDetectionService : MonoBehaviour, IInitializable
    {
        [SerializeField] private float _stickyRadius = 0.35f;
        [SerializeField] private float _blockAttackRadius = 0.75f;
        [SerializeField] private float _attackZoneRadius = 0.62f; // TODO: control from the single source
        [SerializeField] private int _stickableCount = 0;
        [SerializeField] private bool _blockedAttacks = false;
        private List<IStickableWithLamp> _stickables = new();
        private List<IStickableWithLamp> _stickablesToRemove = new();
    
        private Vector2 _position;
        private float _collisionThreshold = 0.0001f;
        private float _combinedStickRadius;
        private int _attackBlockerCount = 0;
    
        public event Action EnemyAttackBlocked;
        public event Action EnemyAttackUnblocked;
        public event Action<IStickableWithLamp> EnemySticked;
        public event Action<IStickableWithLamp> EnemyUnSticked;

        public void Initialize()
        {
            _combinedStickRadius = _stickyRadius + _collisionThreshold;
            _blockedAttacks = false;
        }
    
        public void Reset()
        {
            _stickables.Clear();
            _stickablesToRemove.Clear();
            _stickableCount = 0;
            _blockedAttacks = false;
            enabled = false;
        }

        public void AddStickable(IStickableWithLamp stickableWithLamp)
        {
            if (!_stickables.Contains(stickableWithLamp))
            {
                enabled = true;
                _stickables.Add(stickableWithLamp);
                _stickableCount++;
            }
        }

        public void SetAttackZoneRadius(float attackZoneRadius)
        {
            _attackZoneRadius = attackZoneRadius;
        }

        private void RemoveStickable(IStickableWithLamp stickable)
        {
            if (_stickables.Contains(stickable))
            {
                _stickables.Remove(stickable);
                _stickableCount--; 
            }
        }

        private void Update()
        {
            if (_stickables.Count != 0)
            {
                _position = transform.position;
                CheckStickables();
            }
        }

        private void LateUpdate()
        {
            if (_stickablesToRemove.Count != 0)
            {
                foreach (var stickable in _stickablesToRemove)
                {
                    RemoveStickable(stickable);
                    EnemyUnSticked?.Invoke(stickable);
                }
                _stickablesToRemove.Clear();
            
                if (_stickables.Count == 0)
                {
                    enabled = false;
                }
            }
        }

        private void CheckStickables()
        {
            _attackBlockerCount = 0;
            foreach (var stickable in _stickables)
            {
                // Get Current distance
                Vector2 targetPosition = stickable.Position;
                Vector2 directionRaw = targetPosition - _position;
                float distance = directionRaw.magnitude;
            
                // Collision Measurements
                float attackZoneCombinedRadius = _attackZoneRadius + stickable.Radius;
            
                // Entering Attack Zone
                if (stickable.StickState == StickableState.Outside && distance < attackZoneCombinedRadius)
                {
                    stickable.HandleEnterAttackZone();
                }
            
                // Exiting Attack Zone Before Stick Because of Damage
                if (stickable.StickState == StickableState.InAttackZoneDamaged && distance > attackZoneCombinedRadius)
                {
                    _stickablesToRemove.Add(stickable);
                    stickable.HandleExitAttackZone();
                }
            
                // Stick detection
                if (stickable.StickState == StickableState.InAttackZone &&  distance < _combinedStickRadius + stickable.Radius)
                {
                    stickable.HandleStick(transform);
                    EnemySticked?.Invoke(stickable);
                }
            
                // Exiting Stick Zone
                if (stickable.StickState == StickableState.Sticked &&  distance > _attackZoneRadius + stickable.Radius)
                {
                    _stickablesToRemove.Add(stickable);
                    stickable.HandleExitAttackZone();
                }
            
                // Is Blocking Attack
                if ((distance < _blockAttackRadius + stickable.Radius) && stickable.AttackBlockState == AttackBlockerState.Outisde) 
                {
                    stickable.HandleEnterAttackBlockerZone();
                }
            
                // Counting Attack Blockers
                if (stickable.AttackBlockState == AttackBlockerState.Inside)
                {
                    _attackBlockerCount++;
                }
            }
        
            // Block Attacks
            if (_attackBlockerCount > 0)
            {
                if (!_blockedAttacks)
                {
                    _blockedAttacks = true;
                    EnemyAttackBlocked?.Invoke();
                }
            }
            else
            {
                if (_blockedAttacks)
                {
                    _blockedAttacks = false;
                    EnemyAttackUnblocked?.Invoke();
                }
            }
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _stickyRadius);
            Gizmos.DrawWireSphere(transform.position, _blockAttackRadius);
        
        }
    }
}
