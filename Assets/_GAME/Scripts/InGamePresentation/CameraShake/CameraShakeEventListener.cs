using _GAME.Scripts.Enemies;
using _GAME.Scripts.Factories;
using _GAME.Scripts.GameCoreSystems;
using _GAME.Scripts.GameCoreSystems.EnemyManagement;
using _GAME.Scripts.Lib.Interfaces;

namespace _GAME.Scripts.InGamePresentation.CameraShake
{
    public class CameraShakeEventListener: IDisposable
    {
        private GameModel _gameModel;
        private WaveEnemyDirector _waveEnemyDirector;
        private CameraShakeService _cameraShakeService;
        private BossCameraShakeFactory _bossCameraShakeFactory;


        public CameraShakeEventListener(
            GameModel gameModel,
            WaveEnemyDirector waveEnemyDirector,
            CameraShakeService cameraShakeService,
            BossCameraShakeFactory bossCameraShakeFactory
        )
        {
            _gameModel = gameModel;
            _waveEnemyDirector = waveEnemyDirector;
            _cameraShakeService = cameraShakeService;
            _bossCameraShakeFactory = bossCameraShakeFactory;
        
            _gameModel.GameStarted += OnGameStarted;
            _gameModel.LampDamageStarted += OnLampDamageStarted;
            _gameModel.LampDestroyed += OnLampDestroyed;
            _waveEnemyDirector.FireflyExplosionStarted += OnFireflyExplosionStarted;
            _waveEnemyDirector.BossSpawned += OnBossSpawned;
            _waveEnemyDirector.BossDied -= OnBossDied;
        }

        public void Dispose()
        {
            _gameModel.GameStarted -= OnGameStarted;
            _gameModel.LampDamageStarted -= OnLampDamageStarted;
            _gameModel.LampDestroyed -= OnLampDestroyed;
            _waveEnemyDirector.FireflyExplosionStarted -= OnFireflyExplosionStarted;
            _waveEnemyDirector.BossSpawned -= OnBossSpawned;
            _waveEnemyDirector.BossDied -= OnBossDied;
        }

        private void OnGameStarted()
        {
            _cameraShakeService.DisableBossShake();
        }

        private void OnFireflyExplosionStarted()
        {
            _cameraShakeService.StartExplosionShake();
        }

        private void OnLampDestroyed()
        {
            _cameraShakeService.StartDamageShake();
            _cameraShakeService.DisableBossShake();
        }

        private void OnLampDamageStarted(float obj, string enemy)
        {
            _cameraShakeService.StartDamageShake();
        }

        private void OnBossSpawned(Enemy boss)
        {
            _cameraShakeService.EnableBossShake(_bossCameraShakeFactory.Create(boss)); 
        }

        private void OnBossDied()
        {
            _cameraShakeService.DisableBossShake();
        }
    }
}
