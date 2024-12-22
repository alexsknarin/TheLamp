using UnityEngine;

public class GameStateView : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    private GameStateViewModel _gameStateViewModel;
    
    public void Construct(GameStateViewModel viewModel)
    {
        _gameStateViewModel = viewModel;
        _gameState = _gameStateViewModel.GameState;        
    }
    
}
