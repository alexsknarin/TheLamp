using _GAME.Scripts.Enemies.Dragonfly;
using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [field:Header("Analytics Settings")]
        [field:SerializeField] public float AnalyticsTimeOutTime { get; private set; } = 10f;
        [field:Header("General Enemy Controller Settings")]
        [field:SerializeField] public bool IsSpawnDisabled { get; private set; } = false;
        [field:SerializeField] public bool IsTestStartWave { get; private set; } = false;
        [field:SerializeField] public int TestStartWave { get; private set; } = 0;
        [field:Header("Firefly Explosion Settings")]
        [field:SerializeField] public float FireflyExplosionRadius { get; private set; } = 0.8f;
        [field:SerializeField] public float FireflyExplosionDuration { get; private set; } = 0.22f;
        [field:Header("Enemy Wave Settings")]
        [field:SerializeField] public float MaxAggressionLevel { get; private set; } = 6f;
        [field:SerializeField] public float FirstEnemySpawnDelay { get; private set; } = 0.5f;
        [field:Header("Ladybug Settings")]
        [field:SerializeField] public float LadybugDeathDepth { get; private set; } = 0.3f;
        [field:Header("Dragonfly Settings")]
        [field:SerializeField] public DragonflyPatrolAttackZoneRanges PatrolAttackZonesL { get; private set; }
        [field:SerializeField] public DragonflyPatrolAttackZoneRanges PatrolAttackZonesR { get; private set; }
        [field:SerializeField] public Vector3 DragonflyTailAttackPositionBase { get; private set; }
        [field:SerializeField] public float DragonflyPatrolHeadWaitMin { get; private set; } = 0f;
        [field:SerializeField] public float DragonflyPatrolHeadWaitMax { get; private set; } = 0.5f;
        [field:SerializeField] public float DragonflyPatrolTailWaitMin { get; private set; } = 0f;
        [field:SerializeField] public float DragonflyPatrolTailWaitMax { get; private set; } = 0.5f;
        [field:SerializeField] public float DragonflyHoverWaitMin { get; private set; } = 0.5f;
        [field:SerializeField] public float DragonflyHoverWaitMax { get; private set; } = 2f;
        [field:SerializeField] public Vector3 DragonflySpiderAttackPositionBase { get; private set; }
        [field:SerializeField] public float DragonflySpiderPatrolWaitMin { get; private set; } = 1.05f;
        [field:SerializeField] public float DragonflySpiderPatrolWaitMax { get; private set; } = 1.8f;
        [field:Header("Game Stages Settings")]
        [field:SerializeField] public bool IntroStageSkip { get; private set; } = false;
        [field:SerializeField] public float IntroStageDuration { get; private set; } = 2f;
    
        [field:SerializeField] public bool PrepareInStageSkip { get; private set; } = false;
        [field:SerializeField] public float PrepareInStageDuration { get; private set; } = .5f;
    
        [field:SerializeField] public bool PrepareOutStageSkip { get; private set; } = false;
        [field:SerializeField] public float PrepareOutStageDuration { get; private set; } = .5f;
    
        [field:SerializeField] public bool GameoverInStageSkip { get; private set; } = false;
        [field:SerializeField] public float GameoverInStageDuration { get; private set; } = 5.5f;
    
        [field:SerializeField] public bool GameoverOutStageSkip { get; private set; } = false;
        [field:SerializeField] public float GameoverOutStageDuration { get; private set; } = .7f;
    }
}
