using System.Collections.Generic;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderTangleAttackBaseState: EnemyMovementStateBase
    {
        private enum TangleStates
        {
            Inactive,
            Enter,
            Tangle
        }

        private const float WireThickness = 0.03f;
        private const float LampRadius = 0.5f; // TODO: Replace by DI
        private const float SpiderRadius = 0.325f;  // TODO: Replace by DI
        private static readonly Vector3 HangPoint = new Vector3(-0.96f, 3.13f, 0); // TODO: move to config

        // TODO: read from config
        private const float EnterDuration = 1f;
        private const float TangleSpeed = 330f;
        private const float TangleAcceleration = 280f;
        
        // Dependencies
        private Transform _lampTransform;
        private Transform _visibleBodyTransform;
        private Transform _calculatedTransform;
        private AnimationCurve _swingCurve;
        private AnimationCurve _dropCurve;
        
        private float _tangleSpeedAccelerated;
        private float _localTime;
        
        private TangleStates _state = TangleStates.Inactive;
        public List<Vector3> _collisionPointsLocalized = new();
        public List<Vector3> _collisionPoints = new();
        private Vector3 _hangPoint;
        private Vector3 _currentPositionLocalized;
        private Vector3 _currentPosition;
        private Vector3 _currentPositionOnEnd;
        private Vector3 _lampWorldPosition;
        private bool _isLeftSide;
        private int _side;

        public MegaSpiderTangleAttackBaseState(
            Transform lampTransform, 
            Transform visibleBody, 
            Transform calculatedTransform,  
            AnimationCurve swingCurve, 
            AnimationCurve dropCurve,
            bool isLeftSide
            )
        {
            _lampTransform = lampTransform;
            _visibleBodyTransform = visibleBody;
            _calculatedTransform = calculatedTransform;
            _swingCurve = swingCurve;
            _dropCurve = dropCurve;
            _isLeftSide = isLeftSide;
            _side = isLeftSide ? 1 : -1;
            
            _hangPoint = HangPoint;
            _hangPoint.x *= _side;
        }
        
        public override void Enter()
        {
            ParentVisibleBodyToAnimatedTransform();
            StartEnterState();
        }

        public override void Tick()
        {
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
            
            _calculatedTransform.position = _currentPositionOnEnd;
        }
        
        private void StartEnterState()
        {
            _localTime = 0;
            _collisionPoints.Clear();
            _tangleSpeedAccelerated = TangleSpeed;
            _state = TangleStates.Enter;
        }
        
        private void HandleEnterState()
        {
            // All Intersection calculations are in the world space
            _currentPosition = CalculateSwingPosition(_hangPoint);
            Vector3 currentPointToPivot = _currentPosition - _hangPoint;
            _currentPositionOnEnd = _hangPoint + currentPointToPivot.normalized * ( currentPointToPivot.magnitude + SpiderRadius);
        
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
            _tangleSpeedAccelerated += TangleAcceleration * Time.deltaTime;

            Vector3 axis = Vector3.back;
            if (_isLeftSide)
                axis = Vector3.forward;
            
            currentPositionLocalized = Quaternion.AngleAxis(_tangleSpeedAccelerated * Time.deltaTime, axis) * currentPositionLocalized;
            _currentPositionLocalized = currentPositionLocalized + _collisionPointsLocalized[^1];
                
                
            // Return Position into a world space
            _currentPosition = _lampTransform.localToWorldMatrix.MultiplyPoint(_currentPositionLocalized);
            Vector3 currentPointToPivot = _currentPosition - _collisionPoints[^1];
            _currentPositionOnEnd = _collisionPoints[^1] + currentPointToPivot.normalized * ( currentPointToPivot.magnitude + SpiderRadius);

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
            if (Vector3.Distance(_currentPositionOnEnd, _lampWorldPosition) < SpiderRadius + LampRadius)
            {
                // Fix collision
                _currentPositionOnEnd = 
                    (_currentPositionOnEnd - _lampWorldPosition).normalized 
                    * (SpiderRadius + LampRadius) + _lampWorldPosition;
                    
                Debug.Break();
            }
                
            _localTime += Time.deltaTime;
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
            
                // TODO: extract as method
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
            
            // Find a current angle
            Vector3 centerDirection = (center - pivot).normalized;
            currentPositionFromPivot = currentPosition - pivot;
            Vector3 currentDirection = currentPositionFromPivot.normalized;
            currentAngle = Vector3.Angle(centerDirection, currentDirection);
                
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
            
            Vector3 axis = Vector3.forward;
            if (_isLeftSide)
                axis = Vector3.back;
            correctedDirection = Quaternion.AngleAxis(differenceAngle, axis) * currentPositionFromPivot;
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
            pos.x = pivot.x + _swingCurve.Evaluate(_localTime) * _side;
            return pos;
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
        
        // TODO: extract to library as a static method 
        private void ParentVisibleBodyToAnimatedTransform()
        {
            _visibleBodyTransform.SetParent(_calculatedTransform, false);
            _visibleBodyTransform.localPosition = Vector3.zero;
            _visibleBodyTransform.localRotation = Quaternion.identity;
        }
    }
}
