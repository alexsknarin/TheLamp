using System;
using UnityEngine;

public class ScoresManager : MonoBehaviour, IInitializable
{
    [Header("Score per enemy prices")] 
    [SerializeField] private int _mothlingScorePrice;
    [SerializeField] private int _megamothlingScorePrice;
    [SerializeField] private int _flyScorePrice;
    [SerializeField] private int _fireflyScorePrice;
    [SerializeField] private int _mothScorePrice;
    [SerializeField] private int _ladybugScorePrice;
    [SerializeField] private int _spiderScorePrice;
    [SerializeField] private int _waspsScorePrice;
    [SerializeField] private int _megabeetleScorePrice;
    [SerializeField] private int _dragonflyProjectileScorePrice;
    [SerializeField] private int _currentScore;
    [SerializeField] private SaveDataContainer _saveDataContainer;
    public int CurretScore => _currentScore;
    
    public static event Action<int> OnScoreChangeEvent;

    private void OnEnable()
    {
         EnemyBase.OnEnemyDeathEvent += CollectScore;
         // EnemyManager.OnBossDeath += CollectScore;
    }
    
    private void OnDisable()
    {
        EnemyBase.OnEnemyDeathEvent -= CollectScore;
        // EnemyManager.OnBossDeath += CollectScore;
    }


    public void Initialize()
    {
        _currentScore = _saveDataContainer.CurrentScore;
    }
    
    private void CollectScore(EnemyBase enemy)
    {
        switch (enemy.EnemyType)
        {
            case EnemyType.Mothling:
                _currentScore += _mothlingScorePrice;
                break;
            case EnemyType.Megamothling:
                _currentScore += _megamothlingScorePrice;
                break;
            case  EnemyType.Fly:
                _currentScore += _flyScorePrice;
                break;
            case EnemyType.Firefly:
                _currentScore += _fireflyScorePrice;
                break;
            case EnemyType.Moth:
                _currentScore += _mothScorePrice;
                break;
            case EnemyType.Ladybug:
                _currentScore += _ladybugScorePrice;
                break;
            case EnemyType.Spider:
                _currentScore += _spiderScorePrice;
                break;
            case EnemyType.Wasp:
                _currentScore += _waspsScorePrice;
                break;
            case EnemyType.Megabeetle:
                _currentScore += _megabeetleScorePrice;
                break;
            case EnemyType.DragonflyProjectile:
                _currentScore += _dragonflyProjectileScorePrice;
                break;
        }
        _saveDataContainer.CurrentScore = _currentScore;
        OnScoreChangeEvent?.Invoke(_currentScore);
    }
}
