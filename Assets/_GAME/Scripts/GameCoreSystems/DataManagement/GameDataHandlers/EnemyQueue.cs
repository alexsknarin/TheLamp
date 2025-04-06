using System.Collections.Generic;
using _GAME.Scripts.Lib.Enums;
using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems.DataManagement.GameDataHandlers
{
    public class EnemyQueue
    {
        private List<EnemyType> _enemies;
        public int MaxEnemiesOnScreen { get; set; }
        public int AggressionLevel { get; set; }
        /// <summary>
        /// Spawn delay is the time between each enemy is being spawned
        /// </summary>
        public float SpawnDelay { get; set; }
        /// <summary>
        /// Modifier to the spawn delay - how much shorter (or longer) spawn cooldown should be by the end of the wave.
        /// For example value of 2 means that the spawn delay will two times shorter by the end of the wave.
        /// </summary>
        public float SpawnDelayAcceleration { get; set; }

        public void Clear()
        {
            _enemies.Clear();
        }
    
        public void SetRange(List<EnemyType> enemyList)
        {
            _enemies = enemyList;
        }
    
        public int Count()
        {
            return _enemies.Count;
        }
    
        public EnemyType Get(int index)
        {
            if (index < _enemies.Count)
            {
                return _enemies[index];    
            }
            else
            {
                return EnemyType.None;
            }
        }

        public bool PushEnemyForward(int enemyIndex, EnemyType enemyType)
        {
            int replacementIndex = enemyIndex;
            bool hasReplacemesnt = false;
            // TODO: potential otimization
            while (replacementIndex < _enemies.Count) 
            {
                if (_enemies[replacementIndex] != enemyType)
                {
                    hasReplacemesnt = true;
                    break;
                }
                replacementIndex++;
            }
            
            if (hasReplacemesnt)
            {
                SwapEnemies(enemyIndex, replacementIndex);
                return true;
            }
            return false;
        }
        
        private void SwapEnemies(int index1, int index2)
        {
            (_enemies[index1], _enemies[index2]) = (_enemies[index2], _enemies[index1]);
        }
    }
}
