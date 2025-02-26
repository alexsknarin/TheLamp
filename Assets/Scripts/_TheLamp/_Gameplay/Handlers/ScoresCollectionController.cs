using System;

public class ScoresCollectionController: IInitializable, IDisposable
{
    public bool _isActive = false;
    
    private IGameConfigService _gameConfigService;
    private IEnemyDeactivatedProvider _enemyDeactivatedProvider;
    
    public ScoresCollectionController(
        IGameConfigService gameConfigService,
        IEnemyDeactivatedProvider enemyDeactivatedProvider
        )
    {
        _gameConfigService = gameConfigService;
        _enemyDeactivatedProvider = enemyDeactivatedProvider;
    }
    
    public void StartCollecting()
    {
        _isActive = true;
    }
    
    public void StopCollecting()
    {
        _isActive = false;
    }
    
    public event Action<int> ScoreChanged;
    
    public void Initialize()
    {
        _enemyDeactivatedProvider.EnemyReleasedToPool += OnEnemyDeactivated; // TODO: Probably IDeactivatable interface 
    }

    public void Dispose()
    {
        _enemyDeactivatedProvider.EnemyReleasedToPool -= OnEnemyDeactivated;
    }
    
    // Event Handle Methods
    private void OnEnemyDeactivated(FEnemy enemy)
    {
        if (!_isActive)
        {
            return;
        }
        
        int score = 0;
        
        var enemyType = EnemyTypeLibrary.TypeEnemyDictionary[enemy.GetType()]; 

        switch (enemyType)
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