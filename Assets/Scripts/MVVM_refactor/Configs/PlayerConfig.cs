using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [SerializeField] private float _attackDuration = 0.15f;
    [SerializeField] private int _healthCap = 50;
    [SerializeField] private float _cooldownTimeCap = 0.75f;
    [SerializeField] private float _cooldownDecrement = 0.06f;
    [SerializeField] private float _attackDistanceCap = 0.82f;
    public float AttackDuration => _attackDuration;
    public int HealthCap => _healthCap;
    public float CooldownTimeCap => _cooldownTimeCap;
    public float CooldownDecrement => _cooldownDecrement;
    public float AttackDistanceCap => _attackDistanceCap;
}
