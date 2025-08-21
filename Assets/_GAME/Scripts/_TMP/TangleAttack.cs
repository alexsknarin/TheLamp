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
    
    private const float LampRadius = 0.51f;
    [SerializeField] private Transform _lampTransform;
    [SerializeField] private Vector3 _hangPoint;
    [SerializeField] private AnimationCurve _swingCurve;
    [SerializeField] private AnimationCurve _dropCurve;
    [SerializeField] private float _enterDuration;
    
    private List<Vector3> _collisionPoints = new();

    private float _localTime;
    private bool _isActive;
    private Vector3 _currentPosition;
    private Vector3 _lampPosition;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            _localTime = 0;
            _isActive = true;
            _collisionPoints.Clear();
        }

        if (_isActive)
        {
            _lampPosition = _lampTransform.position;
            CalculateSwingPosition();
            CheckLampIntersection();
            
            
        }
        DrawSwingLine();
    }

    private void DrawSwingLine()
    {
        Debug.DrawLine(_hangPoint, _currentPosition, Color.red);
        if (_collisionPoints.Count > 0)
        {
            foreach (var point in _collisionPoints)
            {
                Debug.DrawLine(Vector3.zero, point, Color.green);
            }    
        }
    }

    private void CheckLampIntersection()
    {
        // Find a reference angle
        float distanceFromHangToCenter = Vector3.Distance(_hangPoint, _lampPosition);
        float distanceToIntersect = Mathf.Sqrt(distanceFromHangToCenter * distanceFromHangToCenter - LampRadius * LampRadius);
        float referenceAngle = Mathf.Acos(distanceToIntersect / distanceFromHangToCenter) * Mathf.Rad2Deg;
            
        // Find a current angle
        Vector3 centerDirection = (_lampPosition - _hangPoint).normalized;
        Vector3 currentDirectionVector = _currentPosition - _hangPoint;
        Vector3 currentDirection = currentDirectionVector.normalized;
        float currentAngle = Vector3.Angle(centerDirection, currentDirection);
            
        // Check if the current angle is greater than the reference angle
        if (currentAngle < referenceAngle && _localTime > 0.5f)
        {
            _isActive = false;
            float differenceAngle = referenceAngle - currentAngle;
            Vector3 correctedDirection = Quaternion.AngleAxis(differenceAngle, Vector3.back) * currentDirectionVector;
                
            _currentPosition = _hangPoint + correctedDirection;
            Vector3 intersectionPoint = _hangPoint + correctedDirection.normalized * distanceToIntersect;
            _collisionPoints.Add(intersectionPoint);
        }
    }

    private void CalculateSwingPosition()
    {
        if (_localTime > _enterDuration)
        {
            _isActive = false;
            return;       
        }
        
        _currentPosition = _hangPoint;
        _currentPosition.y = _hangPoint.y + _dropCurve.Evaluate(_localTime);
        _currentPosition.x = _hangPoint.x + _swingCurve.Evaluate(_localTime);
        
        _localTime += Time.deltaTime;
    }
}
