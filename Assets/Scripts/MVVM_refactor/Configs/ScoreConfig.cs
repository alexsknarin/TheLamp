using UnityEngine;

[CreateAssetMenu(fileName = "ScoreConfig", menuName = "Configs/ScoreConfig")]
public class ScoreConfig : ScriptableObject
{
    [field:Header("------ Score Prices -------")]
    [field:SerializeField] public int MothlingScorePrice { get; private set; } = 1;
    [field: SerializeField] public int MegamothlingScorePrice { get; private set; } = 15;
    [field: SerializeField] public int FlyScorePrice { get; private set; } = 2;
    [field: SerializeField] public int FireflyScorePrice { get; private set; } = 3;
    [field: SerializeField] public int MothScorePrice { get; private set; } = 3;
    [field: SerializeField] public int LadybugScorePrice { get; private set; } = 4;
    [field: SerializeField] public int SpiderScorePrice { get; private set; } = 4;
    [field:SerializeField] public int WaspsScorePrice { get; private set; } = 25;
    [field: SerializeField] public int MegabeetleScorePrice { get; private set; } = 50;
    [field: SerializeField] public int DragonflyProjectileScorePrice { get; private set; } = 100;
}
