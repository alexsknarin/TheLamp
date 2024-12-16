using UnityEngine;

public class GameStateView : MonoBehaviour
{
    // TODO: Make view have it's own copy of the gamestate and viewModel make a conversion so it couldn't change the original gamestate
    [SerializeField] private GameState _gameState;
    private GameStateViewModel _gameStateViewModel;
    
    public void Construct(GameStateViewModel viewModel)
    {
        _gameStateViewModel = viewModel;
        _gameState = _gameStateViewModel.GameState;        
    }
    
}
