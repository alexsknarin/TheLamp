using System;
using UnityEngine;

public class Wasp : BossBase
{
    [SerializeField] private WaspMovement _waspMovement;
    [SerializeField] private FWaspMovement _fWaspMovement;
    [SerializeField] private WaspPresentation _waspPresentation;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private Collider2D _collider;
    public override EnemyType EnemyType => EnemyType.Wasp;

    private void OnEnable()
    {
        // _waspMovement.OnBossAttackStartedEvent += UpdateRecievedLampAttackStatus;
        // _waspMovement.OnDeathStateEndedEvent += HandleDeathMoveStateEnd;
        // _waspMovement.OnLeftTheScreenEvent += HandleLeftScreen;
        Lamp.OnLampDeadEvent += HandleLampDead; // TODO: manage from enemy manager
    }
    
    private void OnDisable()
    {
        // _waspMovement.OnBossAttackStartedEvent -= UpdateRecievedLampAttackStatus;
        // _waspMovement.OnDeathStateEndedEvent -= HandleDeathMoveStateEnd;
        // _waspMovement.OnLeftTheScreenEvent -= HandleLeftScreen;
        Lamp.OnLampDeadEvent -= HandleLampDead;
    }
    public override void Initialize()
    {
        ReceivedLampAttack = false;
        _isGameover = false;
        _currentHealth = _maxHealth;
        _waspPresentation.Initialize();
        // _waspMovement.Initialize();
        gameObject.SetActive(false);
    }

    public override void Reset()
    {
        ReceivedLampAttack = false;
        _currentHealth = _maxHealth;
        _waspPresentation.Initialize();
        // _waspMovement.MovementReset();
        gameObject.SetActive(false);
    }
    
    public override void Play()
    {
        gameObject.SetActive(true);
        _waspPresentation.ResetTrail();
        _waspPresentation.Initialize();
        
        // _waspMovement.Play();
        _fWaspMovement.Play();
    }

    public void TriggerSpread()
    {
        OnTriggerSpreadInvoke();
    }
    
    private void UpdateRecievedLampAttackStatus()
    {
        if (ReceivedLampAttack)
        {
            ReceivedLampAttack = false;
        }
    }

    public override void HandleCollisionWithStickZone()
    {
    }   

    public override void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
        ReceivedLampAttack = true;
        if (_currentHealth > 0)
        {
            float damagePhase = (1 - (float)_currentHealth/_maxHealth) + 0.2f;
            damagePhase = Mathf.Clamp(damagePhase, 0, 1);
            _waspPresentation.SetDamage(damagePhase);
            _waspPresentation.PlayDamageParticles();
        }
        else
        {
            // _waspMovement.SetDead();
            _waspPresentation.PlayDeath();
            _waspPresentation.PlayDamageParticles();
        }    
    }

    public override void UpdateAttackAvailability()
    {
    }

    public override void ReturnToPool()
    {
    }

    public override void SpreadStart()
    {
    }

    public override void StartAttack()
    {
    }
    
    public override void HandleCollisionWithLamp()
    {
    }
    
    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }

    private void HandleLeftScreen()
    {
        if (_isGameover)
        {
            Reset();
            _isGameover = false;
        }
    }
    
    private void HandleLampDead(EnemyBase enemy)
    {
        // _waspMovement.SetLampDestroyed();
    }
    
    private void HandleDeathMoveStateEnd()
    {
        OnDeathInvoke();
        _waspPresentation.Reset();
        gameObject.SetActive(false);
    }
    
    
    // Animation events
    private void ResetTrail()
    {
        _waspPresentation.ResetTrail();
    }
    
    public void ClipEnded()
    {
        _fWaspMovement.ClipEnded();
    }
    
    public void EnableCollider()
    {
        _collider.enabled = true;
    }
    
    public void DisableCollider()
    {
        _collider.enabled = false;
    }
}
