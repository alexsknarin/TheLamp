using UnityEngine;

[CreateAssetMenu(fileName = "DefaultGameStateData", menuName = "Configs/DefaultGameStateData")]
public class DefaultGameStateData : ScriptableObject
{
    [field: SerializeField] public GameState GameState { get; private set; } 
}
