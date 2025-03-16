using System;
using System.Collections;
using UnityEngine;

public class PlayerAttackCooldownHandler : ITickable
{
    private float _power;
    private WaitForSeconds _attackEndWaitDuraiton;
    private float _localTime;
    private float _cooldownDuration;
    private bool _isCooldownPlaying = false;
    // Dependencies
    private MonoBehaviour _coroutineHost; 
    
    public PlayerAttackCooldownHandler(MonoBehaviour monoBehaviour)
    {
        _coroutineHost = monoBehaviour;
    }
    
    public event Action<float> PowerChanged;
    public event Action CooldownEnded;  // TODO: assess if this event is needed
    public event Action PlayerAttackEnded;
    
    public float Power 
    {
        get => _power;
        private set
        {
            _power = value;
            PowerChanged?.Invoke(_power);
        }
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

    public void StopCooldown()
    {
        _isCooldownPlaying = false;
    }

    public void Tick(float deltaTime)
    {
        if (_isCooldownPlaying)
        {
            PerformCooldown(deltaTime);
        }
    }

    private IEnumerator WaitForAttackEnd()
    {
        yield return _attackEndWaitDuraiton;
        StartCooldown();
        PlayerAttackEnded?.Invoke();
    }

    private void StartCooldown()
    {
        Power = 0;
        _localTime = 0;
        _isCooldownPlaying = true;
    }

    private void PerformCooldown(float deltaTime)
    {
        float phase = _localTime / _cooldownDuration;
        if (phase > 1)
        {
            _isCooldownPlaying = false;
            Power = 1;
            CooldownEnded?.Invoke();
            return;
        }
        Power = phase;
        _localTime += deltaTime;
    }
}
