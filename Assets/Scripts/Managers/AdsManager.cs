using System;
using UnityEngine;

public class AdsManager : MonoBehaviour, IInitializable
{
    [SerializeField] private FakeAd _fakeAd;
    
    public event Action OnAdFinishedEvent;

    private void OnEnable()
    {
        _fakeAd.OnAdFinishedEvent += HandleAdFinished;    
    }
    
    private void OnDisable()
    {
        _fakeAd.OnAdFinishedEvent -= HandleAdFinished;
    }

    public void Initialize()
    {
        _fakeAd.gameObject.SetActive(false);
    }

    public void ShowAd()
    {
        _fakeAd.Play();
    }
    
    private void HandleAdFinished()
    {
        OnAdFinishedEvent?.Invoke();
    }
    
}
