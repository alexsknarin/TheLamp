using System;
using UnityEngine;

public class GameStateView : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    private GameStateViewModel _gameStateViewModel;
    
    public void Construct(GameStateViewModel viewModel)
    {
        _gameStateViewModel = viewModel;
        _gameState = _gameStateViewModel.CurrentGameState;        
        
        _gameStateViewModel.OnGameStateChangedEvent += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        _gameStateViewModel.OnGameStateChangedEvent -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState gameState)
    {
        _gameState = gameState;
    }
}
