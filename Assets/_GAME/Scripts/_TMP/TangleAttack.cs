using System;
using System.Collections.Generic;
using UnityEngine;

public class TangleAttack : MonoBehaviour
{
    private enum TangleStates
    {
        Inactive,
        Enter,
        Tangle
    }
    
    private const float LampRadius = 0.5f;
    private const float WireThickness = 0.03f;
    [SerializeField] private float _spiderRadius = 0.325f;
    
    [SerializeField] private Transform _lampTransform;
    [SerializeField] private Vector3 _hangPoint;
    [SerializeField] private AnimationCurve _swingCurve;
    [SerializeField] private AnimationCurve _dropCurve;
    [SerializeField] private float _enterDuration;
    [SerializeField] private float _tangleSpeed;
    [SerializeField] private float _tangleAcceleration;

    private float _tangleSpeedAccelerated;
    
    private TangleStates _state = TangleStates.Inactive;
    
    public List<Vector3> _collisionPointsLocalized = new();
    public List<Vector3> _collisionPoints = new();

    public float ReferenceAngle;
    public float CurrentAngle;

    private float _localTime;
    private Vector3 _currentPositionLocalized;
    private Vector3 _currentPosition;
    private Vector3 _currentPositionOnEnd;
    private Vector3 _lampWorldPosition;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartEnterState();
        }
        
        _lampWorldPosition = _lampTransform.position;

        if (_state == TangleStates.Enter)
        {
            HandleEnterState();
        }

        if (_state == TangleStates.Tangle)
        {
            HandleTangleState();
        }
        
        DrawSwingLine();
    }

    private void StartEnterState()
    {
        _localTime = 0;
        _collisionPoints.Clear();
        _state = TangleStates.Enter;
        // Reset Acceleration
        _tangleSpeedAccelerated = _tangleSpeed;
    }

    private void HandleEnterState()
    {
        // All Intersection calculations are in the world space
        _currentPosition = CalculateSwingPosition(_hangPoint);
        Vector3 currentPointToPivot = _currentPosition - _hangPoint;
        _currentPositionOnEnd = _hangPoint + currentPointToPivot.normalized * ( currentPointToPivot.magnitude + _spiderRadius);
        
        _localTime += Time.deltaTime;
        
        bool isIntersecting = FindCollisionPoints(
            _currentPosition, 
            _hangPoint, 
            _lampWorldPosition, 
            0.4f,
            true);

        if (isIntersecting)
        {
            StartTangleState();
        }
    }

    private bool FindCollisionPoints(
        Vector3 currentPosition, 
        Vector3 pivot,
        Vector3 center, 
        float timeToCheck, 
        bool isWorldSpace)
    {
        float referenceAngle;
        float currentAngle;
        float distancePivotToIntersectExpected;
        Vector3 currentPositionFromPivot;
        Vector3 correctedCurrentDirection;
            
        if (CheckForLampIntersection(
                currentPosition,
                pivot, 
                center, 
                timeToCheck, 
                out referenceAngle,
                out currentAngle,
                out distancePivotToIntersectExpected,
                out currentPositionFromPivot))
        {
            Vector3 tangentPoint = FindTangentPoint(
                pivot, 
                center,
                referenceAngle,
                currentAngle,
                currentPositionFromPivot,
                distancePivotToIntersectExpected,
                out correctedCurrentDirection
            );
            
            // TODO: extract method
            if (isWorldSpace)
            {
                _currentPosition = pivot + correctedCurrentDirection;
                _currentPositionLocalized = _lampTransform.worldToLocalMatrix.MultiplyPoint(_currentPosition);
                
                _collisionPoints.Add(tangentPoint);
                _collisionPointsLocalized.Add(_lampTransform.worldToLocalMatrix.MultiplyPoint(tangentPoint));
            }
            else
            {
                _currentPositionLocalized = pivot + correctedCurrentDirection;
                _collisionPointsLocalized.Add(tangentPoint);
                _collisionPoints.Add(_lampTransform.localToWorldMatrix.MultiplyPoint(tangentPoint));
            }
            return true;
        }
        return false;
    }

    private void StartTangleState()
    {
        _localTime = 0;
        _state = TangleStates.Tangle;
    }

    private void HandleTangleState()
    {
        // Update all existing collisionPoint positions to the new lamp positions
        for (int i = 0; i < _collisionPoints.Count; i++)
        {
            _collisionPoints[i] = _lampTransform.localToWorldMatrix.MultiplyPoint(_collisionPointsLocalized[i]);
        }
            
        // All calculations in lamp space
        // Rotate around the pivot
        Vector3 currentPositionLocalized = _currentPositionLocalized - _collisionPointsLocalized[^1];
        _tangleSpeedAccelerated += _tangleAcceleration * Time.deltaTime;
        currentPositionLocalized = Quaternion.AngleAxis(_tangleSpeedAccelerated * Time.deltaTime, Vector3.forward) * currentPositionLocalized;
        _currentPositionLocalized = currentPositionLocalized + _collisionPointsLocalized[^1];
            
            
        // Return Position into a world space
        _currentPosition = _lampTransform.localToWorldMatrix.MultiplyPoint(_currentPositionLocalized);
        Vector3 currentPointToPivot = _currentPosition - _collisionPoints[^1];
        _currentPositionOnEnd = _collisionPoints[^1] + currentPointToPivot.normalized * ( currentPointToPivot.magnitude + _spiderRadius);

        // Check for intersection
        bool isIntersecting = FindCollisionPoints(
            _currentPositionLocalized, 
            _collisionPointsLocalized[^1], 
            Vector3.zero, 
            0.01f,
            false);
            
        if (isIntersecting)
        {
            StartTangleState();
        }
            
        // Check For Collision
        if (Vector3.Distance(_currentPositionOnEnd, _lampWorldPosition) < _spiderRadius + LampRadius)
        {
            // Fix collision
            _currentPositionOnEnd = 
                (_currentPositionOnEnd - _lampWorldPosition).normalized 
                * (_spiderRadius + LampRadius) + _lampWorldPosition;
                
            Debug.Break();
        }
            
        _localTime += Time.deltaTime;
    }

    private void DrawSwingLine()
    {
        if (_collisionPoints.Count > 0)
        {
            foreach (var point in _collisionPoints)
            {
                Debug.DrawLine(_lampWorldPosition, point, Color.green);
            }
            Debug.DrawLine(_hangPoint, _collisionPoints[0], Color.red);
            Debug.DrawLine(_collisionPoints[^1], _currentPosition, Color.red);

            if (_collisionPoints.Count > 1)
            {
                for (int i = 1; i < _collisionPoints.Count; i++)
                {
                    Debug.DrawLine(_collisionPoints[i - 1], _collisionPoints[i], Color.red);
                }
            }
        }
        else
        {
            Debug.DrawLine(_hangPoint, _currentPosition, Color.red);
        }
    }

    private bool CheckForLampIntersection(
        Vector3 currentPosition,
        Vector3 pivot,
        Vector3 center, 
        float timeToCheck, 
        out float referenceAngle, 
        out float currentAngle,
        out float distancePivotToIntersectExpected,
        out Vector3 currentPositionFromPivot 
        ) 
    {
        // Find a reference angle
        float distancePivotToCenter = Vector3.Distance(pivot, center);
        distancePivotToIntersectExpected = Mathf.Sqrt(
            distancePivotToCenter * distancePivotToCenter - LampRadius * LampRadius);
        referenceAngle = Mathf.Acos(distancePivotToIntersectExpected / distancePivotToCenter) * Mathf.Rad2Deg;
        ReferenceAngle = referenceAngle;
        
        // Find a current angle
        Vector3 centerDirection = (center - pivot).normalized;
        currentPositionFromPivot = currentPosition - pivot;
        Vector3 currentDirection = currentPositionFromPivot.normalized;
        currentAngle = Vector3.Angle(centerDirection, currentDirection);
        CurrentAngle = currentAngle;
            
        // Check if the current angle is smaller than the reference angle
        if (currentAngle < referenceAngle && _localTime > timeToCheck)
            return true;
        
        return false;
    }

    private Vector3 FindTangentPoint(
        Vector3 pivot,
        Vector3 center,
        float referenceAngle, 
        float currentAngle, 
        Vector3 currentPositionFromPivot,
        float distancePivotToIntersectExpected,
        out Vector3 correctedDirection)
    {
        float differenceAngle = referenceAngle - currentAngle;
        correctedDirection = Quaternion.AngleAxis(differenceAngle, Vector3.back) * currentPositionFromPivot;
        Vector3 tangentPoint = pivot + correctedDirection.normalized * distancePivotToIntersectExpected;
        
        // Push tangent Point outside
        Vector3 pushDirection = (tangentPoint - center).normalized * WireThickness;
        tangentPoint += pushDirection;
        
        return tangentPoint;
    }

    private Vector3 CalculateSwingPosition(Vector3 pivot)
    {
        Vector3 pos = pivot;
        pos.y = pivot.y + _dropCurve.Evaluate(_localTime);
        pos.x = pivot.x + _swingCurve.Evaluate(_localTime);
        return pos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_currentPositionOnEnd, _spiderRadius);
    }
}
