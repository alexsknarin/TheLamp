using UnityEngine;

public class LampDamageViewUI : MonoBehaviour
{
    [SerializeField] private BrokenGlassEffect _brokenGlassEffect;
    private PlayerGameplayViewModel _playerGameplayViewModel;
    
    public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
    {
        _playerGameplayViewModel = playerGameplayViewModel;
        
        _playerGameplayViewModel.OnLampDamagedEvent += ShowDamageEffect;
        _playerGameplayViewModel.OnLampDeadEvent += ShowDeathEffect;
    }

    private void OnDestroy()
    {
        _playerGameplayViewModel.OnLampDamagedEvent -= ShowDamageEffect;
        _playerGameplayViewModel.OnLampDeadEvent -= ShowDeathEffect;
    }

    private void ShowDamageEffect(float duration)
    {
        _brokenGlassEffect.Play(BrokenGlassEventType.Damage); // TODO: replace with duration
    }

    private void ShowDeathEffect()
    {
        _brokenGlassEffect.Play(BrokenGlassEventType.Death);
    }
}
