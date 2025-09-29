using System.Collections.Generic;
using System.Linq;
using _GAME.Scripts.Lib.Enums;
using SimpleJSON;
using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems.DataManagement.GameDataHandlers
{
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
            Debug.Log(" ++++ ++++ Generating spawn queue  ++++ ++++ ");
            _spawnQueue = new SpawnQueue();
            var jsonObject = JSON.Parse(_data);

            for (int i = 1; i<jsonObject[2].Count; i++)
            {
                EnemyQueue enemyQueue = new EnemyQueue();
                Debug.Log(" --- Starting EnemyQueue: " + i);
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
                int bossMegaspiderCount = jsonObject[2][i][11].AsInt;
                Debug.Log("bossMegaspiderCount : " + bossMegaspiderCount);
                // Intro
                string enemyIntro = (jsonObject[2][i][18]).ToString().Replace("\"", "");
                
                int totalEnemies = mothlingCount + flyCount + mothCount + fireflyCount + ladybugCount + spiderCount;
                Debug.Log("Total enemies: " + totalEnemies);
                // Add boss
                int bossPosition = 10000;
                bool hasBossWithCustomPosition = false;
                EnemyType bossType = EnemyType.None;
                if (bossMegabeetleCount + bossMegamothlingCount + bossWaspCount + bossDragonflyCount + bossMegaspiderCount > 0)
                {
                    totalEnemies++;
                    if (bossMegabeetleCount > 0)
                    {
                        bossType = EnemyType.Megabeetle;
                    }
                    else if (bossMegamothlingCount > 0)
                    {
                        bossType = EnemyType.Megamothling;
                    }
                    else if (bossWaspCount > 0)
                    {
                        bossType = EnemyType.Wasp;
                        hasBossWithCustomPosition = true;
                        bossPosition = Random.Range(3, totalEnemies);
                    }
                    else if (bossDragonflyCount > 0)
                    {
                        bossType = EnemyType.Dragonfly;
                    }
                    else if (bossMegaspiderCount > 0)
                    {
                        bossType = EnemyType.Megaspider;
                    }
                }
            
                //Data
                enemyQueue.MaxEnemiesOnScreen = jsonObject[2][i][14].AsInt;
                enemyQueue.AggressionLevel = jsonObject[2][i][15].AsInt;
                enemyQueue.SpawnDelay = jsonObject[2][i][16].AsFloat;
                enemyQueue.SpawnDelayAcceleration = jsonObject[2][i][17].AsFloat;
                
                // Generate a base list
                List<EnemyType> rawEnemyList = new List<EnemyType>();
                if (mothlingCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Mothling, mothlingCount));
                if (flyCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Fly, flyCount));
                if (mothCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Moth, mothCount));
                if (fireflyCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Firefly, fireflyCount));
                if (ladybugCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Ladybug, ladybugCount));
                if (spiderCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Spider, spiderCount));
                if (bossWaspCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Wasp, bossWaspCount));
                if (bossMegamothlingCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Megamothling, bossMegamothlingCount));
                if (bossMegabeetleCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Megabeetle, bossMegabeetleCount));
                if (bossDragonflyCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Dragonfly, bossDragonflyCount));
                if (bossMegaspiderCount > 0)
                    rawEnemyList.AddRange(Enumerable.Repeat(EnemyType.Megaspider, bossMegaspiderCount));
                
                Debug.Log(rawEnemyList.Count + " enemies in queue");
                Debug.Log(rawEnemyList[0].ToString());
                
                // Shuffle the list
                int currentEnemyIndex = 0;
                List<EnemyType> shuffledEnemyList = new();
                
                // Introduce a new enemy
                Debug.Log("enemyIntro: " + enemyIntro);
                if (!string.IsNullOrEmpty(enemyIntro))
                {
                    EnemyType firstEnemyType = (EnemyType)System.Enum.Parse(typeof(EnemyType), enemyIntro);
                    if (rawEnemyList.Contains(firstEnemyType))
                    {
                        MoveListElement(rawEnemyList, shuffledEnemyList, firstEnemyType);
                        currentEnemyIndex++;
                    }
                }
                
                while (rawEnemyList.Count > 0 )
                {
                    if (hasBossWithCustomPosition 
                        && currentEnemyIndex == bossPosition 
                        && rawEnemyList.Contains(bossType))
                    {
                        MoveListElement(rawEnemyList, shuffledEnemyList, bossType);
                        currentEnemyIndex++;
                        hasBossWithCustomPosition = false;
                        continue;
                    }
                    
                    int listLength = rawEnemyList.Count;
                    if (hasBossWithCustomPosition)
                    {
                        listLength--;
                    }
                    int randomIndex = Random.Range(0, listLength);
                    MoveListElement(rawEnemyList, shuffledEnemyList, rawEnemyList[randomIndex]);
                }
                
                enemyQueue.SetRange(shuffledEnemyList);
                _spawnQueue.Add(enemyQueue);
            }
            return _spawnQueue;
        }
        
        private void MoveListElement(List<EnemyType> listFrom, List<EnemyType> listTo, EnemyType value)
        {
            listFrom.Remove(value);
            listTo.Add(value);
        }
    }
}
