using System.Collections.Generic;
using UnityEngine;

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
    
    public bool BlockedAttacks => _blockedAttacks;

    public void Initialize()
    {
        _combinedStickRadius = _stickyRadius + _collisionThreshold;
        _blockedAttacks = false;
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

    public void RemoveStickable(IStickableWithLamp stickable)
    {
        if (_stickables.Contains(stickable))
        {
            _stickables.Remove(stickable);
            _stickableCount--; 
        }
    }

    private void Update()
    {
        _position = transform.position;
        if (_stickables.Count != 0)
        {
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
        _blockedAttacks = false;
        if (_attackBlockerCount > 0)
        {
            _blockedAttacks = true;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _stickyRadius);
        Gizmos.DrawWireSphere(transform.position, _blockAttackRadius);
        
    }
}
