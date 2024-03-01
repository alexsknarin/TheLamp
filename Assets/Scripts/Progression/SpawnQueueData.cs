using UnityEditor;
using UnityEngine;

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
