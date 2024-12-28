using UnityEngine;

public class LampAttackView : MonoBehaviour, IInitializable
{
    [SerializeField] private GameObject _lampAttackZoneObject;
    [SerializeField] private AttackDistanceUpgradeAnimationController _attackDistanceUpgradeAnimationController;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _emissionPowerCurve;
    [SerializeField] private AnimationCurve _attackZonePowerCurve;
    private Material _lampAttackZoneMaterial;
    
    private bool _isPlaying = false;
    private float _localTime;
    private float _duration;
    private float _lightPower;
    private float _attackZonePower;
    private bool _isBlockedAttack = false;
    
    private PlayerGameplayViewModel _playerGameplayViewModel;
    private IGameConfigService _gameConfigService;
    
    public void Construct(PlayerGameplayViewModel playerGameplayViewModel, IGameConfigService gameConfigService)
    {
        _playerGameplayViewModel = playerGameplayViewModel;
        _gameConfigService = gameConfigService;
        _playerGameplayViewModel.OnAttackStartEvent += AttackStart;
        _playerGameplayViewModel.AttackDistance.OnChangedEvent += UpdateAttackZoneRadius;
    }

    public void Initialize()
    {
        _lampAttackZoneMaterial = _lampAttackZoneObject.GetComponent<MeshRenderer>().material;
        _attackDistanceUpgradeAnimationController.Initialize();
    }

    private void OnDestroy()
    {
        _playerGameplayViewModel.OnAttackStartEvent -= AttackStart;
        _playerGameplayViewModel.AttackDistance.OnChangedEvent -= UpdateAttackZoneRadius;
    }

    private void AttackStart(float power, bool isBlockedAttack)
    {
        _isBlockedAttack = isBlockedAttack;
        _lightPower = _emissionPowerCurve.Evaluate(power);
        if (!_isBlockedAttack)
            _attackZonePower = _attackZonePowerCurve.Evaluate(power);
        _duration = _gameConfigService.PlayerConfig.AttackDuration;
        _localTime = 0;
        _isPlaying = true;
    }

    private void PerformAttack()
    {
        float phase = _localTime / _duration;
        if (phase > 1)
        {
            _isPlaying = false;
            _lampEmissionController.Intensity = 0f;
            _lampAttackZoneMaterial.SetFloat("_Alpha", 0);
            return;
        }
        _lampEmissionController.Intensity = Mathf.Lerp(_lightPower, 0, phase);
        if (!_isBlockedAttack)
            _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(_attackZonePower, 0, phase));
        _localTime += Time.deltaTime;
    }

    private void UpdateAttackZoneRadius(object sender, Observable<float>.ChangedEventArgs e)
    {
        _attackDistanceUpgradeAnimationController.Play(_gameConfigService.PlayerConfig.AttackDistanceUpgradeAnimationTime);
    }

    void Update()
    {
        if (_isPlaying)
        {
            PerformAttack();
        }
    }
}
