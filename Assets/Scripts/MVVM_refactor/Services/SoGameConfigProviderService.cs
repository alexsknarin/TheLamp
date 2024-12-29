using UnityEngine;

[CreateAssetMenu(fileName = "SOGameConfigProviderService", menuName = "Configs/SOGameConfigProviderService")]
public class SoGameConfigProviderService : ScriptableObject, IGameConfigProviderService
{
    [field:SerializeField] public SpawnQueueData SpawnQueueData { get; private set; }
    [field:SerializeField] public ScoreConfig ScoreConfig { get; private set; }
    [field:SerializeField] public PlayerConfig PlayerConfig { get; private set; }
    [field:SerializeField] public GameConfig GameConfig { get; private set; }
}
