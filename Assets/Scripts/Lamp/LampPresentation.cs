using UnityEngine;
using UnityEngine.Rendering;

public class LampPresentation : MonoBehaviour, IInitializable
{
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
        _lampAttackZoneMaterial = _lampAttackZoneObject.GetComponent<MeshRenderer>().material;
        ResetLightNeutralState();
        _lampHealthBar.Initialize();
        _lampAttackAnimation.Initialize();

        // arrange before the intro
        _lampHealthBar.UpdateHealth(0, 0);
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
        _lampEmissionController.Intensity = 1;
        _lampEmissionController.BlockedModeMix = 0;
        _lampEmissionController.DamageMix = 0;
        _lampEmissionController.IsDamageEnabled = false;
    }
    
    public void EnableBlockedMode()
    {
        _lampEmissionController.BlockedModeMix = _blockedModeStrength;
        isBlocked = true;
    }
    
    public void DisableBlockedMode()
    {
        _lampEmissionController.BlockedModeMix = 0;
        isBlocked = false;
    }
   
    private void StartAttackState(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _lampAttackAnimation.Play(attackDuration, false, currentPower, attackDistance);
    }
    
    private void StartBlockedAttackState(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _lampAttackAnimation.Play(attackDuration, true, currentPower, attackDistance);
    }
    
    private void PerformCooldownState(float currentPower)
    {
        _lampCooldown.PerformCooldown(currentPower, isBlocked);
    }
    
    public void StartIntroState(float introDuration, int currentHealth, int maxHealth)
    {
        _lampIntroAnimation.Play(introDuration, currentHealth, maxHealth);
    }
    
    public void StartDeathState(float deathStateDuration)
    {
        _lampDeathAnimation.Play(deathStateDuration);
    }
    
    public void StartDamageState()
    {
        _lampDamageAnimation.Play(_lightDamageDuration);
    }
}
