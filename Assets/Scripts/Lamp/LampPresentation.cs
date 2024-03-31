using UnityEngine;
using UnityEngine.Rendering;

public class LampPresentation : MonoBehaviour, IInitializable
{
    [SerializeField] private Light _lampLight;
    private Material _lampMaterial;
    [SerializeField] private GameObject _lampAttackZoneObject;
    private Material _lampAttackZoneMaterial;
    [SerializeField] private float _lightDamageDuration;
    [Header("HealthBar")]
    [SerializeField] private MeshRenderer _healthBarMeshRenderer;
    [SerializeField] private Transform _healthBarTransform;
    private Material _healthBarMaterial;
    [Header("Intro")]
    [SerializeField] private AnimationCurve _introAnimationCurve;
    [Header("Death")]
    [SerializeField] private AnimationCurve _deathAnimationCurve;
    
    
    private float _lightPower = 1f;
    private float _attackDuration = 1f;
    private float _localTimeAttack;
    
    private bool _isIntro = false;
    private float _introDuration;
    private float _localTimeIntro;
    private float _initialHealth;
    
    private bool _isAttack = false;
    private float _localTimeDamage;
    
    private bool _isDamage = false;
    private bool _isBlockedAttack = false;

    private float _deathStateDuration;
    private bool _isDeath = false;
    private float _localTimeDeath;
    
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
    
    public void Initialize()
    {
        _lampMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        _lampAttackZoneMaterial = _lampAttackZoneObject.GetComponent<MeshRenderer>().material;
        ResetLightNeutralState();
        _healthBarMaterial = _healthBarMeshRenderer.material;
        _healthBarMaterial.SetFloat("_Health", 1f);
        _healthBarTransform.localScale = Vector3.one;
        
        // arrange before the intro
        UpdateHealthBar(0f);
        _lampLight.intensity = 0;
        _lampMaterial.SetFloat("_EnableAnimation", 0);
        _lampMaterial.SetFloat("_attackPower", 0);
    }
    
    public void UpdateHealthBar(float health)
    {
        _healthBarMaterial.SetFloat("_Health", health);
        Vector3 scale = Vector3.one;
        scale.x = health;
        _healthBarTransform.localScale = scale;
    }

    private void ResetLightNeutralState()
    {
        _lampLight.intensity = _lightNeutralIntensity;
        _lampMaterial.SetFloat("_EmissionLevel", _lampNeutralEmission);
        _lampMaterial.SetFloat("_Damage", 0f);
        _lampMaterial.SetFloat("_BlockedMode", 0f);
    }
    
    public void EnableBlockedMode()
    {
        _lampMaterial.SetFloat("_BlockedMode", 1f);
    }
    
    public void DisableBlockedMode()
    {
        _lampMaterial.SetFloat("_BlockedMode", 0f);
    }
   
    private void StartAttackState(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _lampMaterial.SetFloat("_attackPower", 0f);
        _lightPower = Mathf.Pow(currentPower, 2f);
        _attackDuration = attackDuration;
        _localTimeAttack = 0;
        _isAttack = true;
        _lampAttackZoneObject.transform.localScale = Vector3.one * (attackDistance * 2);
    }
    
    private void StartBlockedAttackState(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _isBlockedAttack = true;
        _lampMaterial.SetFloat("_attackPower", 0f);
        _lightPower = Mathf.Pow(currentPower, 2f);
        _attackDuration = attackDuration;
        _localTimeAttack = 0;
        _isAttack = true;
        _lampAttackZoneObject.transform.localScale = Vector3.one * (attackDistance * 2);
    }
    
    private void PerformAttack()
    {
        float phase = _localTimeAttack / _attackDuration;
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
        _localTimeAttack += Time.deltaTime;
    }
    
    private void PerformCooldownState(float currentPower)
    {
        _lampLight.intensity = Mathf.Lerp(_lightMinimumIntensity, _lightNeutralIntensity, currentPower);
        _lampMaterial.SetFloat("_EmissionLevel", Mathf.Lerp(_lampMinimumEmission, _lampNeutralEmission, currentPower));
        _lampMaterial.SetFloat("_attackPower", currentPower);
    }
    
    public void StartIntroState(float introDuration, float initialHealth)
    {
        _isIntro = true;
        _introDuration = introDuration;
        _initialHealth = initialHealth;
        _localTimeIntro = 0;
    }

    private void PerformIntro()
    {
        float phase = _localTimeIntro / _introDuration;
        if (phase > 1)
        {
            _isIntro = false;
            _lampMaterial.SetFloat("_EnableAnimation", 1);
            _lampMaterial.SetFloat("_attackPower", 1);
            _lampLight.intensity = _lightNeutralIntensity;
            UpdateHealthBar(_initialHealth);
            return;
        }
        float phaseAnimated = _introAnimationCurve.Evaluate(phase);
        float enablePhase = Mathf.Clamp(phaseAnimated * 1.25f, 0f, 1f);
        float attackPowerPhase = Mathf.Clamp((phaseAnimated - 0.75f) * 4, 0f, 1f);
        float lightIntensity = Mathf.Lerp(0, _lightNeutralIntensity, phaseAnimated);
        float health = Mathf.Lerp(0, _initialHealth, phaseAnimated);
        
        UpdateHealthBar(health);
        _lampMaterial.SetFloat("_EnableAnimation", enablePhase);
        _lampMaterial.SetFloat("_attackPower", attackPowerPhase);
        _lampLight.intensity = lightIntensity;
        _localTimeIntro += Time.deltaTime;
    }
    
    public void StartDeathState(float deathStateDuration)
    {
        _deathStateDuration = deathStateDuration;
        _localTimeDeath = 0;
        _isDeath = true;
    }

    private void PerformDeath()
    {
        float phase = _localTimeDeath / _deathStateDuration;
        if (phase > 1)
        {
            _isDeath = false;
            _lampMaterial.SetFloat("_EnableAnimation", 0);
            _lampMaterial.SetFloat("_attackPower", 0);
            _lampLight.intensity = 0;
            UpdateHealthBar(0);
            return;
        }
        
        float phaseAnimated = _deathAnimationCurve.Evaluate(phase);
        
        _lampMaterial.SetFloat("_EnableAnimation", phaseAnimated);
        _lampMaterial.SetFloat("_attackPower", 0);
        
        _lampLight.intensity = Mathf.Lerp(_lightNeutralIntensity, 0f, 1-phaseAnimated);
        
        _localTimeDeath += Time.deltaTime;
    }
    
    
    public void StartDamageState()
    {
        _lampMaterial.SetFloat("_Damage", 1f);
        _isDamage = true;
        _localTimeDamage = 0;
    }
    
    private void PerformDamage()
    {
        float phase = _localTimeDamage / _lightDamageDuration;
        if (phase > 1)
        {
            _isDamage = false;
            _lampMaterial.SetFloat("_Damage", 0f);
            return;
        }
        _lampMaterial.SetFloat("_Damage", Mathf.Lerp(1f, 0f, phase));
        _localTimeDamage += Time.deltaTime;
    }
    
    private void Update()
    {
        if (_isIntro)
        {
            PerformIntro();
        }
        if (_isAttack)
        {
            PerformAttack();
        }
        if (_isDamage)
        {
            PerformDamage();            
        }
        if (_isDeath)
        {
            PerformDeath();
        }
    }
}
