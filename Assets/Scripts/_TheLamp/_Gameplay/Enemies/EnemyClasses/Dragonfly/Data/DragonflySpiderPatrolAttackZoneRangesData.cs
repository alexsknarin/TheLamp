using UnityEngine;

public class DragonflySpiderPatrolAttackZoneRangesData
{
    private Vector3 _frontLZoneMin = Vector3.zero; 
    private Vector3 _frontLZoneMax = Vector3.zero; 
    private Vector3 _backLZoneMin = Vector3.zero; 
    private Vector3 _backLZoneMax = Vector3.zero; 
    private Vector3 _backRZoneMin = Vector3.zero; 
    private Vector3 _backRZoneMax = Vector3.zero;
    private Vector3 _frontRZoneMin = Vector3.zero; 
    private Vector3 _frontRZoneMax = Vector3.zero;
    
    public Vector3 FrontLZoneMin => _frontLZoneMin;
    public Vector3 FrontLZoneMax => _frontLZoneMax;
    public Vector3 BackLZoneMin => _backLZoneMin;
    public Vector3 BackLZoneMax => _backLZoneMax;
    public Vector3 BackRZoneMin => _backRZoneMin;
    public Vector3 BackRZoneMax => _backRZoneMax;
    public Vector3 FrontRZoneMin => _frontRZoneMin;
    public Vector3 FrontRZoneMax => _frontRZoneMax;
    
    public void SetData(
        Vector3 frontLZoneMin, 
        Vector3 frontLZoneMax, 
        Vector3 backLZoneMin, 
        Vector3 backLZoneMax, 
        Vector3 backRZoneMin, 
        Vector3 backRZoneMax, 
        Vector3 frontRZoneMin, 
        Vector3 frontRZoneMax)
    {
        _frontLZoneMin = frontLZoneMin;
        _frontLZoneMax = frontLZoneMax;
        _backLZoneMin = backLZoneMin;
        _backLZoneMax = backLZoneMax;
        _backRZoneMin = backRZoneMin;
        _backRZoneMax = backRZoneMax;
        _frontRZoneMin = frontRZoneMin;
        _frontRZoneMax = frontRZoneMax;
    }
}
