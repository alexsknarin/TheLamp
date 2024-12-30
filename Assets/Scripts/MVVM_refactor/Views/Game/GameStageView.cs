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

    public void Construct(GameStageViewModel viewModel)
    {
        _gameStageViewModel = viewModel;
        _gameStageViewModel.OnIntroStartedEvent += StartIntro;
        _gameStageViewModel.OnPrepareInStartedEvent += StartPrepareIn;
        _gameStageViewModel.OnPrepareOutStartedEvent += StartPrepareOut;
        _gameStageViewModel.OnGameOverInStartedEvent += StartGameOverIn;
        _gameStageViewModel.OnGameOverOutStartedEvent += StartGameOverOut;
        _gameStageViewModel.OnAdvertisementStartedEvent += StartAdvertisement;
        
        _introGameStageAnimationController.OnFinishedEvent += HandleIntroEnd;
        _prepareInGameStageAnimationController.OnFinishedEvent += HandlePrepareInEnd;
        _prepareOutGameStageAnimationController.OnFinishedEvent += HandlePrepareOutEnd;
        _gameOverInGameStageAnimationController.OnFinishedEvent += HandleGameOverInEnd;
        _gameOverOutGameStageAnimationController.OnFinishedEvent += HandleGameOverOutEnd;
        _advertisementAnimationController.OnFinishedEvent += HandleAdvertisementEnd;
    }

    private void OnDestroy()
    {
        _gameStageViewModel.OnIntroStartedEvent -= StartIntro;
        _gameStageViewModel.OnPrepareInStartedEvent -= StartPrepareIn;
        _gameStageViewModel.OnPrepareOutStartedEvent -= StartPrepareOut;
        _gameStageViewModel.OnGameOverInStartedEvent -= StartGameOverIn;
        _gameStageViewModel.OnGameOverOutStartedEvent -= StartGameOverOut;
        _gameStageViewModel.OnAdvertisementStartedEvent -= StartAdvertisement;
        
        _introGameStageAnimationController.OnFinishedEvent -= HandleIntroEnd;
        _prepareInGameStageAnimationController.OnFinishedEvent -= HandlePrepareInEnd;
        _prepareOutGameStageAnimationController.OnFinishedEvent -= HandlePrepareOutEnd;
        _gameOverInGameStageAnimationController.OnFinishedEvent -= HandleGameOverInEnd;
        _gameOverOutGameStageAnimationController.OnFinishedEvent -= HandleGameOverOutEnd;
        _advertisementAnimationController.OnFinishedEvent += HandleAdvertisementEnd;
    }


    public void StartIntro(float normalizedHealth)
    {
        _introGameStageAnimationController.Play(normalizedHealth);
    }

    private void StartPrepareIn(bool isUpgradeUiRequired, int waveNum)
    {
        _prepareInGameStageAnimationController.Play(isUpgradeUiRequired, waveNum);
    }

    private void StartPrepareOut()
    {
        _prepareOutGameStageAnimationController.Play();   
    }

    private void StartGameOverIn(Vector3 enemyPosition)
    {
        _gameOverInGameStageAnimationController.Play(enemyPosition);
    }

    private void StartGameOverOut()
    {
        _gameOverOutGameStageAnimationController.Play();
    }

    private void StartAdvertisement()
    {
        _advertisementAnimationController.Play();
    }


    // Event Handlers

    public void HandleIntroEnd()
    {
        _gameStageViewModel.HandleIntroEnd();    
    }

    public void HandlePrepareInEnd()
    {
        _gameStageViewModel.HandlePrepareInEnd();    
    }

    public void HandlePrepareOutEnd()
    {
        _gameStageViewModel.HandlePrepareOutEnd();    
    }

    private void HandleGameOverInEnd()
    {
        _gameStageViewModel.HandleGameOverInEnd();
    }

    private void HandleGameOverOutEnd()
    {
        _gameStageViewModel.HandleGameOverOutEnd();
    }

    private void HandleAdvertisementEnd()
    {
        _gameStageViewModel.HandleAdvertisementEnd();
    }
}
