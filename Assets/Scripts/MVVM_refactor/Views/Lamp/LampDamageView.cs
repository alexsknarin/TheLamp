using UnityEngine;

public class LampDamageView : MonoBehaviour
{
    [SerializeField] private LampDamageAnimation _lampDamageAnimation;
    
    private PlayerGameplayViewModel _playerGameplayViewModel;

    public void Construct(PlayerGameplayViewModel playerGameplayViewModel)
    {
        _playerGameplayViewModel = playerGameplayViewModel;
        
        _playerGameplayViewModel.OnLampDamagedEvent += ShowDamageEffect;
        _playerGameplayViewModel.OnLampDeadEvent += ShowDeathEffect;
        _lampDamageAnimation.OnFinishedEvent += _playerGameplayViewModel.HandleDamageStateEnded;
    }

    private void OnDestroy()
    {
        _playerGameplayViewModel.OnLampDamagedEvent -= ShowDamageEffect;
        _playerGameplayViewModel.OnLampDeadEvent -= ShowDeathEffect;
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
}
