using System;
using System.Collections.Generic;
using UnityEngine;

public class LadybugLampPositionsHolder : MonoBehaviour
{
    [SerializeField] private Vector3[] _ladybugLampPositions = new Vector3[8];
    [SerializeField] private bool _showGizmos;
    [SerializeField] private float _gizmoDistance;
    [SerializeField] private float _gizmoRadius;
    private List<int> _occupiedPositions = new();
    private int _lastIndex;


    public Vector2 GetClosestLandingPosition(Vector2 position)
    {
        _lastIndex = FindClosestIndex(position);
        return (_ladybugLampPositions[_lastIndex] + transform.position);
    }

    public bool CheckIfClosestLandingPositionIsFree()
    {
        if (_occupiedPositions.Contains(_lastIndex))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void OccupyClosestLandingPosition()
    {
        _occupiedPositions.Add(_lastIndex);
    }
    
    public void FreeLandingPosition(Vector2 position)
    {
        int index = FindClosestIndex(position);
        if (_occupiedPositions.Contains(index))
        {
            _occupiedPositions.Remove(index);
        }
    }

    private int FindClosestIndex(Vector2 position)
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < 8; i++)
        {
            Vector2 currentPosition = _ladybugLampPositions[i] + transform.position;
            float distance = Vector2.Distance(position, currentPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    private void OnDrawGizmos()
    {
        if (_showGizmos)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < _ladybugLampPositions.Length; i++)
            {
                if (_occupiedPositions.Contains(i))
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.cyan;
                }
                Vector3 position = _ladybugLampPositions[i] * _gizmoDistance + transform.position;
                Vector3 catchPosition =  _ladybugLampPositions[i] + transform.position;
                Gizmos.DrawWireSphere(position, _gizmoRadius);
                Gizmos.DrawLine(transform.position, position);
                Gizmos.DrawLine(transform.position, catchPosition);
            }
            Gizmos.color = Color.white;    
        }
    }
}
