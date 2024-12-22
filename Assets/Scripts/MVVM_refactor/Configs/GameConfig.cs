using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    [field:Header("General Enemy Controller Settings")]
    [field:SerializeField] public bool IsTestStartWave { get; private set; } = false;
    [field:SerializeField] public int TestStartWave { get; private set; } = 0;
    [field:Header("Firefly Explosion Settings")]
    [field:SerializeField] public float FireflyExplosionRadius { get; private set; } = 0.8f;
    [field:SerializeField] public float FireflyExplosionDuration { get; private set; } = 0.22f;
}
