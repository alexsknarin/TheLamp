using _GAME.Scripts.Lib;
using _GAME.Scripts.UI.ViewModels;
using UnityEngine;

namespace _GAME.Scripts.Lamp.Views
{
    public class LampHealthBarView : MonoBehaviour
    {
        [SerializeField] private LampHealthBarController _lampHealthBarController;
        PlayerGameplayViewModel _playerGameplayViewModel;
        public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
        {
            _playerGameplayViewModel = playerGameplayViewModel;
            _playerGameplayViewModel.LampNormalizedHealth.Changed += OnLampNormalizedHealthChanged;
            _playerGameplayViewModel.LastHealthPointStarted += OnLastHealthPointStarted;
            _playerGameplayViewModel.LastHealthPointEnded += OnLastHealthPointEnded;
            _playerGameplayViewModel.HealthUpgraded += OnHealthUpgraded;
        }
        private void OnDestroy()
        {
            _playerGameplayViewModel.LampNormalizedHealth.Changed -= OnLampNormalizedHealthChanged;
            _playerGameplayViewModel.LastHealthPointStarted -= OnLastHealthPointStarted;
            _playerGameplayViewModel.LastHealthPointEnded -= OnLastHealthPointEnded;
            _playerGameplayViewModel.HealthUpgraded -= OnHealthUpgraded;
        }
    
        // Event Handle Methods
        private void OnLampNormalizedHealthChanged(object sender, Observable<float>.ChangedEventArgs e)
        {
            _lampHealthBarController.SetHealth(e.NewValue);
        }

        private void OnLastHealthPointStarted()
        {
            _lampHealthBarController.EnableLastHealth();
        }
    
        private void OnLastHealthPointEnded()
        {
            _lampHealthBarController.DisableLastHealth();    
        }
    
        private void OnHealthUpgraded()
        {
            _lampHealthBarController.PlayUpgrade();
        }
    }
}
