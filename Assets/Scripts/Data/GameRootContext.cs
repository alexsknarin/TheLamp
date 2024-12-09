using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameRootContext : MonoBehaviour
{
    [SerializeField] private ConsentSettingsUIView _consentSettingsUIView;
    private List<IDisposable> _disposables = new List<IDisposable>();
    
    private void Awake()
    {
        Debug.Log("------------------------------------------");
        Debug.Log("------ Starting Game Initialization ------");
        Debug.Log("------ Game Settings Initialization ------");
        // TODO: Make game services as fields ????
        IGameSettingsProvider gameSettingsProvider = new PlayerPrefsGameSettingsProvider();
        GameSettingsService gameSettingsService = new GameSettingsService(gameSettingsProvider); // TODO: potentially long operation - consider async and loading screen
        GameSettingsModel gameSettingsModel = new GameSettingsModel(gameSettingsProvider.Get(), gameSettingsService);
        GameSettingsViewModel gameSettingsViewModel = new GameSettingsViewModel(gameSettingsModel);
        _disposables.Add(gameSettingsViewModel);
        gameSettingsViewModel.Initialize();
        
        Debug.Log("------ UI Initialization ------");
        _consentSettingsUIView.Bind(gameSettingsViewModel);
        _consentSettingsUIView.Initialize();
    }
    
    
    private void OnDestroy()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }
}
