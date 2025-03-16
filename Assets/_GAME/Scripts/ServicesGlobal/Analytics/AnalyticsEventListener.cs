public class AnalyticsEventListener: IDisposable
{
    private IAnalyticsService _analyticsService;
    private GameModel _gameModel;
    
    public AnalyticsEventListener(
        IAnalyticsService analyticsService,
        GameModel gameModel
        )
    {
        _analyticsService = analyticsService;
        _gameModel = gameModel;
        
        _gameModel.WaveStarted += OnWaveStarted;
        _gameModel.WaveEnded += OnWaveEnded;
        _gameModel.LampDamageStarted += OnLampDamageStarted;
        _gameModel.HealthUpgraded += OnHealthUpgraded;
        _gameModel.CoolDownUpgraded += OnCoolDownUpgraded;
        _gameModel.AttackDistanceUpgraded += OnAttackDistanceUpgraded;
    }

    public void Dispose()
    {
        _gameModel.WaveStarted -= OnWaveStarted;
        _gameModel.WaveEnded -= OnWaveEnded;
        _gameModel.LampDamageStarted -= OnLampDamageStarted;
        _gameModel.HealthUpgraded -= OnHealthUpgraded;
        _gameModel.CoolDownUpgraded -= OnCoolDownUpgraded;
        _gameModel.AttackDistanceUpgraded -= OnAttackDistanceUpgraded;
    }


    private void OnWaveStarted(int wave)
    {
        _analyticsService.SubmitWaveStartEvent(wave);
    }

    private void OnWaveEnded(int wave)
    {
        _analyticsService.SubmitWaveEndEvent(wave);
    }

    private void OnLampDamageStarted(float amount, string enemyTypeName)
    {
        _analyticsService.SubmitLampDamageEvent(enemyTypeName);
    }

    private void OnHealthUpgraded()
    {
        _analyticsService.SubmitHealthUpgradeEvent();
    }

    private void OnCoolDownUpgraded()
    {
        _analyticsService.SubmitCoolUpgradeEvent();
    }

    private void OnAttackDistanceUpgraded()
    {
        _analyticsService.SubmitAttackUpgradeEvent();
    }
}
