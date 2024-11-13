using UnityEngine;

public class DragonflyPatrolAttackZoneRangesData
{
    private Vector3 _frontZoneMin = Vector3.zero; 
    private Vector3 _frontZoneMax = Vector3.zero; 
    private Vector3 _backLZoneMin = Vector3.zero; 
    private Vector3 _backLZoneMax = Vector3.zero; 
    private Vector3 _backRZoneMin = Vector3.zero; 
    private Vector3 _backRZoneMax = Vector3.zero;
    public Vector3 FrontZoneMin => _frontZoneMin;
    public Vector3 FrontZoneMax => _frontZoneMax;
    public Vector3 BackLZoneMin => _backLZoneMin;
    public Vector3 BackLZoneMax => _backLZoneMax;
    public Vector3 BackRZoneMin => _backRZoneMin;
    public Vector3 BackRZoneMax => _backRZoneMax;
    
    public void SetData(Vector3 frontZoneMin, Vector3 frontZoneMax, Vector3 backLZoneMin, Vector3 backLZoneMax, Vector3 backRZoneMin, Vector3 backRZoneMax)
    {
        _frontZoneMin = frontZoneMin;
        _frontZoneMax = frontZoneMax;
        _backLZoneMin = backLZoneMin;
        _backLZoneMax = backLZoneMax;
        _backRZoneMin = backRZoneMin;
        _backRZoneMax = backRZoneMax;
    }
}
