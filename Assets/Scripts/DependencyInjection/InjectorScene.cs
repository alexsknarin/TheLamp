using UnityEngine;

public class InjectorScene : MonoBehaviour
{
    [Header("Dependency Providers")]
    [SerializeField] private UnityAnalyticsService _analyticsService;
    
    [Header("Dependency Consumers")]
    [SerializeField] private UiManager _uiManager;
    
    private void Awake()
    {
        _uiManager.Inject(_analyticsService);
    }
    
}
