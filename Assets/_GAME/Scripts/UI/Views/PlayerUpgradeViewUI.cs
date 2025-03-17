using _GAME.Scripts.Lib.Interfaces;
using _GAME.Scripts.UI.UiElements;
using _GAME.Scripts.UI.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace _GAME.Scripts.UI.Views
{
    public class PlayerUpgradeViewUI : MonoBehaviour, IInitializable
    {
        [SerializeField] private FadableButtonPresentation _healthButtonPresentation;
        [SerializeField] private FadableButtonPresentation _cooldownButtonPresentation;
        [SerializeField] private FadableButtonPresentation _attackDistanceButtonPresentation;
        [SerializeField] private UiUpgradePoints _uiUpgradePoints;
        private Button _healthButton;
        private Button _cooldownButton;
        private Button _attackDistanceButton;
    
        private PlayerUpgradeViewModel _playerUpgradeViewModel;
    
        public void Bind(PlayerUpgradeViewModel viewModel)
        {
            _playerUpgradeViewModel = viewModel;
        }

        public void Initialize()
        {
            _healthButton = _healthButtonPresentation.GetComponent<Button>();
            _cooldownButton = _cooldownButtonPresentation.GetComponent<Button>();
            _attackDistanceButton = _attackDistanceButtonPresentation.GetComponent<Button>();
        
            _healthButton.onClick.AddListener(OnHealthButtonClicked);
            _cooldownButton.onClick.AddListener(OnCooldownButtonClicked);
            _attackDistanceButton.onClick.AddListener(OnAttackDistanceButtonClicked);
        
            _playerUpgradeViewModel.HealthUpgradeEnabledChanged += OnHealthUpgradeEnabledChanged;
            _playerUpgradeViewModel.CooldownUpgradeEnabledChanged += OnCooldownUpgradeEnabledChanged;
            _playerUpgradeViewModel.AttackDistanceUpgradeEnabledChanged += OnAttackDistanceUpgradeEnabledChanged;
            _playerUpgradeViewModel.UpgradePointsChanged += OnUpgradePointsChanged;
        }

        private void OnDestroy()
        {
            _healthButton.onClick.RemoveListener(OnHealthButtonClicked);
            _cooldownButton.onClick.RemoveListener(OnCooldownButtonClicked);
            _attackDistanceButton.onClick.RemoveListener(OnAttackDistanceButtonClicked);
        
            _playerUpgradeViewModel.HealthUpgradeEnabledChanged -= OnHealthUpgradeEnabledChanged;
            _playerUpgradeViewModel.CooldownUpgradeEnabledChanged -= OnCooldownUpgradeEnabledChanged;
            _playerUpgradeViewModel.AttackDistanceUpgradeEnabledChanged -= OnAttackDistanceUpgradeEnabledChanged;
            _playerUpgradeViewModel.UpgradePointsChanged += OnUpgradePointsChanged;
        }

        private void OnHealthButtonClicked()
        {
            _playerUpgradeViewModel.HandleHealthButtonClicked();
        }

        private void OnCooldownButtonClicked()
        {
            _playerUpgradeViewModel.HandleCooldownButtonClicked();
        }

        private void OnAttackDistanceButtonClicked()
        {
            _playerUpgradeViewModel.HandleAttackDistanceButtonClicked();
        }


        // Event Handle Methods
        private void OnHealthUpgradeEnabledChanged(bool isEnabled)
        {
            // TODO: change to setEnabled to be able to set in 1 line
            if (isEnabled)
            {
                _healthButtonPresentation.EnableButton();
            }
            else
            {
                _healthButtonPresentation.DisableButton();
            }
        }

        private void OnCooldownUpgradeEnabledChanged(bool isEnabled)
        {
            // TODO: change to setEnabled to be able to set in 1 line
            if (isEnabled)
            {
                _cooldownButtonPresentation.EnableButton();
            }
            else
            {
                _cooldownButtonPresentation.DisableButton();
            }
        
        }

        private void OnAttackDistanceUpgradeEnabledChanged(bool isEnabled)
        {
            // TODO: change to setEnabled to be able to set in 1 line
            if (isEnabled)
            {
                _attackDistanceButtonPresentation.EnableButton();
            }
            else
            {
                _attackDistanceButtonPresentation.DisableButton();
            }
        }

        private void OnUpgradePointsChanged(int upgradePoints)
        {
            _uiUpgradePoints.ShowUpgradePoints(upgradePoints);
        }
    }
}
