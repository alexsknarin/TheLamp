using UnityEngine;

public class GameStageView : MonoBehaviour, IInitializable
{
    [SerializeField] private IntroGameStageAnimationController _introGameStageAnimationController;
    [SerializeField] private PrepareInGameStageAnimationController _prepareInGameStageAnimationController;
    [SerializeField] private PrepareOutGameStageAnimationController _prepareOutGameStageAnimationController;
    [SerializeField] private GameOverInGameStageAnimationController _gameOverInGameStageAnimationController;
    [SerializeField] private GameOverOutGameStageAnimationController _gameOverOutGameStageAnimationController;
    [SerializeField] private AdvertisementAnimationController _advertisementAnimationController;
    GameStageViewModel _gameStageViewModel;

    public void Initialize()
    {
        _introGameStageAnimationController.Initialize();
        _gameOverInGameStageAnimationController.Initialize();
        _advertisementAnimationController.Initialize();
    }

    public void Bind(GameStageViewModel viewModel)
    {
        _gameStageViewModel = viewModel;
        _gameStageViewModel.IntroStarted += OnIntroStarted;
        _gameStageViewModel.PrepareInStarted += OnPrepareInStarted;
        _gameStageViewModel.PrepareOutStarted += OnPrepareOutStarted;
        _gameStageViewModel.GameOverInStarted += OnGameOverInStarted;
        _gameStageViewModel.GameOverOutStarted += OnGameOverOutStarted;
        _gameStageViewModel.AdvertisementStarted += OnAdvertisementStarted;
        
        _introGameStageAnimationController.IntroFinished += OnIntroFinished;
        _prepareInGameStageAnimationController.PrepareInFinished += OnPrepareInFinished;
        _prepareOutGameStageAnimationController.PrepareOutFinished += OnPrepareOutFinished;
        _gameOverInGameStageAnimationController.GameoverInFinished += OnGameoverInFinished;
        _gameOverOutGameStageAnimationController.GameoverOutFinished += OnGameoverOutFinished;
        _advertisementAnimationController.AdvertisementFinished += OnAdvertisementFinished;
    }

    private void OnDestroy()
    {
        _gameStageViewModel.IntroStarted -= OnIntroStarted;
        _gameStageViewModel.PrepareInStarted -= OnPrepareInStarted;
        _gameStageViewModel.PrepareOutStarted -= OnPrepareOutStarted;
        _gameStageViewModel.GameOverInStarted -= OnGameOverInStarted;
        _gameStageViewModel.GameOverOutStarted -= OnGameOverOutStarted;
        _gameStageViewModel.AdvertisementStarted -= OnAdvertisementStarted;
        
        _introGameStageAnimationController.IntroFinished -= OnIntroFinished;
        _prepareInGameStageAnimationController.PrepareInFinished -= OnPrepareInFinished;
        _prepareOutGameStageAnimationController.PrepareOutFinished -= OnPrepareOutFinished;
        _gameOverInGameStageAnimationController.GameoverInFinished -= OnGameoverInFinished;
        _gameOverOutGameStageAnimationController.GameoverOutFinished -= OnGameoverOutFinished;
        _advertisementAnimationController.AdvertisementFinished += OnAdvertisementFinished;
    }

    // View Model Event Handlers
    private void OnIntroStarted(float normalizedHealth)
    {
        _introGameStageAnimationController.Play(normalizedHealth);
    }

    private void OnPrepareInStarted(bool isUpgradeUiRequired, int waveNum)
    {
        _prepareInGameStageAnimationController.Play(isUpgradeUiRequired, waveNum);
    }

    private void OnPrepareOutStarted()
    {
        _prepareOutGameStageAnimationController.Play();   
    }

    private void OnGameOverInStarted(Vector3 enemyPosition)
    {
        _gameOverInGameStageAnimationController.Play(enemyPosition);
    }

    private void OnGameOverOutStarted()
    {
        _gameOverOutGameStageAnimationController.Play();
    }

    private void OnAdvertisementStarted()
    {
        _advertisementAnimationController.Play();
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

    private void OnAdvertisementFinished()
    {
        _gameStageViewModel.HandleAdvertisementEnd();
    }
}
