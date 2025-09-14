using _GAME.Scripts.Enemies.Dragonfly;
using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [field:Header("Analytics Settings")]
        [field:SerializeField] public float AnalyticsTimeOutTime { get; private set; } = 10f;
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
        [field:Header("Enemy Configs")]
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
        
        [field:Header("Megaspider Settings")]
        [field:Header("Wire Attack State:")]
        [field:SerializeField] public int MegaspiderWireAttackNumberOfWires { get; private set; } = 5;
        [field:SerializeField] public float MegaspiderWireAttackTimeInterval { get; private set; } = 0.45f;
        [field:SerializeField] public float MegaspiderWireAttackSpiderAttackDelay { get; private set; } = 1.5f;
        [field:SerializeField] public float MegaspiderWireAttackMainAttackDuration { get; private set; } = 1.0f;
        [field:SerializeField] public float MegaspiderWireAttackMainAttackAcceleration { get; private set; } = 3.0f;
        [field:SerializeField] public SpiderwebSpawnRange[] MegaspiderWebStartPositionRanges { get; private set; }
        [field:Header("Tangle Attack State:")]
        [field:SerializeField] public float MegaspiderTangleAttackWireThickness { get; private set; } = 0.03f;
        [field:SerializeField] public Vector3 MegaspiderTangleAttackHangPoint { get; private set; }
        [field:SerializeField] public float MegaspiderTangleAttackTangleSpeed { get; private set; } = 330f;
        [field:SerializeField] public float MegaspiderTangleAttackTangleAcceleration { get; private set; } = 280f;
        [field:Header("Swing State:")]
        [field:SerializeField] public float MegaspiderSwingOverallSpedFactor { get; private set; } = 1.5f;
        [field:SerializeField] public float MegaspiderSwingInitialFallForceMagnitude { get; private set; } = .9f;
        [field:SerializeField] public float MegaspiderSwingSwingForceIncrement { get; private set; } = 5f;
        [field:SerializeField] public float MegaspiderSwingFallForceIncrement { get; private set; } = 2f;
        [field:SerializeField] public float MegaspiderSwingSwingDownIncrement { get; private set; } = 1f;
        [field:SerializeField] public float MegaspiderSwingExitDistance { get; private set; } = 4f;
        [field:SerializeField] public Vector3 MegaspiderSwingSwingPivot { get; private set; }
        
        [field:Header("Megaspider Projectile Spider Settings")]
        [field:Header("Attack State:")]
        [field:SerializeField] public float MegaspiderProjectileSpiderTransitionDuration { get; private set; } = 0.33f;
        [field:SerializeField] public float MegaspiderProjectileSpiderSpeed { get; private set; } = 5f;
        [field:SerializeField] public float MegaspiderProjectileSpiderGravity { get; private set; } = 3.5f;
        [field:SerializeField] public float MegaspiderProjectileSpiderLampSideMaximum { get; private set; } = 1.4f;
        [field:SerializeField] public float MegaspiderProjectileSpiderLampShiftMaximum { get; private set; } = .36f;
        [field:SerializeField] public float MegaspiderProjectileSpiderAttackAccelerationPower { get; private set; } = 0.65f;
        [field:SerializeField] public float MegaspiderProjectileSpiderAttackTopGravityMultiplier { get; private set; } = 0.25f;
        [field:Header("Bounce State:")]
        [field:SerializeField] public float MegaspiderProjectileSpiderBounceSpeed { get; private set; } = 2.5f;
    }
}
