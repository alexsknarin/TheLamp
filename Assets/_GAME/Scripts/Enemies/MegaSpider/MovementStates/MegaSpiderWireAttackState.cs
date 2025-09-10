using System;
using System.Collections.Generic;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderWireAttackState : EnemyMovementStateBase
    {
        private enum WireStates
        {
            Inactive,
            WireAttack,
            PreAttackPause,
            MainAttack
        }

        private readonly SpiderwebSpawnRange[] _webStartPositionsRanges;
        
        private readonly Vector3 _lampEndPoint = new (0.0f, -0.35f, 0);
        private readonly Vector3 _inactivePosition = new (0.0f, -10.0f, 0);
        
        private List<int> _availableWireRangeIndices = new();
        private List<SpiderwebAttackWire> _attackWires = new();
        private List<int> _activeWireIndices = new();
        
        private WireStates _wireState;
        private SpiderwebAttackWire _mainAttackWire;
        private float _fullCollisionDistance;
        private Vector3 _currentPosition;
    
        private int _currentAttackingWireIndex = 0;
        private int _destroyedWiresCount = 0;
        private float _localTime;
    
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _calculatedTransform;
        private Transform _lampTransform;
        private Transform _cameraTransform;

        private readonly float _spiderRadius;
        private readonly float _lampRadius;
        private readonly float _collisionThreshold;
        private readonly int _numberOfWires;
        private readonly float _wireAttackTimeInterval; 
        private readonly float _spiderAttackDelay;       
        private readonly float _mainAttackDuration;
        private readonly float _mainAttackAcceleration;
        
        public MegaspiderWireAttackState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,  
            Transform lampTransform, 
            Transform cameraTransform,
            IGameConfigService configService,
            float spiderRadius
            )
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _lampTransform = lampTransform;
            _cameraTransform = cameraTransform;
            
            _spiderRadius = spiderRadius;
            _lampRadius = configService.PlayerConfig.LampCollisionRadius;
            _collisionThreshold = configService.PlayerConfig.CollisionThreshold;
            _numberOfWires = configService.GameConfig.MegaspiderWireAttackNumberOfWires;
            _wireAttackTimeInterval = configService.GameConfig.MegaspiderWireAttackTimeInterval;
            _spiderAttackDelay = configService.GameConfig.MegaspiderWireAttackSpiderAttackDelay;
            _mainAttackDuration = configService.GameConfig.MegaspiderWireAttackMainAttackDuration;
            _mainAttackAcceleration = configService.GameConfig.MegaspiderWireAttackMainAttackAcceleration;
            _webStartPositionsRanges = configService.GameConfig.MegaspiderWebStartPositionRanges;
            _fullCollisionDistance = _spiderRadius + _lampRadius + _collisionThreshold;
            _wireState = WireStates.Inactive;
            _currentPosition = _inactivePosition;
            CreateEmptyAttackWireVariables();
        }
        public event Action Started;
        
        public bool IsDropped { get; private set; }
        
        public override void Enter()
        {
            // Transform setup
            ParentVisibleBodyToAnimatedTransform();
            
            // Show source ranges
            foreach (var range in _webStartPositionsRanges)
            {
                Debug.DrawLine(range.p1, range.p2, Color.red, 10);
            }
            
            _wireState = WireStates.Inactive;
            
            IsDropped = false;
            IsReadyToSwitch = false;
            _destroyedWiresCount = 0;
            _localTime = 0;
            InitializeAttackWires();
            StartWireAttack();
        }

        public override void Tick()
        {
            // TODO: test later and remove
            if(Input.GetKeyDown(KeyCode.C))
                IsDropped = true;
            
            
            if (_wireState == WireStates.WireAttack)
            {
                HandleWiresAttack();
            }
            else if (_wireState == WireStates.PreAttackPause)
            {
                HandlePreAttackPause();
            }
            else if (_wireState == WireStates.MainAttack)
            {
                HandleMainAttack();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                ReceiveWireDamage(3);
            }

            foreach (var wire in _attackWires)
            {
                wire.UpdateEndPosition(_lampTransform);
            }
            
            _calculatedTransform.position = _currentPosition;
        }

        public override void Exit()
        {
            _localTime = 0;
            _availableWireRangeIndices.Clear();
            _activeWireIndices.Clear();
            foreach (var wire in _attackWires)
            {
                wire.SetActive(false);
            }

            _currentPosition = _inactivePosition;
            _wireState = WireStates.Inactive;
        }

        private void StartWireAttack()
        {
            _currentAttackingWireIndex = 0;
            _localTime = 0f;
            _wireState = WireStates.WireAttack;
        }
        
        private void HandleWiresAttack()
        {
            float attackPhase = _localTime / _wireAttackTimeInterval;
            if (attackPhase > 1)
            {
                _attackWires[_currentAttackingWireIndex].SetActive(true);
                _currentAttackingWireIndex++;
                _localTime = 0f;
            
                if (_currentAttackingWireIndex >= _numberOfWires)
                {
                    StartPreAttackPause();
                }
            }
            _localTime += Time.deltaTime;
        }
        
        private void StartPreAttackPause()
        {
            _wireState = WireStates.PreAttackPause;
            _localTime = 0f;
        }
        
        private void HandlePreAttackPause()
        {
            if (_localTime / _spiderAttackDelay > 1)
            {
                StartMainAttack();            
            }
            _localTime += Time.deltaTime;
        }
        
        private void StartMainAttack()
        {
            _localTime = 0;
            _wireState = WireStates.MainAttack;
            RefreshActiveWiresList();
            if (_activeWireIndices.Count == 0)
            {
                IsDropped = true;
                return;       
            }
            _mainAttackWire = _attackWires[_activeWireIndices[Random.Range(0, _activeWireIndices.Count)]];
            Debug.DrawLine(_mainAttackWire.StartPosition, _mainAttackWire.EndPosition, Color.orangeRed, 10);
            Started?.Invoke();
        }
        
        private void HandleMainAttack()
        {
            if (_mainAttackWire == null)
            {
                return;
            }

            float phase = _localTime / _mainAttackDuration;
            _currentPosition = Vector3.Lerp(
                _mainAttackWire.StartPosition, 
                _mainAttackWire.EndPosition, 
                Mathf.Pow(phase, _mainAttackAcceleration));
    
            CheckForCollision();
        
            _localTime += Time.deltaTime;    
        
        }
        
        // ---
        private void CheckForCollision()
        {
            Vector3 cameraPos = _cameraTransform.position;
            Vector3 projectedPos = CameraProjection.ProjectPointOnXYPlane(cameraPos, _currentPosition);
        
            Vector3 collisionDirection = (_lampTransform.position - projectedPos).normalized;
            Vector3 collisionPoint = projectedPos + collisionDirection * _spiderRadius;
            collisionPoint = CameraProjection.ProjectPointOnXYPlane(cameraPos, collisionPoint);


            float projectedDistance = (_lampTransform.position - collisionPoint).magnitude; 
            if (projectedDistance < (_lampRadius + _collisionThreshold))
            {
                // Push Back to resolve penetration
                Vector3 correctedProjectedPosition = (projectedPos - _lampTransform.position).normalized  * _fullCollisionDistance;
                Vector3 lampEndPos = _lampTransform.TransformPoint(_lampEndPoint);

                float fullSideA = (correctedProjectedPosition - lampEndPos).magnitude;
                float sideA1 = (projectedPos - lampEndPos).magnitude;
            
                float sideB1 = (lampEndPos - _currentPosition).magnitude;
                float fullSideB = (fullSideA * sideB1) / sideA1;
                _currentPosition = lampEndPos + (_currentPosition - lampEndPos).normalized * fullSideB;
                
                IsReadyToSwitch = true; // TODO: External collision Detection!
            }
        }
        
        private void ReceiveWireDamage(int power)
        {
            RefreshActiveWiresList();

            if (_activeWireIndices.Count == 0 && _destroyedWiresCount == _numberOfWires)
            {
                IsDropped = true;
                return;
            }
        
            if (_activeWireIndices.Count == 0)
                return;
        
            SpiderwebAttackWire wire = _attackWires[_activeWireIndices[Random.Range(0, _activeWireIndices.Count)]];
            wire.ReceiveDamage(power);
            if (wire.IsDestroyed)
            {
                _destroyedWiresCount++;
            }
        }
        
        private void CreateEmptyAttackWireVariables()
        {
            for (int i = 0; i < _numberOfWires; i++)
            {
                SpiderwebAttackWire wire = new();
                _attackWires.Add(wire);
            }
        }
        
        private void InitializeAttackWires() 
        {
            _availableWireRangeIndices.Clear();
            
            for (int i = 0; i < _webStartPositionsRanges.Length; i++)
                _availableWireRangeIndices.Add(i);
        
            for (int i = 0; i < _numberOfWires; i++)
            {
                int randomRange = Random.Range(0, _availableWireRangeIndices.Count);
                int randomIndex = _availableWireRangeIndices[randomRange];
                _availableWireRangeIndices.RemoveAt(randomRange);
            
                Vector3 startPoint = Vector3.Lerp(
                    _webStartPositionsRanges[randomIndex].p1,
                    _webStartPositionsRanges[randomIndex].p2,
                    Random.Range(0f, 1f));
                _attackWires[i].Initialize(
                    startPoint, 
                    GetWireStickPoint(startPoint, _lampEndPoint)
                );
            }
        }
        
        private Vector3 GetWireStickPoint(Vector3 startPosition, Vector3 lampEndPoint)
        {
            Vector3 centerPoint = Vector3.zero;
            Vector3 startPoint = startPosition;
            Vector3 endPoint = lampEndPoint;

            Vector3 endToStartDir = (startPoint - endPoint).normalized;
            Vector3 endToCenterDir = (centerPoint - endPoint).normalized;

            float endAngle = Vector3.Angle(endToStartDir, endToCenterDir);
            float intersectionAngle = Mathf.Asin((0.35f * Mathf.Sin(Mathf.Deg2Rad * endAngle)) / 0.5f) * Mathf.Rad2Deg;
            float centerAngle = 180 - endAngle - intersectionAngle;

            float intersectionDistance =
                (0.5f * Mathf.Sin(Mathf.Deg2Rad * centerAngle)) / Mathf.Sin(Mathf.Deg2Rad * endAngle);

            Vector3 intesectionPos = endPoint + endToStartDir * intersectionDistance;
        
            return intesectionPos;
        }
        
        private void RefreshActiveWiresList()
        {
            _activeWireIndices.Clear();

            for (int i = 0; i < _attackWires.Count; i++)
            {
                if (_attackWires[i].IsActive)
                {
                    _activeWireIndices.Add(i);
                }
            }
        }
        
        private void ParentVisibleBodyToAnimatedTransform()
        {
            _visibleBodyTransform.SetParent(_calculatedTransform, false);
            _visibleBodyTransform.localPosition = Vector3.zero;
            _visibleBodyTransform.localRotation = Quaternion.identity;
        }
    }
}
