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
            int mothlingCount = jsonObject[2][i][1].AsInt;
            int flyCount = jsonObject[2][i][2].AsInt;
            int mothCount = jsonObject[2][i][3].AsInt;
            int fireflyCount = jsonObject[2][i][4].AsInt;
            int ladybugCount = jsonObject[2][i][5].AsInt;
            int spiderCount = jsonObject[2][i][6].AsInt;
            // Bosses
            int bossWaspCount = jsonObject[2][i][7].AsInt;
            int bossMegamothlingCount = jsonObject[2][i][8].AsInt;
            int bossMegabeetleCount = jsonObject[2][i][9].AsInt;
            int bossDragonflyCount = jsonObject[2][i][10].AsInt;
            
            int totalEnemies = mothlingCount + flyCount + mothCount + fireflyCount + ladybugCount + spiderCount;
            //Data
            enemyQueue.MaxEnemiesOnScreen = jsonObject[2][i][14].AsInt;
            enemyQueue.AggressionLevel = jsonObject[2][i][15].AsInt;
            enemyQueue.SpawnDelay = jsonObject[2][i][16].AsFloat;
            enemyQueue.SpawnDelayAcceleration = jsonObject[2][i][17].AsFloat;
            
            // Introduce a new enemy
            string enemyIntro = (jsonObject[2][i][18]).ToString().Replace("\"", "");
            if (!string.IsNullOrEmpty(enemyIntro))
            {
                EnemyType firstEnemyType = (EnemyType)System.Enum.Parse(typeof(EnemyType), enemyIntro);
                enemyQueue.Add(firstEnemyType);
                switch (firstEnemyType)
                {
                    case EnemyType.Mothling:
                        mothlingCount--;
                        break;
                    case EnemyType.Fly:
                        flyCount--;
                        break;
                    case EnemyType.Firefly:
                        fireflyCount--;
                        break;
                    case EnemyType.Moth:
                        mothCount--;
                        break;
                    case EnemyType.Ladybug:
                        ladybugCount--;
                        break;
                    case EnemyType.Spider:
                        spiderCount--;
                        break;
                }
                totalEnemies--;
            }
            
            // Add boss
            if (bossMegabeetleCount + bossMegamothlingCount + bossWaspCount + bossDragonflyCount > 0)
            {
                totalEnemies++;
            }

            int bossPosition = 10000;
            if (bossWaspCount > 0)
            {
                if (totalEnemies > 4)
                {
                    bossPosition = Random.Range(3, totalEnemies);
                }
            }
            else
            {
                if (totalEnemies > 3)
                {
                    bossPosition = Random.Range(1, totalEnemies);
                }
                else
                {
                    bossPosition = 0;
                }
            }
            
            for (int j = 0; j < totalEnemies; j++)
            {
                if(j == bossPosition)
                {
                    if (bossWaspCount > 0)
                    {
                        enemyQueue.Add(EnemyType.Wasp);
                        bossWaspCount--;
                    }
                    else if (bossMegamothlingCount > 0)
                    {
                        enemyQueue.Add(EnemyType.Megamothling);
                        bossMegamothlingCount--;
                    }
                    else if (bossMegabeetleCount > 0)
                    {
                        enemyQueue.Add(EnemyType.Megabeetle);
                        bossMegabeetleCount--;
                    }
                    else if (bossDragonflyCount > 0)
                    {
                        enemyQueue.Add(EnemyType.Dragonfly);
                        bossDragonflyCount--;
                    }
                }
                else
                {
                    int randomSelection = Random.Range(0, 6);
                    if(randomSelection == 0)
                    {
                        if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Mothling);
                            mothlingCount--;
                        }
                        else if(flyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Fly);
                            flyCount--;
                        }
                        else if(mothCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Moth);
                            mothCount--;
                        }
                        else if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Spider);
                            spiderCount--;
                        }
                        else if(fireflyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Firefly);
                            fireflyCount--;
                        }
                        else if(ladybugCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Ladybug);
                            ladybugCount--;
                        }
                    }
                    else if(randomSelection == 1)
                    {
                        if(flyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Fly);
                            flyCount--;
                        }
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Mothling);
                            mothlingCount--;
                        }
                        else if(mothCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Moth);
                            mothCount--;
                        }
                        else if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Spider);
                            spiderCount--;
                        }
                        else if(fireflyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Firefly);
                            fireflyCount--;
                        }
                        else if(ladybugCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Ladybug);
                            ladybugCount--;
                        }
                    }
                    else if (randomSelection == 2)
                    {
                        if(mothCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Moth);
                            mothCount--;
                        }
                        else if(flyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Fly);
                            flyCount--;
                        }
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Mothling);
                            mothlingCount--;
                        }
                        else if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Spider);
                            spiderCount--;
                        }
                        else if(fireflyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Firefly);
                            fireflyCount--;
                        }
                        else if(ladybugCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Ladybug);
                            ladybugCount--;
                        }
                    }
                    else if (randomSelection == 3)
                    {
                        if(fireflyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Firefly);
                            fireflyCount--;
                        }
                        else if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Spider);
                            spiderCount--;
                        }
                        else if(mothCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Moth);
                            mothCount--;
                        }
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Mothling);
                            mothlingCount--;
                        }
                        else if(flyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Fly);
                            flyCount--;
                        }
                        else if(ladybugCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Ladybug);
                            ladybugCount--;
                        }
                    }
                    else if (randomSelection == 4)
                    {
                        if(ladybugCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Ladybug);
                            ladybugCount--;
                        }
                        else if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Spider);
                            spiderCount--;
                        }
                        else if(mothCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Moth);
                            mothCount--;
                        }
                        else if(fireflyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Firefly);
                            fireflyCount--;
                        }
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Mothling);
                            mothlingCount--;
                        }
                        else if(flyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Fly);
                            flyCount--;
                        }
                    }
                    else if (randomSelection == 5)
                    {
                        if(spiderCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Spider);
                            spiderCount--;
                        }
                        else if(ladybugCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Ladybug);
                            ladybugCount--;
                        }
                        else if(mothCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Moth);
                            mothCount--;
                        }
                        else if(fireflyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Firefly);
                            fireflyCount--;
                        }
                        else if(flyCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Fly);
                            flyCount--;
                        }
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyType.Mothling);
                            mothlingCount--;
                        }
                    }
                }
            }
            _spawnQueue.Add(enemyQueue);
        }
        return _spawnQueue;
    }
}
