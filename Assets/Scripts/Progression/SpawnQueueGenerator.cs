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
            // Enemies
            int flyCount = jsonObject[2][i][1].AsInt;
            int mothCount = jsonObject[2][i][2].AsInt;
            int fireflyCount = jsonObject[2][i][3].AsInt;
            int ladybugCount = jsonObject[2][i][4].AsInt;
            int spiderCount = jsonObject[2][i][5].AsInt;
            // Bosses
            int bossWaspCount = jsonObject[2][i][6].AsInt;
            int totalEnemies = flyCount + mothCount + fireflyCount + ladybugCount + spiderCount;
            int maxOnScreen = jsonObject[2][i][8].AsInt;
            int AggressionLevel = jsonObject[2][i][9].AsInt;
            enemyQueue.MaxEnemiesOnScreen = maxOnScreen;
            enemyQueue.AggressionLevel = AggressionLevel;
            
            int bossPosition = Random.Range(3, 5);
            
            for (int j = 0; j < totalEnemies; j++)
            {
                if(j == bossPosition && bossWaspCount > 0)
                {
                    enemyQueue.Add(EnemyTypes.Wasp);
                    bossWaspCount--;
                }
                else
                {
                    int randomSelection = Random.Range(0, 5);
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
                        else if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Spider);
                            spiderCount--;
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
                        else if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Spider);
                            spiderCount--;
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
                        else if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Spider);
                            spiderCount--;
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
                        else if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Spider);
                            spiderCount--;
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
                    else if (randomSelection == 4)
                    {
                        if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Spider);
                            spiderCount--;
                        }
                        else if(ladybugCount > 0)
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
            }
            _spawnQueue.Add(enemyQueue);
        }
        return _spawnQueue;
    }
}
