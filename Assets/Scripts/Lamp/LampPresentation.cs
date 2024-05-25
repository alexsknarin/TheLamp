using UnityEngine;
using UnityEngine.Rendering;

public class LampPresentation : MonoBehaviour, IInitializable
{
    [SerializeField] private Light _lampLight;
    private Material _lampMaterial;
    
    
    [SerializeField] private LampEmissionController _lampEmissionController;
    [Range(0, 1)]
    [SerializeField] private float _blockedModeStrength;
    
    [SerializeField] private GameObject _lampAttackZoneObject;
    private Material _lampAttackZoneMaterial;
    [Header("HealthBar")]
    [SerializeField] private LampHealthBar _lampHealthBar;
    [Header("Intro")]
    [SerializeField] private LampIntroAnimation _lampIntroAnimation;
    [Header("Cooldown")]
    [SerializeField] private LampCooldown _lampCooldown;
    [Header("Death")]
    [SerializeField] private LampDeathAnimation _lampDeathAnimation;
    [Header("Damage")]
    [SerializeField] private LampDamageAnimation _lampDamageAnimation;
    [SerializeField] private float _lightDamageDuration;
    [Header("Attack")]
    [SerializeField] private LampAttackAnimation _lampAttackAnimation;
    
    private bool isBlocked = false;
    
    private readonly float _lightNeutralIntensity = 22;
    private readonly float _lampNeutralEmission = 1f;
    
    private readonly float _lightMinimumIntensity = 0.1f;
    private readonly float _lampMinimumEmission = 0.01f;
    
    private readonly float _lightMaximumIntensity = 130;
    private readonly float _lampMaximumEmission = 35f;
    
    
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
        _lampHealthBar.Initialize();
        _lampIntroAnimation.Initialize();
        _lampDeathAnimation.Initialize();
        _lampDamageAnimation.Initialize();
        _lampAttackAnimation.Initialize();

        // arrange before the intro
        _lampHealthBar.UpdateHealth(0, 0);
        _lampLight.intensity = 0;
        _lampMaterial.SetFloat("_EnableAnimation", 0);
        _lampMaterial.SetFloat("_attackPower", 0);
    }
    
    public void UpdateHealthBar(float normalizedHealth, int actualHealth)
    {
        _lampHealthBar.UpdateHealth(normalizedHealth, actualHealth);
    }
    
    public void UpgradeHealthBar()
    {
        _lampHealthBar.PlayUpgrade();
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
        _lampMaterial.SetFloat("_BlockedMode", _blockedModeStrength);
        _lampEmissionController.BlockedModeMix = 0.9f;
        isBlocked = true;
    }
    
    public void DisableBlockedMode()
    {
        _lampMaterial.SetFloat("_BlockedMode", 0f);
        _lampEmissionController.BlockedModeMix = 0;
        isBlocked = false;
    }
   
    private void StartAttackState(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _lampAttackAnimation.Play(attackDuration, false, _lightMinimumIntensity, 
            _lampMinimumEmission, currentPower, _lightMaximumIntensity, 
            _lampMaximumEmission, attackDistance);
    }
    
    private void StartBlockedAttackState(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _lampAttackAnimation.Play(attackDuration, true, _lightMinimumIntensity, 
            _lampMinimumEmission, currentPower, _lightMaximumIntensity, 
            _lampMaximumEmission, attackDistance);
    }
    
    private void PerformCooldownState(float currentPower)
    {
        _lampLight.intensity = Mathf.Lerp(_lightMinimumIntensity, _lightNeutralIntensity, currentPower);
        _lampMaterial.SetFloat("_EmissionLevel", Mathf.Lerp(_lampMinimumEmission, _lampNeutralEmission, currentPower));
        _lampMaterial.SetFloat("_attackPower", currentPower);
        
        // NEW LAMP
        _lampCooldown.PerformCooldown(currentPower, isBlocked);
    }
    
    public void StartIntroState(float introDuration, int currentHealth, int maxHealth)
    {
        _lampIntroAnimation.Play(introDuration, currentHealth, maxHealth, _lightNeutralIntensity);
    }
    
    public void StartDeathState(float deathStateDuration)
    {
        _lampDeathAnimation.Play(deathStateDuration, _lightNeutralIntensity);
    }
    
    public void StartDamageState()
    {
        _lampDamageAnimation.Play(_lightDamageDuration);
    }
}
