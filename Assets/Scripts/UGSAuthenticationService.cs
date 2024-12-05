using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class UGSAuthenticationService : MonoBehaviour, IUGSAuthenticationService
{
    public async void Initialize()
    {
        Debug.Log("UGS: Initializing UGS");
        var options = new InitializationOptions();
        options.SetEnvironmentName("production");
        await UnityServices.InitializeAsync(options);
        SignInAnonymously();
    }
    
    private async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("UGS: Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            Debug.Log("UGS: Authentification Failed");
            Debug.Log(s);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
