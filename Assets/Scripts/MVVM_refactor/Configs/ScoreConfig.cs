using UnityEngine;

[CreateAssetMenu(fileName = "ScoreConfig", menuName = "Configs/ScoreConfig")]
public class ScoreConfig : ScriptableObject
{
    [Header("Score per enemy prices")] 
    [SerializeField] private int _mothlingScorePrice = 1;
    [SerializeField] private int _megamothlingScorePrice = 15;
    [SerializeField] private int _flyScorePrice = 2;
    [SerializeField] private int _fireflyScorePrice = 3;
    [SerializeField] private int _mothScorePrice = 3;
    [SerializeField] private int _ladybugScorePrice = 4;
    [SerializeField] private int _spiderScorePrice = 4;
    [SerializeField] private int _waspsScorePrice = 25;
    [SerializeField] private int _megabeetleScorePrice = 50;
    [SerializeField] private int _dragonflyProjectileScorePrice = 100;
    public int MothlingScorePrice => _mothlingScorePrice;
    public int MegamothlingScorePrice => _megamothlingScorePrice;
    public int FlyScorePrice => _flyScorePrice;
    public int FireflyScorePrice => _fireflyScorePrice;
    public int MothScorePrice => _mothScorePrice;
    public int LadybugScorePrice => _ladybugScorePrice;
    public int SpiderScorePrice => _spiderScorePrice;
    public int WaspsScorePrice => _waspsScorePrice;
    public int MegabeetleScorePrice => _megabeetleScorePrice;
    public int DragonflyProjectileScorePrice => _dragonflyProjectileScorePrice;
    
}
