using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private bool _skipIntro = true;
    [SerializeField] private float _introDuration = 4f;
    [SerializeField] private float _deathDuration = 3f;
    
    public bool SkipIntro => _skipIntro;
    public float IntroDuration => _introDuration;
    public float DeathDuration => _deathDuration;
}
