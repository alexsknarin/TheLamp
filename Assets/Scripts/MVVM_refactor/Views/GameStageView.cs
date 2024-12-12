using System.Collections;
using UnityEngine;

public class GameStageView : MonoBehaviour
{
    [SerializeField] private float _introDuration;
    [SerializeField] private float _waveDuration;
    [SerializeField] private float _prepareDuration;
    [SerializeField] private float _gameoverDuration;
    
    GameStageViewModel _gameStageViewModel;
    
    public void Bind(GameStageViewModel viewModel)
    {
        _gameStageViewModel = viewModel;
        
        _gameStageViewModel.CurrentGameStageState.OnChangedEvent += OnGameStageStateChanged;
        
    }

    private void OnGameStageStateChanged(object sender, Observable<GameStageState>.ChangedEventArgs e)
    {
        switch (e.NewValue)
        {
            case GameStageState.Intro:
                Debug.Log(" >> Startinf Intro");
                StartCoroutine(PlayIntro());
                break;
            case GameStageState.Wave:
                Debug.Log(" >> Starting Wave");
                StartCoroutine(PlayWave());
                break;
            case GameStageState.Prepare:
                Debug.Log(" >> Starting Prepare");
                StartCoroutine(PlayPrepare());
                break;
            case GameStageState.Gameover:
                Debug.Log(" >> Starting Gameover");
                StartCoroutine(PlayGameover());
                break;
        }
    }
    
    private IEnumerator PlayIntro()
    {
        Debug.Log("...");
        yield return new WaitForSeconds(_introDuration);
        Debug.Log("Intro Finished");
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
