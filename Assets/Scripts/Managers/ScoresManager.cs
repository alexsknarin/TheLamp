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
    [SerializeField] private int _currentScore;
    [SerializeField] private SaveDataContainer _saveDataContainer;
    public int CurretScore => _currentScore;
    
    public static event Action<int> OnScoreChange;

    private void OnEnable()
    {
         EnemyBase.OnEnemyDeath += CollectScore;
         EnemyManager.OnBossDeath += CollectScore;
    }
    
    private void OnDisable()
    {
        EnemyBase.OnEnemyDeath -= CollectScore;
        EnemyManager.OnBossDeath += CollectScore;
    }


    public void Initialize()
    {
        _currentScore = _saveDataContainer.CurrentScore;
    }
    
    private void CollectScore(EnemyBase enemy)
    {
        switch (enemy.EnemyType)
        {
            case EnemyTypes.Mothling:
                _currentScore += _mothlingScorePrice;
                break;
            case EnemyTypes.Megamothling:
                _currentScore += _megamothlingScorePrice;
                break;
            case  EnemyTypes.Fly:
                _currentScore += _flyScorePrice;
                break;
            case EnemyTypes.Firefly:
                _currentScore += _fireflyScorePrice;
                break;
            case EnemyTypes.Moth:
                _currentScore += _mothScorePrice;
                break;
            case EnemyTypes.Ladybug:
                _currentScore += _ladybugScorePrice;
                break;
            case EnemyTypes.Spider:
                _currentScore += _spiderScorePrice;
                break;
            case EnemyTypes.Wasp:
                _currentScore += _waspsScorePrice;
                break;
        }
        _saveDataContainer.CurrentScore = _currentScore;
        OnScoreChange?.Invoke(_currentScore);
    }
}
