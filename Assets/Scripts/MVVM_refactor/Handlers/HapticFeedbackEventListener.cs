using UnityEngine;

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
        _gameModel.LampDied += OnLampLampDied;
        _gameModel.UpgradeClicked += OnUpgradeClicked;
        _gameModel.LampBlockedModeSet += OnLampBlockedModeSet;
    }

    public void Dispose()
    {
        _enemyController.FireflyExplosionStarted -= OnFireflyExplosionStarted;
        _gameModel.LampAttackStarted -= OnAttackClicked;
        _gameModel.LampDamageStarted -= OnLampDamaged;
        _gameModel.LampDied -= OnLampLampDied;
        _gameModel.UpgradeClicked -= OnUpgradeClicked;
        _gameModel.LampBlockedModeSet -= OnLampBlockedModeSet;
    }

    private void OnAttackClicked(float power)
    {
        _hapticFeedbackService.DoTouchHaptic();
    }

    private void OnFireflyExplosionStarted()
    {
        _hapticFeedbackService.DoExplosionVibration();
    }

    private void OnLampDamaged(float obj)
    {
        _hapticFeedbackService.DoDamageVibration();
    }

    private void OnUpgradeClicked()
    {
        _hapticFeedbackService.DoTouchHaptic();
    }

    private void OnLampBlockedModeSet(bool obj)
    {
        _hapticFeedbackService.DoDamageVibration();
    }

    private void OnLampLampDied(EnemyBase obj)
    {
        _hapticFeedbackService.DoDamageVibration();
    }
}
