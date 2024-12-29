using UnityEngine;
using UnityEngine.UI;

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
    
    public void Construct(PlayerUpgradeViewModel viewModel)
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
        
        _playerUpgradeViewModel.OnHealthUpgradeEnabledChangedEvent += HandleHealthButtonEnabled;
        _playerUpgradeViewModel.OnCooldownUpgradeEnabledChangedEvent += HandleCooldownButtonEnabled;
        _playerUpgradeViewModel.OnAttackDistanceUpgradeEnabledChangedEvent += HandleAttackDistanceButtonEnabled;
        _playerUpgradeViewModel.OnUpgradePointsChangedEvent += HandleUpgradePointsChanged;
    }

    private void OnDestroy()
    {
        _healthButton.onClick.RemoveListener(OnHealthButtonClicked);
        _cooldownButton.onClick.RemoveListener(OnCooldownButtonClicked);
        _attackDistanceButton.onClick.RemoveListener(OnAttackDistanceButtonClicked);
        
        _playerUpgradeViewModel.OnHealthUpgradeEnabledChangedEvent -= HandleHealthButtonEnabled;
        _playerUpgradeViewModel.OnCooldownUpgradeEnabledChangedEvent -= HandleCooldownButtonEnabled;
        _playerUpgradeViewModel.OnAttackDistanceUpgradeEnabledChangedEvent -= HandleAttackDistanceButtonEnabled;
        _playerUpgradeViewModel.OnUpgradePointsChangedEvent += HandleUpgradePointsChanged;
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


    // Event Handlers

    private void HandleHealthButtonEnabled(bool isEnabled)
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

    private void HandleCooldownButtonEnabled(bool isEnabled)
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

    private void HandleAttackDistanceButtonEnabled(bool isEnabled)
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

    private void HandleUpgradePointsChanged(int upgradePoints)
    {
        Debug.Log("+++++++++++++++++UPGRADE POINT WAS USED!!!!");
        _uiUpgradePoints.ShowUpgradePoints(upgradePoints);
    }
}
