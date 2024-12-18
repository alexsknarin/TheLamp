using System.Collections;
using UnityEngine;

public class GameStageView : MonoBehaviour, IInitializable
{
    [SerializeField] private IntroGameStageAnimationController _introGameStageAnimationController;
    [SerializeField] private PrepareInGameStageAnimationController _prepareInGameStageAnimationController;
    [SerializeField] private PrepareOutGameStageAnimationController _prepareOutGameStageAnimationController;
    [SerializeField] private float _waveDuration;
    [SerializeField] private float _prepareDuration;
    [SerializeField] private float _gameoverDuration;
    
    GameStageViewModel _gameStageViewModel;
    
    public void Construct(GameStageViewModel viewModel)
    {
        _gameStageViewModel = viewModel;
        _gameStageViewModel.OnIntroStartedEvent += StartIntro;
        _gameStageViewModel.OnPrepareInStartedEvent += StartPrepareIn;
        _gameStageViewModel.OnPrepareOutStartedEvent += StartPrepareOut;
        
        _introGameStageAnimationController.OnFinishedEvent += HandleIntroEnd;
        _prepareInGameStageAnimationController.OnFinishedEvent += HandlePrepareInEnd;
        _prepareOutGameStageAnimationController.OnFinishedEvent += HandlePrepareOutEnd;
    }
    
    public void Initialize()
    {
        _introGameStageAnimationController.Initialize();
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
    
    
    public void StartIntro(int currentHealth, int maxHealth)
    {
        _introGameStageAnimationController.Play(currentHealth, maxHealth);
    }
    
    private void StartPrepareIn(bool isUpgradeUiRequired, int waveNum)
    {
        // TODO: add upgrade UI logic
        _prepareInGameStageAnimationController.Play(waveNum);
    }
    
    private void StartPrepareOut()
    {
        _prepareOutGameStageAnimationController.Play();   
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
