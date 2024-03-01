using UnityEngine;
using SimpleJSON;

public class SpawnQueueGenerator
{
    private string _data;
    private SpawnQueue _spawnQueue;
    
    public SpawnQueueGenerator(string data)
    {
        _data = data;
    }
    
    public SpawnQueue Generate()
    {
        _spawnQueue = new SpawnQueue();
        var jsonObject = JSON.Parse(_data);

        for (int i = 1; i<jsonObject[2].Count; i++)
        {
            EnemyQueue enemyQueue = new EnemyQueue();
            int flyCount = jsonObject[2][i][1].AsInt;
            int mothCount = jsonObject[2][i][2].AsInt;
            int fireflyCount = jsonObject[2][i][3].AsInt;
            int ladybugCount = jsonObject[2][i][4].AsInt;
            int totalEnemies = flyCount + mothCount + fireflyCount + ladybugCount;
            
            for (int j = 0; j < totalEnemies; j++)
            {
                int randomSelection = Random.Range(0, 4);
                if(randomSelection == 0)
                {
                    if(flyCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Fly);
                        flyCount--;
                    }
                    else if(mothCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Moth);
                        mothCount--;
                    }
                    else if(fireflyCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Firefly);
                        fireflyCount--;
                    }
                    else if(ladybugCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Ladybug);
                        ladybugCount--;
                    }
                }
                else if (randomSelection == 1)
                {
                    if(mothCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Moth);
                        mothCount--;
                    }
                    else if(flyCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Fly);
                        flyCount--;
                    }
                    else if(fireflyCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Firefly);
                        fireflyCount--;
                    }
                    else if(ladybugCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Ladybug);
                        ladybugCount--;
                    }
                }
                else if (randomSelection == 2)
                {
                    if(fireflyCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Firefly);
                        fireflyCount--;
                    }
                    else if(mothCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Moth);
                        mothCount--;
                    }
                    else if(flyCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Fly);
                        flyCount--;
                    }
                    else if(ladybugCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Ladybug);
                        ladybugCount--;
                    }
                }
                else if (randomSelection == 3)
                {
                    if(ladybugCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Ladybug);
                        ladybugCount--;
                    }
                    else if(mothCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Moth);
                        mothCount--;
                    }
                    else if(fireflyCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Firefly);
                        fireflyCount--;
                    }
                    else if(flyCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Fly);
                        flyCount--;
                    }
                }
            }
            _spawnQueue.Add(enemyQueue);
        }
        return _spawnQueue;
    }
}
