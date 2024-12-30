using UnityEngine;

public class LampHealthBarView : MonoBehaviour
{
    [SerializeField] private LampHealthBarController _lampHealthBarController;
    PlayerGameplayViewModel _playerGameplayViewModel;
    public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
    {
        _playerGameplayViewModel = playerGameplayViewModel;
        _playerGameplayViewModel.LampNormalizedHealth.OnChangedEvent += UpdateHealth;
        _playerGameplayViewModel.OnLastHealthPointStartedEvent += EnableLastHealth;
        _playerGameplayViewModel.OnLastHealthPointEndedEvent += DisableLastHealth;
        _playerGameplayViewModel.OnHealthUpgradedEvent += PlayUpgrade;
    }
    private void OnDestroy()
    {
        _playerGameplayViewModel.LampNormalizedHealth.OnChangedEvent -= UpdateHealth;
        _playerGameplayViewModel.OnLastHealthPointStartedEvent -= EnableLastHealth;
        _playerGameplayViewModel.OnLastHealthPointEndedEvent -= DisableLastHealth;
        _playerGameplayViewModel.OnHealthUpgradedEvent -= PlayUpgrade;
    }

    private void UpdateHealth(object sender, Observable<float>.ChangedEventArgs e)
    {
        _lampHealthBarController.SetHealth(e.NewValue);
    }

    private void EnableLastHealth()
    {
        _lampHealthBarController.EnableLastHealth();
    }
    
    private void DisableLastHealth()
    {
        _lampHealthBarController.DisableLastHealth();    
    }
    
    private void PlayUpgrade()
    {
        _lampHealthBarController.PlayUpgrade();
    }
}
