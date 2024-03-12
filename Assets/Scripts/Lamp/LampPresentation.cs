using UnityEngine;

public class LampPresentation : MonoBehaviour
{
    [SerializeField] private Light _lampLight;
    private Material _lampMaterial;
    [SerializeField] private GameObject _lampAttackZoneObject;
    private Material _lampAttackZoneMaterial;
    [SerializeField] private float _lightAttackFadeDuration;
    [SerializeField] private float _lightRegenerateDuration;
    [SerializeField] private float _lightDamageDuration;
    private LampStates _lampState = LampStates.Neutral;
    
    private float _lightPower = 1f;
    private float _attackDuration = 1f;
    private float _prevTimeAttack;
    private bool _isAttack = false;
    private float _prevTimeDamage;
    private bool _isDamage = false;
    private bool _isBlockedAttack = false;
    
    private readonly float _lightNeutralIntensity = 22;
    private readonly float _lampNeutralEmission = 1f;
    
    private readonly float _lightMinimumIntensity = 0.1f;
    private readonly float _lampMinimumEmission = 0.01f;
    
    private readonly float _lightMaximumIntensity = 90;
    private readonly float _lampMaximumEmission = 30f;
    
    
    private void OnEnable()
    {
        LampAttackModel.OnLampAttack += StartAttackState;
        LampAttackModel.OnLampCurrentPowerChanged += PerformCooldownState;
        LampAttackModel.OnLampBlockedAttack += StartBlockedAttackState;
    }

    private void OnDisable()
    {
        LampAttackModel.OnLampAttack -= StartAttackState;
        LampAttackModel.OnLampCurrentPowerChanged -= PerformCooldownState;
        LampAttackModel.OnLampBlockedAttack -= StartBlockedAttackState;
    }

   
    private void Start()
    {
        _lampMaterial = GetComponent<MeshRenderer>().material;
        _lampAttackZoneMaterial = _lampAttackZoneObject.GetComponent<MeshRenderer>().material;
        ResetLightNeutralState();
    }

    private void Update()
    {
        if (_isAttack)
        {
            PerformAttack();
        }
        if (_isDamage)
        {
            PerformDamage();            
        }
    }

    private void ResetLightNeutralState()
    {
        _lampLight.intensity = _lightNeutralIntensity;
        _lampMaterial.SetFloat("_EmissionLevel", _lampNeutralEmission);
    }
    
    private void StartAttackState(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _lampMaterial.SetFloat("_attackPower", 0f);
        _lightPower = Mathf.Pow(currentPower, 2f);
        _attackDuration = attackDuration;
        _prevTimeAttack = Time.time;
        _isAttack = true;
        _lampAttackZoneObject.transform.localScale = Vector3.one * (attackDistance * 2);
    }
    
    private void StartBlockedAttackState(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _isBlockedAttack = true;
        _lampMaterial.SetFloat("_attackPower", 0f);
        _lightPower = Mathf.Pow(currentPower, 2f);
        _attackDuration = attackDuration;
        _prevTimeAttack = Time.time;
        _isAttack = true;
        _lampAttackZoneObject.transform.localScale = Vector3.one * (attackDistance * 2);
    }
    
    private void PerformAttack()
    {
        float phase = (Time.time - _prevTimeAttack) / _attackDuration;
        if (phase > 1)
        {
            _isAttack = false;
            _lampLight.intensity = _lightMinimumIntensity;
            _lampMaterial.SetFloat("_EmissionLevel", _lampMinimumEmission);
            _lampAttackZoneMaterial.SetFloat("_Alpha", 0);
            _isBlockedAttack = false;
            return;
        }
        if(!_isBlockedAttack)
        {
            _lampLight.intensity = Mathf.Lerp(_lightMaximumIntensity * _lightPower, _lightMinimumIntensity, phase);
            _lampMaterial.SetFloat("_EmissionLevel", Mathf.Lerp(_lampMaximumEmission * _lightPower, _lampMinimumEmission, phase));
            _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(_lightPower, 0, phase));
        }
    }
    
    private void PerformCooldownState(float currentPower)
    {
        _lampLight.intensity = Mathf.Lerp(_lightMinimumIntensity, _lightNeutralIntensity, currentPower);
        _lampMaterial.SetFloat("_EmissionLevel", Mathf.Lerp(_lampMinimumEmission, _lampNeutralEmission, currentPower));
        _lampMaterial.SetFloat("_attackPower", currentPower);
    }
    
    public void StartDamageState()
    {
        _lampMaterial.SetFloat("_Damage", 1f);
        _isDamage = true;
        _prevTimeDamage = Time.time;
    }
    
    private void PerformDamage()
    {
        if (_isDamage)
        {
            float phase = (Time.time - _prevTimeDamage) / _lightDamageDuration;
            if (phase > 1)
            {
                _isDamage = false;
                _lampMaterial.SetFloat("_Damage", 0f);
                return;
            }
            _lampMaterial.SetFloat("_Damage", Mathf.Lerp(1f, 0f, phase));
        }
    }
}
