using UnityEngine;

public class GameInitialization : MonoBehaviour
{
    [SerializeField] private Lamp _lamp;
    [SerializeField] private GoogleSheetsDataReader _googleSheetsDataReader;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private EnemyPool _enemyPool;

    private void OnEnable()
    {
        _googleSheetsDataReader.OnDataLoaded += InitializeEnemyManager;
    }

    private void OnDisable()
    {
        _googleSheetsDataReader.OnDataLoaded -= InitializeEnemyManager;
    }


    void Start()
    {
        Application.targetFrameRate = 60;
        _lamp.Initialize(); 
        _enemyPool.Initialize();
        _googleSheetsDataReader.Initialize();
    }
    
    private void InitializeEnemyManager()
    {
        _enemyManager.Initialize();
    }

}
