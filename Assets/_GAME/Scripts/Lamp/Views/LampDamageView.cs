using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;
using _GAME.Scripts.Lib.Interfaces;
using _GAME.Scripts.UI.ViewModels;
using UnityEngine;

namespace _GAME.Scripts.Lamp.Views
{
    public class LampDamageView : MonoBehaviour, IInitializable
    {
        [SerializeField] private LampDamageAnimation _lampDamageAnimation;
        [SerializeField] private LampEmissionController _lampEmissionController;
    
        private PlayerGameplayViewModel _playerGameplayViewModel;

        public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
        {
            _playerGameplayViewModel = playerGameplayViewModel;
        
            _playerGameplayViewModel.LampDamaged += OnLampDamaged;
            // _playerGameplayViewModel.LampDied += OnLampDied;
            _playerGameplayViewModel.LampGlassDamageChanged += OnLampGlassDamageChanged;
            _lampDamageAnimation.Finished += _playerGameplayViewModel.OnDamageStateEnded;
        }

        private void OnDestroy()
        {
            _playerGameplayViewModel.LampDamaged -= OnLampDamaged;
            // _playerGameplayViewModel.LampDied -= OnLampDied;
            _playerGameplayViewModel.LampGlassDamageChanged -= OnLampGlassDamageChanged;
            _lampDamageAnimation.Finished -= _playerGameplayViewModel.OnDamageStateEnded;
        }

        public void Initialize()
        {
            _lampDamageAnimation.Initialize();
        }

        /// <summary>
        /// Show Lamp Damage Effect
        /// </summary>
        /// <param name="duration"></param>
        private void OnLampDamaged(float duration)
        {
            _lampDamageAnimation.Play(duration);
        }

        /// <summary>
        /// Show Lamp Death Effect
        /// </summary>
        private void OnLampDied()
        {
            _lampDamageAnimation.Play(0.2f); // TODO: magic number
        }

        private void OnLampGlassDamageChanged(GlassDamageData data)
        {
            _lampEmissionController.LampGlassDamageUpdate(data);
        }
    }
}
