using System;
using UnityEngine;

enum LampStates
{
    Neutral,
    Attack,
    Regenerate
}

public class LampPresentation : MonoBehaviour
{
    [SerializeField] private Light _lampLight;
    private Material _lampMaterial;
    
    [SerializeField] private float _lightAttackFadeDuration;
    [SerializeField] private float _lightRegenerateDuration;
    private LampStates _lampState = LampStates.Neutral;
    
    private float _attackPower = 1f;
    private float _attackPowerCached;
    
    private float _prevTime;
    
    private readonly float _lightNeutralIntensity = 22;
    private readonly float _lampNeutralEmission = 1f;
    
    private readonly float _lightMinimumIntensity = 1;
    private readonly float _lampMinimumEmission = 0.05f;
    
    private readonly float _lightMaximumIntensity = 90;
    private readonly float _lampMaximumEmission = 30f;
    
    
    private void OnEnable()
    {
        PlayerInput.OnPlayerAttack += StartAttack;
    }

    private void OnDisable()
    {
        PlayerInput.OnPlayerAttack -= StartAttack;
    }

    private void Start()
    {
        _lampMaterial = GetComponent<MeshRenderer>().material;
        ResetLightNeutral();
        _lampState = LampStates.Neutral;
    }
    
    private void ResetLightNeutral()
    {
        _lampLight.intensity = _lightNeutralIntensity;
        _lampMaterial.SetFloat("_EmissionLevel", _lampNeutralEmission);
        UpdateMaterialAttackPower();
    }
    
    private void ResetLightRegenerate()
    {
        _lampLight.intensity = _lightMinimumIntensity;
        _lampMaterial.SetFloat("_EmissionLevel", _lampMinimumEmission);
        UpdateMaterialAttackPower();
    }
    
    private void UpdateMaterialAttackPower()
    {
        _lampMaterial.SetFloat("_attackPower", _attackPower);
    }
    
    private void StartAttack()
    {
        if (_lampState != LampStates.Attack)
        {
            _lampState = LampStates.Attack;
            _prevTime = Time.time;
            _attackPowerCached = (float)Math.Pow(_attackPower, 2f);
        }
    }
    
    private void PerformAttack()
    {
        float phase = (Time.time - _prevTime) / _lightAttackFadeDuration;
        
        if (phase > 1)
        {
            StartRegenerate();
            return;
        }
        _lampLight.intensity = Mathf.Lerp(_lightMaximumIntensity * _attackPowerCached, _lightMinimumIntensity, phase);
        _lampMaterial.SetFloat("_EmissionLevel", Mathf.Lerp(_lampMaximumEmission * _attackPowerCached, _lampMinimumEmission, phase));
        _attackPower = 0;
        UpdateMaterialAttackPower();
    }
    
    private void StartRegenerate()
    {
        ResetLightRegenerate();
        _prevTime = Time.time;
        _lampState = LampStates.Regenerate;
    }
    
    private void PerformRegenerate()
    {
        float phase = (Time.time - _prevTime) / _lightRegenerateDuration;
        _attackPower = phase;
        UpdateMaterialAttackPower();
        if (phase > 1)
        {
            StartNeutral();
            _attackPower = 1f;
            UpdateMaterialAttackPower();
            return;
        }
        _lampLight.intensity = Mathf.Lerp(_lightMinimumIntensity, _lightNeutralIntensity, phase);
        _lampMaterial.SetFloat("_EmissionLevel", Mathf.Lerp(_lampMinimumEmission, _lampNeutralEmission, phase));
    }
    
    private void StartNeutral()
    {
        ResetLightNeutral();
        _lampState = LampStates.Neutral;
    }

    private void Update()
    {
        switch (_lampState)
        {
            case LampStates.Attack:
                PerformAttack();
                break;
            case LampStates.Regenerate:
                PerformRegenerate();
                break;
            case LampStates.Neutral:
                break;
        }
    }
}
