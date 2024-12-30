using System;
using UnityEngine;

public class LampCooldownView : MonoBehaviour, IInitializable
{
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _lampCooldownAnimCurve;
    [SerializeField] private AnimationCurve _lampNoiseAmountAnimCurve;
    [SerializeField] private MeshRenderer _lampAttackZoneRenderer;
    private Material _lampAttackZoneMaterial;
    
    private PlayerGameplayViewModel _playerGameplayViewModel;

    public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
    {
        _playerGameplayViewModel = playerGameplayViewModel;
        _playerGameplayViewModel.Power.OnChangedEvent += SetPower;
    }

    private void OnDestroy()
    {
        _playerGameplayViewModel.Power.OnChangedEvent += SetPower;
    }

    public void Initialize()
    {
        _lampAttackZoneMaterial = _lampAttackZoneRenderer.material;
    }

    public void SetPower(object sender, Observable<float>.ChangedEventArgs e)
    {
        float phase = e.NewValue;
        _lampEmissionController.Intensity = _lampCooldownAnimCurve.Evaluate(phase);
        // if (isBlocked)
        // {
        //     return;    
        // }
        _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(phase);
        _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(0, 0.005f, _lampCooldownAnimCurve.Evaluate(phase)));
    }
}
