using System;
using UnityEngine;

public class AdsManager : MonoBehaviour, IInitializable
{
    [SerializeField] private FakeAd _fakeAd;
    
    public event Action OnAdFinished;

    private void OnEnable()
    {
        _fakeAd.OnAdFinished += HandleAdFinished;    
    }
    
    private void OnDisable()
    {
        _fakeAd.OnAdFinished -= HandleAdFinished;
    }

    public void Initialize()
    {
        Debug.Log("... Connecting to Unity Ads Service");
        Debug.Log("... Preloading ads");
        _fakeAd.gameObject.SetActive(false);
    }

    public void ShowAd()
    {
        _fakeAd.Play();
    }
    
    private void HandleAdFinished()
    {
        OnAdFinished?.Invoke();
    }
    
}
