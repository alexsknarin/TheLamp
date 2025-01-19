using UnityEngine;

public class GameStageView : MonoBehaviour, IInitializable
{
    [SerializeField] private IntroGameStageAnimationController _introGameStageAnimationController;
    [SerializeField] private PrepareInGameStageAnimationController _prepareInGameStageAnimationController;
    [SerializeField] private PrepareOutGameStageAnimationController _prepareOutGameStageAnimationController;
    [SerializeField] private GameOverInGameStageAnimationController _gameOverInGameStageAnimationController;
    [SerializeField] private GameOverOutGameStageAnimationController _gameOverOutGameStageAnimationController;
    [SerializeField] private AdvertisementStageAnimation _advertisementStageAnimation;
    GameStageViewModel _gameStageViewModel;

    public void Initialize()
    {
        _introGameStageAnimationController.Initialize();
        _gameOverInGameStageAnimationController.Initialize();
    }

    public void Bind(GameStageViewModel viewModel)
    {
        _gameStageViewModel = viewModel;
        _gameStageViewModel.IntroStarted += OnIntroStarted;
        _gameStageViewModel.PrepareInStarted += OnPrepareInStarted;
        _gameStageViewModel.PrepareOutStarted += OnPrepareOutStarted;
        _gameStageViewModel.GameOverInStarted += OnGameOverInStarted;
        _gameStageViewModel.GameOverOutStarted += OnGameOverOutStarted;
        _gameStageViewModel.AdvertisementStarted += _advertisementStageAnimation.Play;
        
        _introGameStageAnimationController.IntroFinished += OnIntroFinished;
        _prepareInGameStageAnimationController.PrepareInFinished += OnPrepareInFinished;
        _prepareOutGameStageAnimationController.PrepareOutFinished += OnPrepareOutFinished;
        _gameOverInGameStageAnimationController.GameoverInFinished += OnGameoverInFinished;
        _gameOverOutGameStageAnimationController.GameoverOutFinished += OnGameoverOutFinished;
    }

    private void OnDestroy()
    {
        _gameStageViewModel.IntroStarted -= OnIntroStarted;
        _gameStageViewModel.PrepareInStarted -= OnPrepareInStarted;
        _gameStageViewModel.PrepareOutStarted -= OnPrepareOutStarted;
        _gameStageViewModel.GameOverInStarted -= OnGameOverInStarted;
        _gameStageViewModel.GameOverOutStarted -= OnGameOverOutStarted;
        _gameStageViewModel.AdvertisementStarted += _advertisementStageAnimation.Play;
        
        _introGameStageAnimationController.IntroFinished -= OnIntroFinished;
        _prepareInGameStageAnimationController.PrepareInFinished -= OnPrepareInFinished;
        _prepareOutGameStageAnimationController.PrepareOutFinished -= OnPrepareOutFinished;
        _gameOverInGameStageAnimationController.GameoverInFinished -= OnGameoverInFinished;
        _gameOverOutGameStageAnimationController.GameoverOutFinished -= OnGameoverOutFinished;
    }

    // View Model Event Handlers
    private void OnIntroStarted(bool skipStage, float duration, float normalizedHealth)
    {
        _introGameStageAnimationController.Play(skipStage, duration, normalizedHealth);
    }

    private void OnPrepareInStarted(bool skipStage, float duration, bool isUpgradeUiRequired, int waveNum)
    {
        _prepareInGameStageAnimationController.Play(skipStage, duration, isUpgradeUiRequired, waveNum);
    }

    private void OnPrepareOutStarted(bool skipStage, float duration)
    {
        _prepareOutGameStageAnimationController.Play(skipStage, duration);   
    }

    private void OnGameOverInStarted(bool skipStage, bool isAdNeeded, float duration, Vector3 enemyPosition)
    {
        _gameOverInGameStageAnimationController.Play(skipStage, isAdNeeded, duration, enemyPosition);
    }

    private void OnGameOverOutStarted(bool skipStage, float duration)
    {
        _gameOverOutGameStageAnimationController.Play(skipStage, duration);
    }

    // Animation controllers Event Handlers
    private void OnIntroFinished()
    {
        _gameStageViewModel.HandleIntroEnd();    
    }

    private void OnPrepareInFinished()
    {
        _gameStageViewModel.HandlePrepareInEnd();    
    }

    private void OnPrepareOutFinished()
    {
        _gameStageViewModel.HandlePrepareOutEnd();    
    }

    private void OnGameoverInFinished()
    {
        _gameStageViewModel.HandleGameOverInEnd();
    }

    private void OnGameoverOutFinished()
    {
        _gameStageViewModel.HandleGameOverOutEnd();
    }
}
