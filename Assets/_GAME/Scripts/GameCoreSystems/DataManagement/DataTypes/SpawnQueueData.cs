using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes
{
    [CreateAssetMenu(menuName = "Assets/Create/SpawnQueueData")]
    public class SpawnQueueData : ScriptableObject
    {
        [SerializeField] private string _spawnQueue;
        public string Data
        {
            get { return _spawnQueue; }
            set { _spawnQueue = value; }
        }
    }
}
