using System;

public class ScoresCollectionHandler: IInitializable, IDisposable
{
    private IGameConfigService _gameConfigService;
    
    public ScoresCollectionHandler(IGameConfigService gameConfigService)
    {
        _gameConfigService = gameConfigService;
    }
    
    public event Action<int> ScoreChanged;
    

    public void Initialize()
    {
        EnemyBase.EnemyDied += OnEnemyDied; // TODO: remove static - Use Interface???
    }

    public void Dispose()
    {
        EnemyBase.EnemyDied += OnEnemyDied;
    }
    
    private void OnEnemyDied(EnemyBase enemy)
    {
        int score = 0;

        switch (enemy.EnemyType)
        {
            case EnemyType.Mothling:
                score = _gameConfigService.ScoreConfig.MothlingScorePrice;
                break;
            case EnemyType.Megamothling:
                score = _gameConfigService.ScoreConfig.MegamothlingScorePrice;
                break;
            case  EnemyType.Fly:
                score = _gameConfigService.ScoreConfig.FlyScorePrice;
                break;
            case EnemyType.Firefly:
                score = _gameConfigService.ScoreConfig.FireflyScorePrice;
                break;
            case EnemyType.Moth:
                score = _gameConfigService.ScoreConfig.MothScorePrice;
                break;
            case EnemyType.Ladybug:
                score = _gameConfigService.ScoreConfig.LadybugScorePrice;
                break;
            case EnemyType.Spider:
                score = _gameConfigService.ScoreConfig.SpiderScorePrice;
                break;
            case EnemyType.Wasp:
                score = _gameConfigService.ScoreConfig.WaspsScorePrice;
                break;
            case EnemyType.Megabeetle:
                score = _gameConfigService.ScoreConfig.MegabeetleScorePrice;
                break;
            case EnemyType.DragonflyProjectile:
                score = _gameConfigService.ScoreConfig.DragonflyProjectileScorePrice;
                break;
            case EnemyType.Dragonfly:
                score = _gameConfigService.ScoreConfig.Dragonfly;
                break;
        }
        
        ScoreChanged?.Invoke(score);
    }
}
