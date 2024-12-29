using UnityEngine;
using UnityEngine.UI;

public class PlayerGameplayViewUI : MonoBehaviour, IInitializable
{
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _restartButton;
    
    private PlayerGameplayViewModel _playerGameplayViewModel;
    
    public void Construct(PlayerGameplayViewModel viewModel)
    {
        _playerGameplayViewModel = viewModel;
    }

    public void Initialize()
    {
        _exitButton.onClick.AddListener(_playerGameplayViewModel.HandleExitButtonClicked);
        _restartButton.onClick.AddListener(_playerGameplayViewModel.HandleRestartButtonClicked);
    }
    
    private void OnDestroy()
    {
        _exitButton.onClick.RemoveListener(_playerGameplayViewModel.HandleExitButtonClicked);
        _restartButton.onClick.RemoveListener(_playerGameplayViewModel.HandleRestartButtonClicked);
    }
}
