using System;
using UnityEngine;

public class PlayerUpgradeViewModel : IDisposable
{
    // TODO: get settings service to get upgrade cap values

    #region HealthUpgradeEnabled
    private bool _healthUpgradeEnabled;
    public bool HealthUpgradeEnabled
    {
        get => _healthUpgradeEnabled;
        private set
        {
            _healthUpgradeEnabled = value;
            OnHealthUpgradeEnabledChangedEvent?.Invoke(value);
        }
    }
    public event Action<bool> OnHealthUpgradeEnabledChangedEvent; 
    #endregion
    
    #region HealthUpgradeEnabled
    private bool _cooldownUpgradeEnabled;
    public bool CooldownUpgradeEnabled
    {
        get => _cooldownUpgradeEnabled;
        private set
        {
            _cooldownUpgradeEnabled = value;
            OnCooldownUpgradeEnabledChangedEvent?.Invoke(value);
        }
    }
    public event Action<bool> OnCooldownUpgradeEnabledChangedEvent; 
    #endregion
    
    #region HealthUpgradeEnabled
    private bool _attackDistanceUpgradeEnabled;
    public bool AttackDistanceUpgradeEnabled
    {
        get => _attackDistanceUpgradeEnabled;
        private set
        {
            _attackDistanceUpgradeEnabled = value;
            OnAttackDistanceUpgradeEnabledChangedEvent?.Invoke(value);
        }
    }
    public event Action<bool> OnAttackDistanceUpgradeEnabledChangedEvent; 
    #endregion
    
    private GameModel _gameModel;
    private IGameConfigService _gameConfigService;

    public PlayerUpgradeViewModel(GameModel gameModel, IGameConfigService gameConfigService)
    {
        _gameModel = gameModel;
        _gameConfigService = gameConfigService;
        
        _gameModel.OnGameStageStateChangedEvent += HandleStartUpgrade;
        
        _gameModel.OnLampHealthChangedEvent += HandleHealthChanged;
        _gameModel.OnLampCooldownTimeChangedEvent += HandleCooldownChanged;
        _gameModel.OnLampAttackDistanceChangedEvent += HandleAttackDistanceChanged;
        
        _gameModel.OnUpgradePointsChangedEvent += HandleUpgradePointsChanged;
    }

    public void Dispose()
    {
        _gameModel.OnGameStageStateChangedEvent -= HandleStartUpgrade;
        _gameModel.OnLampHealthChangedEvent -= HandleHealthChanged;
        _gameModel.OnLampCooldownTimeChangedEvent -= HandleCooldownChanged;
        _gameModel.OnLampAttackDistanceChangedEvent -= HandleAttackDistanceChanged;
        _gameModel.OnUpgradePointsChangedEvent -= HandleUpgradePointsChanged;
    }

    private void HandleStartUpgrade(GameStageState stageState)
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
    }

    private void HandleHealthChanged(int health)
    {
        // Disable button if healthcap is reached or no upgrade points
        if(_gameModel.LampHealth >= _gameConfigService.PlayerConfig.HealthCap || _gameModel.UpgradePoints <= 0)
        {
            HealthUpgradeEnabled = false;
            return;
        }
        HealthUpgradeEnabled = true;
    }

    private void HandleCooldownChanged(float cooldownTime)
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

    private void HandleAttackDistanceChanged(float attackDistance)
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

    private void HandleUpgradePointsChanged(int obj)
    {
        // Disable all buttons if no upgrade points
        if(_gameModel.UpgradePoints <= 0)
        {
            HealthUpgradeEnabled = false;
            CooldownUpgradeEnabled = false;
            AttackDistanceUpgradeEnabled = false;
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
}
