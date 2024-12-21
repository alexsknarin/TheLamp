using System;
using System.Collections;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
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
        StartCoroutine(WaitForAttackEnd());
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
    
    private void PerformCooldown()
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
        _localTime += Time.deltaTime;
    }
    
    private void Update()
    {
        if (_isCooldownPlaying)
        {
            PerformCooldown();
        }
    }
    
}
