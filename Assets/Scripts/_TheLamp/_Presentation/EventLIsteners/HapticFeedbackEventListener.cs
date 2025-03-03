public class HapticFeedbackEventListener: IDisposable
{
    private HapticFeedbackService _hapticFeedbackService;
    private WaveEnemyDirector _waveEnemyDirector;
    private GameModel _gameModel;
    
    public HapticFeedbackEventListener(
        HapticFeedbackService hapticFeedbackService,
        WaveEnemyDirector waveEnemyDirector,
        GameModel gameModel
        )
    {
        _hapticFeedbackService = hapticFeedbackService;
        _waveEnemyDirector = waveEnemyDirector;
        _gameModel = gameModel;

        _waveEnemyDirector.FireflyExplosionStarted += OnFireflyExplosionStarted;
        _waveEnemyDirector.BossSpawned += OnLampDestroyed;
        _waveEnemyDirector.BossDied += OnLampDestroyed;
        _gameModel.LampAttackStarted += OnAttackClicked;
        _gameModel.LampDamageStarted += OnLampDamaged;
        _gameModel.LampDestroyed += OnLampDestroyed;
        _gameModel.UpgradeClicked += OnUpgradeClicked;
        _gameModel.IsLampBlockedChanged += OnIsLampBlockedChanged;
    }

    public void Dispose()
    {
        _waveEnemyDirector.FireflyExplosionStarted -= OnFireflyExplosionStarted;
        _waveEnemyDirector.BossSpawned -= OnLampDestroyed;
        _waveEnemyDirector.BossDied -= OnLampDestroyed;
        _gameModel.LampAttackStarted -= OnAttackClicked;
        _gameModel.LampDamageStarted -= OnLampDamaged;
        _gameModel.LampDestroyed -= OnLampDestroyed;
        _gameModel.UpgradeClicked -= OnUpgradeClicked;
        _gameModel.IsLampBlockedChanged -= OnIsLampBlockedChanged;
    }

    private void OnAttackClicked(float power)
    {
        _hapticFeedbackService.DoTouchHaptic();
    }

    private void OnFireflyExplosionStarted()
    {
        _hapticFeedbackService.DoExplosionVibration();
    }

    private void OnLampDamaged(float obj, string enemyTypeName)
    {
        _hapticFeedbackService.DoDamageVibration();
    }

    private void OnUpgradeClicked()
    {
        _hapticFeedbackService.DoTouchHaptic();
    }

    private void OnIsLampBlockedChanged(bool obj)
    {
        _hapticFeedbackService.DoDamageVibration();
    }

    private void OnLampDestroyed()
    {
        _hapticFeedbackService.DoDamageVibration();
    }
}
