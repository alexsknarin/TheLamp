using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [SerializeField] private float _attackDuration = 0.15f;
    public float AttackDuration => _attackDuration;
}
