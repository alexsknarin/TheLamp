using System;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.Core;

using Unity.Services.Core.Environments;
using UnityEngine;

public class UGSSetup : MonoBehaviour
{
    private bool _isConnecceted = false;
    
    public event Action OnConsentAddressed;

    public void AllowDataCollection()
    {
        PlayerPrefs.SetInt("dataConsent", 1);
        PlayerPrefs.Save();
        OnConsentAddressed?.Invoke();
    }
    
    public void RefuseDataCollection()
    {
        PlayerPrefs.SetInt("dataConsent", 0);
        PlayerPrefs.Save();
        OnConsentAddressed?.Invoke();
    }

    public async void Setup()
    {
        if (!_isConnecceted)
        {
            
            Debug.Log("Setting up UGS");
            var options = new InitializationOptions();
            options.SetEnvironmentName("production");
            await UnityServices.InitializeAsync(options);
            SignInAnonymously();
            GiveConsent();
            _isConnecceted = true;
        }
    }
    
    private async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            Debug.Log("Authentification Failed");
            Debug.Log(s);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    
    private void GiveConsent()
    {
        if (PlayerPrefs.GetInt("dataConsent") == 1)
        {
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("Consent has been provided. The SDK is now collecting data");
        }
    }
    
    public void StopAnalyticsCollection()
    {
        AnalyticsService.Instance.StopDataCollection();
    }

    public void StartAnalyticsCollection()
    {
        AnalyticsService.Instance.StartDataCollection();
    }
}
