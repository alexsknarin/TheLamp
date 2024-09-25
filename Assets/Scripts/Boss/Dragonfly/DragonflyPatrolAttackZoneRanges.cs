using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyPatrolAttackZoneRanges", menuName = "DragonflyData/DragonflyPatrolAttackZoneRanges")]
public class DragonflyPatrolAttackZoneRanges : ScriptableObject
{
    [SerializeField] private Vector3 _frontZoneMin; 
    [SerializeField] private Vector3 _frontZoneMax; 
    [SerializeField] private Vector3 _backLZoneMin; 
    [SerializeField] private Vector3 _backLZoneMax; 
    [SerializeField] private Vector3 _backRZoneMin; 
    [SerializeField] private Vector3 _backRZoneMax; 
    
    public void GetRanges(DragonflyPatrolAttackZoneRangesData data)
    {
        data.SetData(
            _frontZoneMin,
            _frontZoneMax,
            _backLZoneMin,
            _backLZoneMax,
            _backRZoneMin,
            _backRZoneMax
            );
    }
}
