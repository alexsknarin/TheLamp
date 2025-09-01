using System;
using System.Collections.Generic;
using _GAME.Scripts.Lib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderWireAttackState : EnemyMovementStateBase
    {
        private enum WireStates
        {
            Inactive,
            WireAttack,
            PreAttackPause,
            MainAttack
        }
        
        private const float LampRadius = 0.5f;
        private const float SpiderRadius = 0.325f;
        private const float CollisionThreshold = 0.00001f;
        private const int NumberOfWires = 5;
        private const float WireAttackTimeInterval = 0.45f; // TODO: to config
        private const float SpiderAttackDelay = 1.5f;       // TODO: to config
        private const float MainAttackDuration = 1f;
        private const float MainAttackAcceleration = 3f;
        
        // TODO: extract to a config
        private readonly SpiderwebSpawnRange[] _webStartPositionsRanges = new []
        {
            new SpiderwebSpawnRange(-0.1696585f, -1.571486f, -5.533077f, -0.5621628f, -1.601049f, -5.457012f),
            new SpiderwebSpawnRange(-0.5621628f, -1.601049f, -5.457012f, -0.8296229f, -1.489632f, -4.630661f),
            new SpiderwebSpawnRange(-0.8296229f, -1.489632f, -4.630661f, -1.097083f, -1.378216f, -3.80431f),
            new SpiderwebSpawnRange(-2.991441f, 2.364229f, 1.8949f, -3.222974f, 3.461487f, 3.060626f),
            new SpiderwebSpawnRange(-3.222974f, 3.461487f, 3.060626f, -3.454506f, 4.558745f, 4.226351f),
            new SpiderwebSpawnRange(0.1696585f, -1.571486f, -5.533077f, 0.5621628f, -1.601049f, -5.457012f),
            new SpiderwebSpawnRange(0.5621628f, -1.601049f, -5.457012f, 0.8296229f, -1.489632f, -4.630661f),
            new SpiderwebSpawnRange(0.8296229f, -1.489632f, -4.630661f, 1.097083f, -1.378216f, -3.80431f),
            new SpiderwebSpawnRange(2.991441f, 2.364229f, 1.8949f, 3.222974f, 3.461487f, 3.060626f),
            new SpiderwebSpawnRange(3.222974f, 3.461487f, 3.060626f, 3.454506f, 4.558745f, 4.226351f)
        };
        
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
        
        public MegaSpiderWireAttackState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,  
            Transform lampTransform, 
            Transform cameraTransform)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _lampTransform = lampTransform;
            _cameraTransform = cameraTransform;
            
            _fullCollisionDistance = SpiderRadius + LampRadius + CollisionThreshold;
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
                // TODO: remove later
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
            float attackPhase = _localTime / WireAttackTimeInterval;
            if (attackPhase > 1)
            {
                _attackWires[_currentAttackingWireIndex].SetActive(true);
                _currentAttackingWireIndex++;
                _localTime = 0f;
            
                if (_currentAttackingWireIndex >= NumberOfWires)
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
            if (_localTime / SpiderAttackDelay > 1)
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

            float phase = _localTime / MainAttackDuration;
            _currentPosition = Vector3.Lerp(
                _mainAttackWire.StartPosition, 
                _mainAttackWire.EndPosition, 
                Mathf.Pow(phase, MainAttackAcceleration));
    
            CheckForCollision();
        
            _localTime += Time.deltaTime;    
        
        }
        
        // ---
        private void CheckForCollision()
        {
            Vector3 cameraPos = _cameraTransform.position;
            Vector3 projectedPos = CameraProjection.ProjectPointOnXYPlane(cameraPos, _currentPosition);
        
            Vector3 collisionDirection = (_lampTransform.position - projectedPos).normalized;
            Vector3 collisionPoint = projectedPos + collisionDirection * SpiderRadius;
            collisionPoint = CameraProjection.ProjectPointOnXYPlane(cameraPos, collisionPoint);


            float projectedDistance = (_lampTransform.position - collisionPoint).magnitude; 
            if (projectedDistance < (LampRadius + CollisionThreshold))
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
        
        // TODO: DI to make this class recieve lamp attacks!
        private void ReceiveWireDamage(int power)
        {
            RefreshActiveWiresList();

            if (_activeWireIndices.Count == 0 && _destroyedWiresCount == NumberOfWires)
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
            for (int i = 0; i < NumberOfWires; i++)
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
        
            for (int i = 0; i < NumberOfWires; i++)
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
        
        // TODO: extract to library as a static method 
        private void ParentVisibleBodyToAnimatedTransform()
        {
            _visibleBodyTransform.SetParent(_calculatedTransform, false);
            _visibleBodyTransform.localPosition = Vector3.zero;
            _visibleBodyTransform.localRotation = Quaternion.identity;
        }
    }
}
