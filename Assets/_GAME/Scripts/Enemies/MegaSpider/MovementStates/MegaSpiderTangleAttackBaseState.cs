using System;
using System.Collections.Generic;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using Unity.Hierarchy;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderTangleAttackBaseState: EnemyMovementStateBase, ITangledWireProvider
    {
        private enum TangleStates
        {
            Inactive,
            Enter,
            Tangle
        }
        
        // Dependencies
        private readonly Transform _lampTransform;
        private readonly Transform _visibleBodyTransform;
        private readonly Transform _calculatedTransform;
        private readonly AnimationCurve _swingCurve;
        private readonly AnimationCurve _dropCurve;
        private readonly float _megaspiderRadius;
        // Config
        private readonly float _lampRadius;
        private readonly float _wireThickness;
        private readonly float _tangleSpeed;
        private readonly float _tangleAcceleration;
        
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
        private Vector3 _startPoint;
        private List<Vector3> _collisionPoints1;
        private Vector3 _endPoint;

        public MegaspiderTangleAttackBaseState(
            Transform lampTransform, 
            Transform visibleBody, 
            Transform calculatedTransform,  
            AnimationCurve swingCurve, 
            AnimationCurve dropCurve,
            IGameConfigService configService,
            float megaspiderRadius,
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
            _megaspiderRadius = megaspiderRadius;

            _hangPoint = configService.GameConfig.MegaspiderTangleAttackHangPoint;
            _hangPoint.x *= _side;
            _lampRadius = configService.PlayerConfig.LampCollisionRadius;
            _wireThickness = configService.GameConfig.MegaspiderTangleAttackWireThickness;
            _tangleSpeed = configService.GameConfig.MegaspiderTangleAttackTangleSpeed;
            _tangleAcceleration = configService.GameConfig.MegaspiderTangleAttackTangleAcceleration;
        }
        
        public event Action Started;
        public event Action Ended;
        public event Action<Vector3> PivotChanged;
        
        Vector3 ITangledWireProvider.StartPoint => _hangPoint;
        List<Vector3> ITangledWireProvider.CollisionPoints => _collisionPoints;
        Vector3 ITangledWireProvider.EndPoint => _currentPosition;
        
        public override void Enter()
        {
            HierarchyUtilities.ParentWithoutOffset(
                _visibleBodyTransform, 
                _calculatedTransform);
            
            StartEnterState();
            Started?.Invoke();
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
            _tangleSpeedAccelerated = _tangleSpeed;
            _state = TangleStates.Enter;
        }
        
        private void HandleEnterState()
        {
            // All Intersection calculations are in the world space
            _currentPosition = CalculateSwingPosition(_hangPoint);
            Vector3 currentPointToPivot = _currentPosition - _hangPoint;
            _currentPositionOnEnd = _hangPoint + currentPointToPivot.normalized * ( currentPointToPivot.magnitude + _megaspiderRadius);
        
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
            _tangleSpeedAccelerated += _tangleAcceleration * Time.deltaTime;

            Vector3 axis = Vector3.back;
            if (_isLeftSide)
                axis = Vector3.forward;
            
            currentPositionLocalized = Quaternion.AngleAxis(_tangleSpeedAccelerated * Time.deltaTime, axis) * currentPositionLocalized;
            _currentPositionLocalized = currentPositionLocalized + _collisionPointsLocalized[^1];
                
                
            // Return Position into a world space
            _currentPosition = _lampTransform.localToWorldMatrix.MultiplyPoint(_currentPositionLocalized);
            Vector3 currentPointToPivot = _currentPosition - _collisionPoints[^1];
            _currentPositionOnEnd = _collisionPoints[^1] + currentPointToPivot.normalized * ( currentPointToPivot.magnitude + _megaspiderRadius);

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
            
                if (isWorldSpace)
                {
                    _currentPosition = pivot + correctedCurrentDirection;
                    _currentPositionLocalized = _lampTransform.worldToLocalMatrix.MultiplyPoint(_currentPosition);
                
                    _collisionPoints.Add(tangentPoint);
                    PivotChanged?.Invoke(tangentPoint);
                    _collisionPointsLocalized.Add(_lampTransform.worldToLocalMatrix.MultiplyPoint(tangentPoint));
                }
                else
                {
                    _currentPositionLocalized = pivot + correctedCurrentDirection;
                    _collisionPointsLocalized.Add(tangentPoint);
                    Vector3 worldPivot = _lampTransform.localToWorldMatrix.MultiplyPoint(tangentPoint);
                    _collisionPoints.Add(worldPivot);
                    PivotChanged?.Invoke(worldPivot);
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
                distancePivotToCenter * distancePivotToCenter - _lampRadius * _lampRadius);
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
            Vector3 pushDirection = (tangentPoint - center).normalized * _wireThickness;
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
                Debug.DrawLine(_hangPoint, _collisionPoints[0], Color.yellow);
                Debug.DrawLine(_collisionPoints[^1], _currentPosition, Color.yellow);

                if (_collisionPoints.Count > 1)
                {
                    for (int i = 1; i < _collisionPoints.Count; i++)
                    {
                        Debug.DrawLine(_collisionPoints[i - 1], _collisionPoints[i], Color.yellow);
                    }
                }
            }
            else
            {
                Debug.DrawLine(_hangPoint, _currentPosition, Color.yellow);
            }
        }

        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
