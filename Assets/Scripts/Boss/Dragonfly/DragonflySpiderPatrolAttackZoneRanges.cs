using UnityEngine;

[CreateAssetMenu(fileName = "DragonflySpiderPatrolAttackZoneRanges", menuName = "DragonflyData/DragonflySpiderPatrolAttackZoneRanges")]
public class DragonflySpiderPatrolAttackZoneRanges : ScriptableObject
{
    [SerializeField] private Vector3 _frontLZoneMin; 
    [SerializeField] private Vector3 _frontLZoneMax; 
    [SerializeField] private Vector3 _backLZoneMin; 
    [SerializeField] private Vector3 _backLZoneMax; 
    [SerializeField] private Vector3 _backRZoneMin; 
    [SerializeField] private Vector3 _backRZoneMax; 
    [SerializeField] private Vector3 _frontRZoneMin; 
    [SerializeField] private Vector3 _frontRZoneMax;
    
    public void GetRanges(DragonflySpiderPatrolAttackZoneRangesData data)
    {
        data.SetData(
            _frontLZoneMin,
            _frontLZoneMax,
            _backLZoneMin,
            _backLZoneMax,
            _backRZoneMin,
            _backRZoneMax,
            _frontRZoneMin,
            _frontRZoneMax
            );
    }
}
