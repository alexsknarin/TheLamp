using System;
using UnityEngine;
using UnityEngine.UI;

public class ConsentSettingsViewUI : MonoBehaviour, IInitializable
{
    [SerializeField] private GameObject _consentPanel;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private Button _enableDataCollectionButton;
    [SerializeField] private Button _disableDataCollectionButton;
    
    private GameSettingsViewModel _gameSettingsViewModel;

    public void Bind(GameSettingsViewModel gameSettingsViewModel)
    {
        _gameSettingsViewModel = gameSettingsViewModel;
    }

    public void Initialize()
    {
        _yesButton.onClick.AddListener(_gameSettingsViewModel.HandleYesInitialConsentButtonClicked);
        _noButton.onClick.AddListener(_gameSettingsViewModel.HandleNoInitialConsentButtonClicked);
        _enableDataCollectionButton.onClick.AddListener(_gameSettingsViewModel.HandleEnableDataCollectionButtonClicked);
        _disableDataCollectionButton.onClick.AddListener(_gameSettingsViewModel.HandleDisableDataCollectionButtonClicked);
        
        _gameSettingsViewModel.IsConsentSetView.Changed += OnIsConsentSetViewChanged;
        _gameSettingsViewModel.IsDataCollectionEnabledView.Changed += OnIsDataCollectionEnabledViewChanged;
        // Initial UI setup
        _consentPanel.SetActive(!_gameSettingsViewModel.IsConsentSetView.Value);
        _enableDataCollectionButton.gameObject.SetActive(!_gameSettingsViewModel.IsDataCollectionEnabledView.Value);
        _disableDataCollectionButton.gameObject.SetActive(_gameSettingsViewModel.IsDataCollectionEnabledView.Value);
    }

    private void OnDestroy()
    {
        _gameSettingsViewModel.IsConsentSetView.Changed -= OnIsConsentSetViewChanged;
        _gameSettingsViewModel.IsDataCollectionEnabledView.Changed -= OnIsDataCollectionEnabledViewChanged;
    }

    private void OnIsConsentSetViewChanged(object sender, Observable<bool>.ChangedEventArgs e)
    {
        _consentPanel.SetActive(!e.NewValue);
    }

    private void OnIsDataCollectionEnabledViewChanged(object sender, Observable<bool>.ChangedEventArgs e)
    {
        _enableDataCollectionButton.gameObject.SetActive(!e.NewValue);
        _disableDataCollectionButton.gameObject.SetActive(e.NewValue);
    }
}
