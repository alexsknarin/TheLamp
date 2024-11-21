using System;
using System.Collections;
using UnityEngine;

public class Wasp : BossBase
{
    [SerializeField] private WaspMovement _waspMovement;
    [SerializeField] private FWaspMovement _fWaspMovement;
    [SerializeField] private WaspPresentation _waspPresentation;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private bool _isAttackPauseEnabled = false;
    [SerializeField] private float _attackPauseTime = 0.5f;
    public override EnemyType EnemyType => EnemyType.Wasp;
    
    private WaitForSeconds _attackPause;
    
    private bool _isDead = false;

    private void Awake()
    {
        _attackPause = new WaitForSeconds(_attackPauseTime);
    }

    private void OnEnable()
    {
        _fWaspMovement.OnBossAttackStartedEvent += UpdateRecievedLampAttackStatus;
        _fWaspMovement.OnDeathStateEndedEvent += HandleDeathMoveStateEnd;
        // _fWaspMovement.OnLeftTheScreenEvent += HandleLeftScreen;
        Lamp.OnLampDeadEvent += HandleLampDead; // TODO: manage from enemy manager
    }
    
    private void OnDisable()
    {
        _fWaspMovement.OnBossAttackStartedEvent -= UpdateRecievedLampAttackStatus;
        _fWaspMovement.OnDeathStateEndedEvent -= HandleDeathMoveStateEnd;
        // _fWaspMovement.OnLeftTheScreenEvent -= HandleLeftScreen;
        Lamp.OnLampDeadEvent -= HandleLampDead;
    }
    public override void Initialize()
    {
        ReceivedLampAttack = false;
        _isGameover = false;
        _currentHealth = _maxHealth;
        _waspPresentation.Initialize();
        _fWaspMovement.Initialize();
        gameObject.SetActive(false);
    }

    public override void Play()
    {
        _isDead = false;
        gameObject.SetActive(true);
        _waspPresentation.ResetTrail();
        _waspPresentation.Initialize();
        _fWaspMovement.Play();
    }

    public override void Reset()
    {
        ReceivedLampAttack = false;
        _currentHealth = _maxHealth;
        _waspPresentation.Initialize();
        // _fWaspMovement.MovementReset();
        gameObject.SetActive(false);
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
            _fWaspMovement.SetDamaged();
        }
        else
        {
            _currentHealth = 0;
            _fWaspMovement.SetDead();
            _waspPresentation.PlayDeath();
            _waspPresentation.PlayDamageParticles();
            _isDead = true;
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
        if (_isAttackPauseEnabled)
        {
            StartCoroutine(AttackPause());    
        }
        else
        {
            _fWaspMovement.SetCollidedWithLamp();
        }
    }
    
    private IEnumerator AttackPause()
    {
        yield return _attackPause;
        _fWaspMovement.SetCollidedWithLamp();
    }
    
    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }

    private void HandleLampDead(EnemyBase enemy)
    {
        _fWaspMovement.SetLampDestroyed();
    }
    
    private void HandleDeathMoveStateEnd()
    {
        OnDeathInvoke();
        _waspPresentation.Reset();
        StartCoroutine(DeactivateOnDeath());
    }
    
    // Let movement FSM to switch to idle state properly and then deactivate
    private IEnumerator DeactivateOnDeath()
    {
        yield return null;
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
