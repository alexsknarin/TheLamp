using UnityEngine;

public class InjectorScene : MonoBehaviour
{
    // TODO: Add a system that can quickly replace providers with dummies for testing
    
    [Header("Dependency Providers")]
    [SerializeField] private UnityAnalyticsService _analyticsService;
    [SerializeField] private UGSAuthenticationService _ugsAuthenticationService;
    
    [Header("Dependency Clients")]
    [SerializeField] private Game _game;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private Lamp _lamp;
    [SerializeField] private LampStatsManager _lampStatsManager;
    
    private void Awake()
    {
        _game.Inject(
            _analyticsService, 
            _ugsAuthenticationService
            );                                                 // TODO: Use Fluent Builder to inject many things ???
        _uiManager.Inject(_analyticsService);
        _enemyManager.Inject(_analyticsService);
        _lamp.Inject(_analyticsService);
        _lampStatsManager.Inject(_analyticsService);
        
        
        Debug.Log("Dpendencies are Injected");
        
    }
    
}
