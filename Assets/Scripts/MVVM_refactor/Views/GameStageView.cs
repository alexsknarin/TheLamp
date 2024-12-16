using System.Collections;
using UnityEngine;

public class GameStageView : MonoBehaviour, IInitializable
{
    [SerializeField] private IntroGameStageAnimationController _introGameStageAnimationController;
    [SerializeField] private float _waveDuration;
    [SerializeField] private float _prepareDuration;
    [SerializeField] private float _gameoverDuration;
    
    GameStageViewModel _gameStageViewModel;
    
    public void Construct(GameStageViewModel viewModel)
    {
        _gameStageViewModel = viewModel;
        _gameStageViewModel.CurrentGameStageState.OnChangedEvent += OnGameStageStateChanged;
        _introGameStageAnimationController.OnFinishedEvent += OnAnimationFinishedHandler;
    }

    public void Initialize()
    {
        _introGameStageAnimationController.Initialize();
    }

    private void OnDestroy()
    {
        _gameStageViewModel.CurrentGameStageState.OnChangedEvent -= OnGameStageStateChanged;
        _introGameStageAnimationController.OnFinishedEvent -= OnAnimationFinishedHandler;
    }

    private void OnGameStageStateChanged(object sender, Observable<GameStageState>.ChangedEventArgs e)
    {
        switch (e.NewValue)
        {
            case GameStageState.IntroAnimation:
                Debug.Log(" >> Startinf Intro");
                _introGameStageAnimationController.Play(8, 8);
                break;
            case GameStageState.Wave:
                Debug.Log(" >> Starting Wave");
                StartCoroutine(PlayWave());
                break;
            case GameStageState.PrepareInAnimation:
                Debug.Log(" >> Starting Prepare");
                StartCoroutine(PlayPrepare());
                break;
            case GameStageState.GameOverAnimation:
                Debug.Log(" >> Starting Gameover");
                StartCoroutine(PlayGameover());
                break;
        }
    }

    private void OnAnimationFinishedHandler()
    {
        _gameStageViewModel.HandleCurrentStageStateFinished();
    }

    private IEnumerator PlayWave()
    {
        Debug.Log("...");
        yield return new WaitForSeconds(_waveDuration);
        Debug.Log("Wave Finished");
        _gameStageViewModel.HandleCurrentStageStateFinished();
    }

    private IEnumerator PlayPrepare()
    {
        Debug.Log("...");
        yield return new WaitForSeconds(_prepareDuration);
        Debug.Log("Prepare Finished");
        _gameStageViewModel.HandleCurrentStageStateFinished();
    }

    private IEnumerator PlayGameover()
    {
        Debug.Log("...");
        yield return new WaitForSeconds(_gameoverDuration);
        Debug.Log("Gameover Finished");
        _gameStageViewModel.HandleCurrentStageStateFinished();
    }
}
