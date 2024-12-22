using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    [field:SerializeField] public bool IsTestStartWave { get; private set; } = false;
    [field:SerializeField] public int TestStartWave { get; private set; } = 0;
}
