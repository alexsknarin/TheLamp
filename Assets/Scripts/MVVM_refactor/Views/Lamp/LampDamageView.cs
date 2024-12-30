using UnityEngine;

public class LampDamageView : MonoBehaviour
{
    [SerializeField] private LampDamageAnimation _lampDamageAnimation;
    [SerializeField] private LampEmissionController _lampEmissionController;
    
    private PlayerGameplayViewModel _playerGameplayViewModel;

    public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
    {
        _playerGameplayViewModel = playerGameplayViewModel;
        
        _playerGameplayViewModel.OnLampDamagedEvent += ShowDamageEffect;
        _playerGameplayViewModel.OnLampDeadEvent += ShowDeathEffect;
        _playerGameplayViewModel.OnLampGlassDamageChangedEvent += SetLampGlassDamage;
        _lampDamageAnimation.OnFinishedEvent += _playerGameplayViewModel.HandleDamageStateEnded;
        
    }

    private void OnDestroy()
    {
        _playerGameplayViewModel.OnLampDamagedEvent -= ShowDamageEffect;
        _playerGameplayViewModel.OnLampDeadEvent -= ShowDeathEffect;
        _playerGameplayViewModel.OnLampGlassDamageChangedEvent -= SetLampGlassDamage;
        _lampDamageAnimation.OnFinishedEvent -= _playerGameplayViewModel.HandleDamageStateEnded;
    }

    private void ShowDamageEffect(float duration)
    {
        _lampDamageAnimation.Play(duration);
    }

    private void ShowDeathEffect()
    {
        _lampDamageAnimation.Play(0.2f); // TODO: magic number
    }

    private void SetLampGlassDamage(GlassDamageData data)
    {
        _lampEmissionController.LampGlassDamageUpdate(data);
    }
}
