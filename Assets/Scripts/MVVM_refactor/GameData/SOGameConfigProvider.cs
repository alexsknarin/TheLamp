using UnityEngine;

[CreateAssetMenu(fileName = "SOGameConfigProvider", menuName = "Configs/SOGameConfigProvider")]
public class SOGameConfigProvider : ScriptableObject, IGameConfigProvider
{
    [field:SerializeField] public SpawnQueueData SpawnQueueData { get; private set; }
    [field:SerializeField] public ScoreConfig ScoreConfig { get; private set; }
    [field:SerializeField] public PlayerConfig PlayerConfig { get; private set; }
}
