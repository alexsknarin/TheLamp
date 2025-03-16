using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.UI.UiElements;
using _GAME.Scripts.UI.ViewModels;
using UnityEngine;

namespace _GAME.Scripts.UI.Views
{
    public class LampDamageViewUI : MonoBehaviour
    {
        [SerializeField] private BrokenGlassEffect _brokenGlassEffect;
        private PlayerGameplayViewModel _playerGameplayViewModel;
    
        public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
        {
            _playerGameplayViewModel = playerGameplayViewModel;
        
            _playerGameplayViewModel.LampDamaged += OnLampDamaged;
        }

        private void OnDestroy()
        {
            _playerGameplayViewModel.LampDamaged -= OnLampDamaged;
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
}
