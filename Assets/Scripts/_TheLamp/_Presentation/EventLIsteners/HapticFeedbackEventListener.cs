public class HapticFeedbackEventListener: IDisposable
{
    private HapticFeedbackService _hapticFeedbackService;
    private EnemyController _enemyController;
    private GameModel _gameModel;
    
    public HapticFeedbackEventListener(
        HapticFeedbackService hapticFeedbackService,
        EnemyController enemyController,
        GameModel gameModel
        )
    {
        _hapticFeedbackService = hapticFeedbackService;
        _enemyController = enemyController;
        _gameModel = gameModel;

        _enemyController.FireflyExplosionStarted += OnFireflyExplosionStarted;
        _gameModel.LampAttackStarted += OnAttackClicked;
        _gameModel.LampDamageStarted += OnLampDamaged;
        _gameModel.LampDestroyed += OnLampDestroyed;
        _gameModel.UpgradeClicked += OnUpgradeClicked;
        _gameModel.IsLampBlockedChanged += OnIsLampBlockedChanged;
    }

    public void Dispose()
    {
        _enemyController.FireflyExplosionStarted -= OnFireflyExplosionStarted;
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
