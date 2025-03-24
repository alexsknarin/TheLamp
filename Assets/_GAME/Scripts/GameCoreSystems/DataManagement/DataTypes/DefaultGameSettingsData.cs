using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes
{
    [CreateAssetMenu(fileName = "DefaultGameSettingsData", menuName = "Configs/DefaultGameSettingsData")]
    public class DefaultGameSettingsData : ScriptableObject
    {
        [field: SerializeField] public GameSettings GameSettings { get; private set; }
    }
}
