using UnityEngine;

public class FEnemyFactory
{
    private FMothling _mothlingEnemyPrefab;
    private MothlingMovementStateFactory _mothlingMovementStateFactory;
    
    public FEnemyFactory(FMothling mothlingEnemyPrefab, MothlingMovementStateFactory mothlingMovementStateFactory)
    {
        _mothlingEnemyPrefab = mothlingEnemyPrefab; // find a way to load dynamically ???? check if it's possible
        _mothlingMovementStateFactory = mothlingMovementStateFactory;
    }
    
    public FEnemy CreateMothling()
    {
        FMothling enemyInstance = Object.Instantiate(_mothlingEnemyPrefab);
        enemyInstance.GetComponent<FMothlingMovement>().Construct(_mothlingMovementStateFactory);
        enemyInstance.GetComponent<FMothlingPresentation>().Initialize();
        enemyInstance.Initialize();
        return enemyInstance;
    }
}
