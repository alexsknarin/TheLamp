using UnityEngine;

public class LampDamageViewUI : MonoBehaviour
{
    [SerializeField] private BrokenGlassEffect _brokenGlassEffect;
    private PlayerGameplayViewModel _playerGameplayViewModel;
    
    public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
    {
        _playerGameplayViewModel = playerGameplayViewModel;
        
        _playerGameplayViewModel.LampDamaged += OnLampDamaged;
        _playerGameplayViewModel.LampDied += OnLampDied;
    }

    private void OnDestroy()
    {
        _playerGameplayViewModel.LampDamaged -= OnLampDamaged;
        _playerGameplayViewModel.LampDied -= OnLampDied;
    }
    
    // Event Handle Methods
    private void OnLampDamaged(float duration)
    {
        _brokenGlassEffect.Play(BrokenGlassEventType.Damage); // TODO: replace with duration
    }

    private void OnLampDied()
    {
        _brokenGlassEffect.Play(BrokenGlassEventType.Death);
    }
}
