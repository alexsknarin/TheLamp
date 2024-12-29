using UnityEngine;

public class GameStageView : MonoBehaviour, IInitializable
{
    [SerializeField] private IntroGameStageAnimationController _introGameStageAnimationController;
    [SerializeField] private PrepareInGameStageAnimationController _prepareInGameStageAnimationController;
    [SerializeField] private PrepareOutGameStageAnimationController _prepareOutGameStageAnimationController;
    [SerializeField] private GameOverInGameStageAnimationController _gameOverInGameStageAnimationController;
    GameStageViewModel _gameStageViewModel;
    
    public void Construct(GameStageViewModel viewModel)
    {
        _gameStageViewModel = viewModel;
        _gameStageViewModel.OnIntroStartedEvent += StartIntro;
        _gameStageViewModel.OnPrepareInStartedEvent += StartPrepareIn;
        _gameStageViewModel.OnPrepareOutStartedEvent += StartPrepareOut;
        _gameStageViewModel.OnGameOverInStartedEvent += StartGameOverIn;
        
        _introGameStageAnimationController.OnFinishedEvent += HandleIntroEnd;
        _prepareInGameStageAnimationController.OnFinishedEvent += HandlePrepareInEnd;
        _prepareOutGameStageAnimationController.OnFinishedEvent += HandlePrepareOutEnd;
    }
    
    public void Initialize()
    {
        _introGameStageAnimationController.Initialize();
        _gameOverInGameStageAnimationController.Initialize();
    }

    private void OnDestroy()
    {
        _gameStageViewModel.OnIntroStartedEvent -= StartIntro;
        _gameStageViewModel.OnPrepareInStartedEvent -= StartPrepareIn;
        _gameStageViewModel.OnPrepareOutStartedEvent -= StartPrepareOut;
        _introGameStageAnimationController.OnFinishedEvent -= HandleIntroEnd;
        _prepareInGameStageAnimationController.OnFinishedEvent -= HandlePrepareInEnd;
        _prepareOutGameStageAnimationController.OnFinishedEvent -= HandlePrepareOutEnd;
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
    
    private void StartGameOverIn()
    {
        _gameOverInGameStageAnimationController.Play();
    }

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
}
