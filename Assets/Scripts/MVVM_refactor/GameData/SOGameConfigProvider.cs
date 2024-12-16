using UnityEngine;

[CreateAssetMenu(fileName = "SOGameConfigProvider", menuName = "Configs/SOGameConfigProvider")]
public class SOGameConfigProvider : ScriptableObject, IGameConfigProvider
{
    [SerializeField] private SpawnQueueData _spawnQueueData;
    public SpawnQueueData SpawnQueueData => _spawnQueueData;
    
    [SerializeField] private ScoreConfig _scoreConfig;
    public ScoreConfig ScoreConfig => _scoreConfig;
}
