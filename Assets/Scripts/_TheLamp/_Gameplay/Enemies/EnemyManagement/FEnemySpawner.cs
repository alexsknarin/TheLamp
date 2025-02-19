using System.Collections.Generic;
using UnityEngine;

public class FEnemySpawner: ITickable, IInitializable, IDisposable
{
    private EnemyQueue _enemyQueue;
    
    // Dependencies
    private readonly FEnemyPool _enemyPool;
    
    public FEnemySpawner(FEnemyPool enemyPool)
    {
        _enemyPool = enemyPool;
    }

    public void Initialize()
    {
    }

    public void Dispose()
    {
    }

    private void OnEnemyReleased(FEnemy enemy)
    {
    }

    public void PrepareWave(EnemyQueue enemyQueue)
    {
        _enemyQueue = enemyQueue;
        string waveData = "";
        
        for (int i = 0; i < _enemyQueue.Count(); i++)
        {
            _enemyPool.PreloadEnemy(EnemyTypeLibrary.EnemyTypeDictionary[_enemyQueue.Get(i)]);
            waveData = waveData + " - " + _enemyQueue.Get(i).ToString();    
        }
        Debug.Log(waveData);
    }

    public void StartWave()
    {
        for (int i = 0; i < _enemyQueue.Count(); i++)
        {
            var enemy = _enemyPool.Get(EnemyTypeLibrary.EnemyTypeDictionary[_enemyQueue.Get(i)]);
            enemy.Play();
        }
    }

    public void Tick(float deltaTime)
    {
    }
}
