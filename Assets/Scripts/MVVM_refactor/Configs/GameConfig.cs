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
