using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [field:SerializeField] public bool IsDamageable { get; private set; } = true;
        [field:SerializeField] public float AttackDuration { get; private set; } = 0.15f;
        [field: SerializeField] public float DamageDuration { get; private set; } = 0.2f;
        [field:SerializeField] public int HealthCap { get; private set; } = 50;
        [field:SerializeField] public float CooldownTimeCap { get; private set; } = 0.75f;
        [field:SerializeField] public float CooldownDecrement { get; private set; } = 0.06f;
        [field: SerializeField] public float AttackDistanceCap { get; private set; } = 0.82f;
        [field: SerializeField] public float AttackDistanceIncrement { get; private set; } = 0.01f;
        [field: SerializeField] public float AttackDistanceUpgradeAnimationTime { get; private set; } = 0.45f;
        [field:Header("Collision Settings")]
        [field: SerializeField] public float LampCollisionRadius { get; private set; } = 0.49f;
        [field: SerializeField] public float CollisionThreshold { get; private set; } = 0.0001f;
        [field: SerializeField] public float DefaultAttackZoneRadius { get; private set; } = 0.62f;
    }
}
