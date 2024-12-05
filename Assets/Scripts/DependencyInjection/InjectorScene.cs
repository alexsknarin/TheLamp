using UnityEngine;

public class InjectorScene : MonoBehaviour
{
    [Header("Dependency Providers")]
    [SerializeField] private UnityAnalyticsService _analyticsService;
    [SerializeField] private UGSAuthenticationService _ugsAuthenticationService;
    
    [Header("Dependency Clients")]
    [SerializeField] private Game _game;
    [SerializeField] private UiManager _uiManager;
    
    private void Awake()
    {
        _game.Inject(
            _analyticsService, 
            _ugsAuthenticationService
            );                                                 // TODO: Use Fluent Builder to inject many things ???
        _uiManager.Inject(_analyticsService);
        
        
        Debug.Log("Dpendencies are Injected");
        
    }
    
}
