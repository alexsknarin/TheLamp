using System.Threading.Tasks;
using _GAME.Scripts.Lib.Interfaces;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace _GAME.Scripts.ServicesGlobal
{
    public class UGSAuthenticationService : IUGSAuthenticationService, IInitializable
    {
        public bool IsConnected { get; private set; }

        public async void Initialize()
        {
            Debug.Log("UGS: Initializing UGS");
            var options = new InitializationOptions();
            options.SetEnvironmentName("production");
            await UnityServices.InitializeAsync(options);
            await SignInAnonymously();
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
            IsConnected = true;
        }
    }
}
