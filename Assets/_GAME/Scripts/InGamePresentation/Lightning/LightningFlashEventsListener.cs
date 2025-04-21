using _GAME.Scripts.Enemies;
using _GAME.Scripts.GameCoreSystems.EnemyManagement;
using _GAME.Scripts.InGamePresentation.GameStageTransitions;
using _GAME.Scripts.Lib.Interfaces;

namespace _GAME.Scripts.InGamePresentation.Lightning
{
    public class LightningFlashEventsListener: IDisposable
    {
        private WaveEnemyDirector _waveEnemyDirector;
        private LightningFlashController _lightningFlashController;

        public LightningFlashEventsListener(
            WaveEnemyDirector waveEnemyDirector,
            LightningFlashController lightningFlashController
        )
        {
            _waveEnemyDirector = waveEnemyDirector;
            _lightningFlashController = lightningFlashController;
        
            _waveEnemyDirector.BossSpawned += OnBossSpawned;
            _waveEnemyDirector.BossDied += OnBossDied;
        }

        public void Dispose()
        {
            _waveEnemyDirector.BossSpawned -= OnBossSpawned;
            _waveEnemyDirector.BossDied -= OnBossDied;
        }

        private void OnBossSpawned(Enemy boss)
        {
            _lightningFlashController.Play();
        }

        private void OnBossDied()
        {
            _lightningFlashController.Play();
        }
    }
}
