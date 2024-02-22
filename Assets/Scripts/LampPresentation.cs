using System;
using UnityEngine;

public class LampPresentation : MonoBehaviour
{
    [SerializeField] private Light _lampLight;
    private Material _lampMaterial;
    
    [SerializeField] private float _lightAttackFadeDuration;
    [SerializeField] private float _lightRegenerateDuration;
    private LampStates _lampState = LampStates.Neutral;
    
    private float _lightPower = 1f;
    private float _attackDuration = 1f;
    private float _prevTime;
    private bool _isAttack = false;
    
    private readonly float _lightNeutralIntensity = 22;
    private readonly float _lampNeutralEmission = 1f;
    
    private readonly float _lightMinimumIntensity = 1;
    private readonly float _lampMinimumEmission = 0.05f;
    
    private readonly float _lightMaximumIntensity = 90;
    private readonly float _lampMaximumEmission = 30f;
    
    
    private void OnEnable()
    {
        LampAttackModel.OnLampAttack += StartAttackState;
        LampAttackModel.OnLampCurrentPowerChanged += PerformCooldownState;
    }

    private void OnDisable()
    {
        LampAttackModel.OnLampAttack -= StartAttackState;
        LampAttackModel.OnLampCurrentPowerChanged -= PerformCooldownState;
    }

   
    private void Start()
    {
        _lampMaterial = GetComponent<MeshRenderer>().material;
        ResetLightNeutralState();
    }

    private void Update()
    {
        if (_isAttack)
        {
            PerformAttack();
        }
    }

    private void ResetLightNeutralState()
    {
        _lampLight.intensity = _lightNeutralIntensity;
        _lampMaterial.SetFloat("_EmissionLevel", _lampNeutralEmission);
    }
    
    private void StartAttackState(int attackPower, float currentPower, float attackDuration)
    {
        _lampMaterial.SetFloat("_attackPower", 0f);
        _lightPower = Mathf.Pow(currentPower, 2f);
        _attackDuration = attackDuration;
        _prevTime = Time.time;
        _isAttack = true;
    }
    
    private void PerformAttack()
    {
        float phase = (Time.time - _prevTime) / _attackDuration;
        if (phase > 1)
        {
            _isAttack = false;
            _lampLight.intensity = _lightMinimumIntensity;
            _lampMaterial.SetFloat("_EmissionLevel", _lampMinimumEmission);
            return;
        }
        _lampLight.intensity = Mathf.Lerp(_lightMaximumIntensity * _lightPower, _lightMinimumIntensity, phase);
        _lampMaterial.SetFloat("_EmissionLevel", Mathf.Lerp(_lampMaximumEmission * _lightPower, _lampMinimumEmission, phase));
    }
    
    private void PerformCooldownState(float currentPower)
    {
        _lampLight.intensity = Mathf.Lerp(_lightMinimumIntensity, _lightNeutralIntensity, currentPower);
        _lampMaterial.SetFloat("_EmissionLevel", Mathf.Lerp(_lampMinimumEmission, _lampNeutralEmission, currentPower));
        _lampMaterial.SetFloat("_attackPower", currentPower);
    }
}
