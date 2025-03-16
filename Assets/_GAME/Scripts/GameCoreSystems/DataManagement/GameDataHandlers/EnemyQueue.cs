using System.Collections.Generic;
using _GAME.Scripts.Lib.Enums;

namespace _GAME.Scripts.GameCoreSystems.DataManagement.GameDataHandlers
{
    public class EnemyQueue
    {
        private List<EnemyType> _enemies;

        public EnemyQueue()
        {
            _enemies = new List<EnemyType>();
        }

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
    
        public void Add(EnemyType enemy)
        {
            _enemies.Add(enemy);
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

    }
}
