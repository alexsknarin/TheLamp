using System;
using UnityEngine;

public class LampAttackView : MonoBehaviour, IInitializable
{
    [SerializeField] private GameObject _lampAttackZoneObject;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _emissionPowerCurve;
    [SerializeField] private AnimationCurve _attackZonePowerCurve;
    private Material _lampAttackZoneMaterial;
    
    private bool _isPlaying = false;
    private float _localTime;
    private float _duration;
    private float _lightPower;
    private float _attackZonePower;
    
    private PlayerWaveViewModel _playerWaveViewModel;
    private IGameConfigService _gameConfigService;
    
    public void Construct(PlayerWaveViewModel playerWaveViewModel, IGameConfigService gameConfigService)
    {
        _playerWaveViewModel = playerWaveViewModel;
        _gameConfigService = gameConfigService;
        _playerWaveViewModel.OnAttackStartEvent += AttackStart;
    }

    public void Initialize()
    {
        _lampAttackZoneMaterial = _lampAttackZoneObject.GetComponent<MeshRenderer>().material;
    }

    private void OnDestroy()
    {
        _playerWaveViewModel.OnAttackStartEvent -= AttackStart;
    }

    private void AttackStart(float power)
    {
        _lightPower = _emissionPowerCurve.Evaluate(power);
        _attackZonePower = _attackZonePowerCurve.Evaluate(power);
        _isPlaying = true;
        _duration = _gameConfigService.PlayerConfig.AttackDuration;
        _localTime = 0;
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
        _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(_attackZonePower, 0, phase));
        _localTime += Time.deltaTime;
    }

    void Update()
    {
        if (_isPlaying)
        {
            PerformAttack();
        }
    }
}
