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
            int totalEnemies = mothlingCount + flyCount + mothCount + fireflyCount + ladybugCount + spiderCount;
            //Data
            enemyQueue.MaxEnemiesOnScreen = jsonObject[2][i][14].AsInt;
            enemyQueue.AggressionLevel = jsonObject[2][i][15].AsInt;
            enemyQueue.SpawnDelay = jsonObject[2][i][16].AsFloat;
            enemyQueue.SpawnDelayAcceleration = jsonObject[2][i][17].AsFloat;
            
            // Introduce a new enemy
            string enemyIntro = (jsonObject[2][i][18]).ToString().Replace("\"", "");
            if ( !string.IsNullOrEmpty(enemyIntro))
            {
                EnemyTypes firstEnemyType = (EnemyTypes)System.Enum.Parse(typeof(EnemyTypes), enemyIntro);
                enemyQueue.Add(firstEnemyType);
                switch (firstEnemyType)
                {
                    case EnemyTypes.Mothling:
                        mothlingCount--;
                        break;
                    case EnemyTypes.Fly:
                        flyCount--;
                        break;
                    case EnemyTypes.Firefly:
                        fireflyCount--;
                        break;
                    case EnemyTypes.Moth:
                        mothCount--;
                        break;
                    case EnemyTypes.Ladybug:
                        ladybugCount--;
                        break;
                    case EnemyTypes.Spider:
                        spiderCount--;
                        break;
                }
                totalEnemies--;
            }
            
            int bossPosition = Random.Range(3, 5);
            if (bossMegamothlingCount > 0)
            {
                bossPosition = 0;
            }
            for (int j = 0; j < totalEnemies; j++)
            {
                if(j == bossPosition)
                {
                    if (bossWaspCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Wasp);
                        bossWaspCount--;
                    }
                    else if (bossMegamothlingCount > 0)
                    {
                        enemyQueue.Add(EnemyTypes.Megamothling);
                        bossMegamothlingCount--;
                    }
                }
                else
                {
                    int randomSelection = Random.Range(0, 6);
                    if(randomSelection == 0)
                    {
                        if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Mothling);
                            mothlingCount--;
                        }
                        else if(flyCount > 0)
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
                    else if(randomSelection == 1)
                    {
                        if(flyCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Fly);
                            flyCount--;
                        }
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Mothling);
                            mothlingCount--;
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
                    else if (randomSelection == 2)
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
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Mothling);
                            mothlingCount--;
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
                    else if (randomSelection == 3)
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
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Mothling);
                            mothlingCount--;
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
                    else if (randomSelection == 4)
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
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Mothling);
                            mothlingCount--;
                        }
                        else if(flyCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Fly);
                            flyCount--;
                        }
                    }
                    else if (randomSelection == 5)
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
                        else if(mothlingCount > 0)
                        {
                            enemyQueue.Add(EnemyTypes.Mothling);
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
