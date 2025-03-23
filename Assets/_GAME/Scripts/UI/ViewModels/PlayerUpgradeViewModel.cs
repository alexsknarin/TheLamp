using System;
using _GAME.Scripts.GameCoreSystems;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using IDisposable = _GAME.Scripts.Lib.Interfaces.IDisposable;

namespace _GAME.Scripts.UI.ViewModels
{
    public class PlayerUpgradeViewModel : IDisposable
    {
        private bool _healthUpgradeEnabled;
        private bool _cooldownUpgradeEnabled;
        private bool _attackDistanceUpgradeEnabled;
        private int _upgradePoints;
        private GameModel _gameModel;
        private IGameConfigService _gameConfigService;

        public PlayerUpgradeViewModel(GameModel gameModel, IGameConfigService gameConfigService)
        {
            _gameModel = gameModel;
            _gameConfigService = gameConfigService;
        
            _gameModel.GameStageStateChanged += OnGameStageStateChanged;
            _gameModel.LampHealthChanged += OnLampHealthChanged;
            _gameModel.LampCooldownTimeChanged += OnLampCooldownTimeChanged;
            _gameModel.LampAttackDistanceChanged += OnLampAttackDistanceChanged;
            _gameModel.UpgradePointsChanged += OnUpgradePointsChanged;
        }

        public void Dispose()
        {
            _gameModel.GameStageStateChanged -= OnGameStageStateChanged;
            _gameModel.LampHealthChanged -= OnLampHealthChanged;
            _gameModel.LampCooldownTimeChanged -= OnLampCooldownTimeChanged;
            _gameModel.LampAttackDistanceChanged -= OnLampAttackDistanceChanged;
            _gameModel.UpgradePointsChanged -= OnUpgradePointsChanged;
        }

        public event Action<bool> HealthUpgradeEnabledChanged;
        public event Action<bool> CooldownUpgradeEnabledChanged;
        public event Action<bool> AttackDistanceUpgradeEnabledChanged;
        public event Action<int> UpgradePointsChanged;
    
        public bool HealthUpgradeEnabled
        {
            get => _healthUpgradeEnabled;
            private set
            {
                _healthUpgradeEnabled = value;
                HealthUpgradeEnabledChanged?.Invoke(value);
            }
        }
        public bool CooldownUpgradeEnabled
        {
            get => _cooldownUpgradeEnabled;
            private set
            {
                _cooldownUpgradeEnabled = value;
                CooldownUpgradeEnabledChanged?.Invoke(value);
            }
        }
        public bool AttackDistanceUpgradeEnabled
        {
            get => _attackDistanceUpgradeEnabled;
            private set
            {
                _attackDistanceUpgradeEnabled = value;
                AttackDistanceUpgradeEnabledChanged?.Invoke(value);
            }
        }
        public int UpgradePoints
        {
            get => _upgradePoints;
            private set
            {
                _upgradePoints = value;
                UpgradePointsChanged?.Invoke(value);
            }
        }
    
    
        // Calls from view TODO: change Handle in a name to something else

        public void HandleHealthButtonClicked()
        {
            _gameModel.HandleHealthUpgrade();
        }

        public void HandleCooldownButtonClicked()
        {
            _gameModel.HandleCooldownUpgrade();
        }

        public void HandleAttackDistanceButtonClicked()
        {
            _gameModel.HandleAttackDistanceUpgrade();
        }
    
    
        // Event Handle Methods
        /// <summary>
        /// Handle Start onf the stage
        /// </summary>
        /// <param name="stageState"></param>
        private void OnGameStageStateChanged(GameStageState stageState)
        {
            if(stageState != GameStageState.Prepare)
                return;
        
            // Check Health if button can be enabled
            HealthUpgradeEnabled = false;
            if(_gameModel.LampHealth < _gameConfigService.PlayerConfig.HealthCap && _gameModel.UpgradePoints > 0)
            {
                HealthUpgradeEnabled = true;
            }
        
            // Check Cooldown if button can be enabled
            CooldownUpgradeEnabled = false;
            if(_gameModel.LampCooldownTime > _gameConfigService.PlayerConfig.CooldownTimeCap &&
               !Mathf.Approximately(_gameModel.LampCooldownTime, _gameConfigService.PlayerConfig.CooldownTimeCap) &&
               _gameModel.UpgradePoints > 0)
            {
                CooldownUpgradeEnabled = true;
            }
        
            // Check Attack Distance if button can be enabled
            AttackDistanceUpgradeEnabled = false;
            if(_gameModel.LampAttackDistance < _gameConfigService.PlayerConfig.AttackDistanceCap && _gameModel.UpgradePoints > 0)
            {
                AttackDistanceUpgradeEnabled = true;
            }
        
            // Check Upgrade Points
            UpgradePoints = _gameModel.UpgradePoints;
        }

        private void OnLampHealthChanged(int health)
        {
            // Disable button if healthcap is reached or no upgrade points
            if(_gameModel.LampHealth >= _gameConfigService.PlayerConfig.HealthCap || _gameModel.UpgradePoints <= 0)
            {
                HealthUpgradeEnabled = false;
                return;
            }
            HealthUpgradeEnabled = true;
        }

        private void OnLampCooldownTimeChanged(float cooldownTime)
        {
            // Disable button if cooldowncap is reached or no upgrade points
            if(_gameModel.LampCooldownTime < _gameConfigService.PlayerConfig.CooldownTimeCap ||
               Mathf.Approximately(_gameModel.LampCooldownTime, _gameConfigService.PlayerConfig.CooldownTimeCap) ||
               _gameModel.UpgradePoints <= 0)
            {
                CooldownUpgradeEnabled = false;
                return;
            }
            CooldownUpgradeEnabled = true;
        }

        private void OnLampAttackDistanceChanged(float attackDistance)
        {
            // Disable button if attackDistanceCap is reached or no upgrade points
            if (_gameModel.LampAttackDistance > _gameConfigService.PlayerConfig.AttackDistanceCap ||
                Mathf.Approximately(_gameModel.LampAttackDistance, _gameConfigService.PlayerConfig.AttackDistanceCap) ||
                _gameModel.UpgradePoints <= 0)
            {
                AttackDistanceUpgradeEnabled = false;
                return;
            }
        
            AttackDistanceUpgradeEnabled = true;
        }

        private void OnUpgradePointsChanged(int upgradePoints)
        {
            // Disable all buttons if no upgrade points
            if(upgradePoints <= 0)
            {
                HealthUpgradeEnabled = false;
                CooldownUpgradeEnabled = false;
                AttackDistanceUpgradeEnabled = false;
                UpgradePoints = upgradePoints;
            }
            else
            {
                UpgradePoints = upgradePoints;    
            }
        }
    }
}
