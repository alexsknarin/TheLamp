using System;
using UnityEngine;

public class AdvertisementAnimationController : MonoBehaviour, IInitializable
{
    [SerializeField] private FakeAd _fakeAd;
    [SerializeField] private GameObject _ingameUi;
    [SerializeField] private GameObject _gameOverUi;
    [SerializeField] private GameObject _waveText;
    
    public event Action OnFinishedEvent;

    public void Initialize()
    {
        _fakeAd.OnAdFinishedEvent += HandleAdFinished;
    }

    private void OnDestroy()
    {
        _fakeAd.OnAdFinishedEvent -= HandleAdFinished;
    }

    public void Play()
    {
        _ingameUi.SetActive(false);
        _gameOverUi.SetActive(false);
        _waveText.SetActive(false);
        _fakeAd.Play();
    }

    private void HandleAdFinished()
    {
        OnFinishedEvent?.Invoke();
    }
}
    