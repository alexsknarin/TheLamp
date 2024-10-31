using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DragonflyPatrolAttackPositionProvider
{
    private DragonflyPatrolAttackZoneRanges _patrolAttackZonesL;
    private DragonflyPatrolAttackZoneRanges _patrolAttackZonesR;
    
    private DragonflyPatrolAttackZoneRangesData _patrolAttackZonesDataL = new DragonflyPatrolAttackZoneRangesData();
    private DragonflyPatrolAttackZoneRangesData _patrolAttackZonesDataR = new DragonflyPatrolAttackZoneRangesData();
    private DragonflyPatrolAttackZoneRangesData _patrolAttackZonesData;
    
    private Vector3 _tailAttackZoneLMin;
    private Vector3 _tailAttackZoneLMax;
    private Vector3 _tailAttackZoneRMin;
    private Vector3 _tailAttackZoneRMax;
    
    public DragonflyPatrolAttackPositionProvider(
        DragonflyPatrolAttackZoneRanges patrolAttackZonesL,
        DragonflyPatrolAttackZoneRanges patrolAttackZonesR,
        Vector3 tailAttackZoneLMin,
        Vector3 tailAttackZoneLMax,
        Vector3 tailAttackZoneRMin,
        Vector3 tailAttackZoneRMax
        )
    {
        _patrolAttackZonesL = patrolAttackZonesL;
        _patrolAttackZonesR = patrolAttackZonesR;
        
        _patrolAttackZonesL.GetRanges(_patrolAttackZonesDataL);
        _patrolAttackZonesR.GetRanges(_patrolAttackZonesDataR);
        _patrolAttackZonesData = _patrolAttackZonesDataL;
        
        _tailAttackZoneLMin = tailAttackZoneLMin;
        _tailAttackZoneLMax = tailAttackZoneLMax;
        _tailAttackZoneRMin = tailAttackZoneRMin;
        _tailAttackZoneRMax = tailAttackZoneRMax;
    }
    
    public Vector3 GenerateRandomPreAttackHeadPosition(IState movementState)
    {
        // Get ranges Data
        if (movementState.GetType() == typeof(FDragonflyPatrolStateL))
        {
            _patrolAttackZonesData = _patrolAttackZonesDataL;
        }
        if (movementState.GetType() == typeof(FDragonflyPatrolStateR))
        {
            _patrolAttackZonesData = _patrolAttackZonesDataR;
        }
        
        // Select one of three zones
        int zone = Random.Range(0, 3);
        Vector3 rangeMin;
        Vector3 rangeMax;
        switch (zone)
        {
            case 0:
                // Front
                rangeMin = _patrolAttackZonesData.FrontZoneMin;
                rangeMax = _patrolAttackZonesData.FrontZoneMax;
                break;
            case 1:
                // Back L
                rangeMin = _patrolAttackZonesData.BackLZoneMin;
                rangeMax = _patrolAttackZonesData.BackLZoneMax;
                break;
            case 2:
                // Back R
                rangeMin = _patrolAttackZonesData.BackRZoneMin;
                rangeMax = _patrolAttackZonesData.BackRZoneMax;
                break;
            default:
                rangeMin = Vector3.zero;
                rangeMax = Vector3.zero;
                break;
        }
        
        // Generate random position inside a range
        Vector3 patrolAttackPosition = Vector3.zero;
        patrolAttackPosition.x = Random.Range(rangeMin.x, rangeMax.x);
        patrolAttackPosition.y = Random.Range(rangeMin.y, rangeMax.y);
        patrolAttackPosition.z = Random.Range(rangeMin.z, rangeMax.z);
        patrolAttackPosition.Normalize();
        
        Debug.DrawRay(Vector3.zero, patrolAttackPosition*2, Color.blue, 5f);
        
        patrolAttackPosition.y = 0;
        patrolAttackPosition.Normalize();
        
        return patrolAttackPosition;
    }
    
    public Vector3 GenerateRandomPreAttackTailPosition(IState movementState)
    {
        Vector3 rangeMin = Vector3.zero;
        Vector3 rangeMax = Vector3.zero;
        
        if (movementState.GetType() == typeof(FDragonflyPatrolStateL))
        {
            rangeMin = _tailAttackZoneLMin;
            rangeMax = _tailAttackZoneLMax;
        }
        if (movementState.GetType() == typeof(FDragonflyPatrolStateR))
        {
            rangeMin = _tailAttackZoneRMin;
            rangeMax = _tailAttackZoneRMax;
        }
        
        Vector3 patrolAttackPosition = Vector3.zero;
        patrolAttackPosition.x = Random.Range(rangeMin.x, rangeMax.x);
        patrolAttackPosition.y = Random.Range(rangeMin.y, rangeMax.y);
        patrolAttackPosition.z = Random.Range(rangeMin.z, rangeMax.z);
        patrolAttackPosition.Normalize();
        
        Debug.DrawRay(Vector3.zero, patrolAttackPosition*2, Color.red, 5f);
        
        patrolAttackPosition.y = 0;
        patrolAttackPosition.Normalize();

        return patrolAttackPosition;
    }
}
