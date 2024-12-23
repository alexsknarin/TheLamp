using System;
using System.Collections;
using UnityEngine;

public class PlayerAttackController : ITickable
{
    private float _power;
    public float Power 
    {
        get => _power;
        private set
        {
            _power = value;
            OnPowerChangedEvent?.Invoke(_power);
        }
    } 
    public event Action<float> OnPowerChangedEvent;
    public event Action OnCooldownEndedEvent;  // TODO: assess if this event is needed
    private WaitForSeconds _attackEndWaitDuraiton;
    private float _localTime;
    private float _cooldownDuration;
    private bool _isCooldownPlaying = false;
    public event Action OnAttackEndedEvent; 
    
    // Dependencies
    private MonoBehaviour _coroutineHost; 
    public PlayerAttackController(MonoBehaviour monoBehaviour)
    {
        _coroutineHost = monoBehaviour;
    }
    
    public void SetAttackDuration(float attackDuration)
    {
        _attackEndWaitDuraiton = new WaitForSeconds(attackDuration);
    }
    public void SetCooldownDuration(float cooldownDuration)
    {
        _cooldownDuration = cooldownDuration;
    }
    
    public void PlayAttack()
    {
        if (_isCooldownPlaying)
        {
            StopCooldown();
        }
        _coroutineHost.StartCoroutine(WaitForAttackEnd());
    }
    
    public void PlayCooldown()
    {
        if (_isCooldownPlaying)
        {
            StopCooldown();
        }
        StartCooldown();
    }
    
    private IEnumerator WaitForAttackEnd()
    {
        yield return _attackEndWaitDuraiton;
        StartCooldown();
        OnAttackEndedEvent?.Invoke();
    }
    
    private void StartCooldown()
    {
        Power = 0;
        _localTime = 0;
        _isCooldownPlaying = true;
    }
    
    private void StopCooldown()
    {
        _isCooldownPlaying = false;
    }
    
    private void PerformCooldown(float deltaTime)
    {
        float phase = _localTime / _cooldownDuration;
        if (phase > 1)
        {
            _isCooldownPlaying = false;
            Power = 1;
            OnCooldownEndedEvent?.Invoke();
            return;
        }
        Power = phase;
        _localTime += deltaTime;
    }
    
    public void Tick(float deltaTime)
    {
        if (_isCooldownPlaying)
        {
            PerformCooldown(deltaTime);
        }
    }
}
