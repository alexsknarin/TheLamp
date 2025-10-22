using System;
using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderSpiderweb : MonoBehaviour
    {
        private enum SpiderwebState
        {
            Inactive,
            ShootStatic,
            VibrateStatic,
            FallBreakStatic,
            TautStatic,
            ShootDynamic,
            VibrateDynamic,
            ShootDynamicFull,
            VibrateDynamicFull,
            TautDynamic,
            FallBreakDynamic,
            Hang,
            BreakHang,
            Tangle,
            Detangle
            
        }
        
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private AnimationCurve _shootSineAmplitudeCurve;
        [SerializeField] private AnimationCurve _vibrateAmplitudeCurve;
        [SerializeField] private AnimationCurve _vibrateFrequencyCurve;
        [SerializeField] private AnimationCurve _fallDeformationCurve;
        [SerializeField] private AnimationCurve _fallContractionCurve;
        private SpiderwebState _spiderwebState = SpiderwebState.Inactive;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private List<Vector3> _tanglePositions;
        private IPositionProvider _startPositionProvider;
        private IPositionProvider _endPositionProvider;
        private IStartEndPositionsProvider _startEndPositionsProvider;
        private ITangledWireProvider _tangledWireProvider;
        private float _shootDuration = 0.25f;
        private float _shootSineFrequency = 13f;
        private int _shootPointCount = 20;
        private float _vibrateDuration = 0.45f;
        private float _vibrateAmplitude = 1f;
        private float _fallDuration = 1f;
        private float _breakHangDuration = .65f;
        private float _breakHangNoiseOffset;
        private float _localTime;

        public void StartShootStatic(
            Vector3 startPosition, 
            Vector3 endPosition, 
            float vibrateDuration = 0.45f,
            float vibrateAmplitude = 1.0f)
        {
            _localTime = 0;
            _startPosition = startPosition;
            _endPosition = endPosition;
            _vibrateDuration = vibrateDuration;
            _vibrateAmplitude = vibrateAmplitude;
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _shootPointCount;
            enabled = true;
            _spiderwebState = SpiderwebState.ShootStatic;
        }
        
        private void PerformShootStatic()
        {
            bool isShootFinished = SpiderwebMotionLibrary.CalculateWireShootMotion(
                _startPosition,
                _endPosition,
                _lineRenderer,
                _shootDuration,
                _shootSineFrequency,
                _shootSineAmplitudeCurve,
                ref _localTime);
            
            if (isShootFinished)
                StartVibrateStatic();
        }


        private void StartVibrateStatic()
        {
            _localTime = 0;
            _spiderwebState = SpiderwebState.VibrateStatic;
        }
        
        private void PerformVibrateStatic()
        {
            bool isVibrateFinished = SpiderwebMotionLibrary.CalculateWireVibrateMotion(
                _startPosition,
                _endPosition,
                _lineRenderer,
                _vibrateDuration,
                _vibrateFrequencyCurve,
                _vibrateAmplitudeCurve,
                _vibrateAmplitude,
                ref _localTime);
            
            if (isVibrateFinished)
            {
                enabled = false;
            }
        }

        public void StartFallBreakStatic()
        {
            _localTime = 0;
            _spiderwebState = SpiderwebState.FallBreakStatic;
            enabled = true;
        }

        private void PerformFallBreakStatic()
        {
            bool isBreakFinished = SpiderwebMotionLibrary.CalculateWireFallBreakMotion(
                _startPosition,
                _endPosition,
                _lineRenderer,
                _fallDuration,
                _fallContractionCurve,
                _fallDeformationCurve,
                ref _localTime);
            
            if (isBreakFinished)
            {
                _spiderwebState = SpiderwebState.Inactive;
                _lineRenderer.enabled = false;
                enabled = false;
            }
        }
        
        public void StartHang(Vector3 startPosition, IPositionProvider endPositionProvider)
        {
            _localTime = 0;
            _startPosition = startPosition;
            _endPositionProvider = endPositionProvider;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _startPosition);
            _lineRenderer.SetPosition(1, _endPositionProvider.Position3D);
            _lineRenderer.enabled = true;
            enabled = true;
            _spiderwebState = SpiderwebState.Hang;

            if (_startPosition.z < -3.5f)
            {
                _lineRenderer.widthMultiplier = 0.019f;
            }
            else
            {
                _lineRenderer.widthMultiplier = 0.025f;
            }
        }

        private void PerformHang()
        {
            _lineRenderer.SetPosition(1, _endPositionProvider.Position3D);
        }

        public void StopHang()
        {
            enabled = false;
            _spiderwebState = SpiderwebState.Inactive;
            _lineRenderer.enabled = false;
        }
        
        public void StartBreakHang()
        {
            _endPosition = _endPositionProvider.Position3D;
            _spiderwebState = SpiderwebState.BreakHang;
            _lineRenderer.positionCount = _shootPointCount;
            _breakHangNoiseOffset = Random.Range(0.0f, 2.5f);
            _localTime = 0;
            enabled = true;
        }
        
        private void PerformBreakHang()
        {
            bool isBreakHangFinished = SpiderwebMotionLibrary.CalculateWireBreakHangMotion(
                _startPosition,
                _endPosition,
                _lineRenderer,
                _breakHangDuration,
                _breakHangNoiseOffset,
                ref _localTime);
            
            if (isBreakHangFinished)
            {
                _spiderwebState = SpiderwebState.Inactive;
                _lineRenderer.enabled = false;
                enabled = false;
            }
        }
        
        public void StartTangle(ITangledWireProvider tangledWireProvider)
        {
            _tangledWireProvider = tangledWireProvider;
            _spiderwebState = SpiderwebState.Tangle;
            _lineRenderer.enabled = true;
            enabled = true;
        }
        
        private void PerformTangle()
        {
            int currentPointIndex = 0;
            _lineRenderer.positionCount = 2 + _tangledWireProvider.CollisionPoints.Count;
            _lineRenderer.SetPosition(currentPointIndex, _tangledWireProvider.StartPoint);
            currentPointIndex++;
            if (_tangledWireProvider.CollisionPoints.Count > 0)
            {
                foreach (var point in _tangledWireProvider.CollisionPoints)
                {
                    _lineRenderer.SetPosition(currentPointIndex, point);
                    currentPointIndex++;
                }
            }
            _lineRenderer.SetPosition(currentPointIndex, _tangledWireProvider.EndPoint);
        }

        public void StartDetangle()
        {
            _startPosition = _tangledWireProvider.StartPoint;
            _endPosition = _tangledWireProvider.EndPoint;
            _tanglePositions = _tangledWireProvider.CollisionPoints;
            _lineRenderer.enabled = true;
            _localTime = 0f;
            enabled = true;
            _spiderwebState = SpiderwebState.Detangle;
        }

        private void PerformDetangle()
        {
            _lineRenderer.positionCount = 2 + _tangledWireProvider.CollisionPoints.Count;
            
            int currentPointIndex = 0;
            _lineRenderer.SetPosition(currentPointIndex, _startPosition);
            currentPointIndex++;

            if (_tanglePositions.Count > 0)
            {
                for (int i = 0; i < _tanglePositions.Count; i++)
                {
                    Vector3 expandedDirection = _tanglePositions[i].normalized;
                    _tanglePositions[i] = _tanglePositions[i] + expandedDirection * (Time.deltaTime * .8f); // Make speed parameter
                    _lineRenderer.SetPosition(currentPointIndex, _tanglePositions[i]);
                    currentPointIndex++;
                }
            
                float phase = _localTime / 0.03f;
                if (phase > 1f)
                {
                    _endPosition = _tanglePositions[_tanglePositions.Count - 1];
                    _tanglePositions.RemoveAt(_tanglePositions.Count - 1);
                    _localTime = 0;
                }
                else
                {
                    _lineRenderer.SetPosition(
                        currentPointIndex, 
                        Vector3.Lerp(_endPosition, _tanglePositions[_tanglePositions.Count - 1], phase)
                    );
                }
                _localTime += Time.deltaTime;
            }
            else
            {
                _lineRenderer.SetPosition(currentPointIndex, _endPosition);
                StartPostTangleBreakHang();
            }
        }
        
        private void StartPostTangleBreakHang()
        {
            _spiderwebState = SpiderwebState.BreakHang;
            _lineRenderer.positionCount = _shootPointCount;
            _breakHangNoiseOffset = Random.Range(0.0f, 2.5f);
            _localTime = 0;
            enabled = true;
        }
        
        public void StartShootDynamic(
            IPositionProvider startPositionProvider, 
            Vector3 endPosition, 
            float vibrateDuration = 0.45f,
            float vibrateAmplitude = 1.0f
            )
        {
            _localTime = 0;
            _startPosition = startPositionProvider.Position3D;
            _endPosition = endPosition;
            _vibrateDuration = vibrateDuration;
            _vibrateAmplitude = vibrateAmplitude;
            _startPositionProvider = startPositionProvider;
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _shootPointCount;
            enabled = true;
            _spiderwebState = SpiderwebState.ShootDynamic;
        }
        
        private void PerformShootDynamic()
        {
            bool isShootFinished = SpiderwebMotionLibrary.CalculateWireShootMotion(
                _startPositionProvider.Position3D,
                _endPosition,
                _lineRenderer,
                _shootDuration,
                _shootSineFrequency,
                _shootSineAmplitudeCurve,
                ref _localTime);
            
            if (isShootFinished)
                StartVibrateDynamic();
        }
        
        private void StartVibrateDynamic()
        {
            _localTime = 0;
            _spiderwebState = SpiderwebState.VibrateDynamic;
        }
        
        private void PerformVibrateDynamic()
        {
            bool isVibrateFinished = SpiderwebMotionLibrary.CalculateWireVibrateMotion(
                _startPositionProvider.Position3D,
                _endPosition,
                _lineRenderer,
                _vibrateDuration,
                _vibrateFrequencyCurve,
                _vibrateAmplitudeCurve,
                _vibrateAmplitude,
                ref _localTime);
            
            if (isVibrateFinished)
            {
                StartHang(_endPosition, _startPositionProvider);
            }
        }

        public void StartShootDynamicFull(IStartEndPositionsProvider startEndPositionProvider, float vibrateAmplitude = 1.0f) 
        {
            _localTime = 0;
            _startPosition = startEndPositionProvider.StartPosition;
            _endPosition = startEndPositionProvider.EndPosition;
            _vibrateAmplitude = vibrateAmplitude;
            _startEndPositionsProvider = startEndPositionProvider;
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _shootPointCount;
            enabled = true;
            _spiderwebState = SpiderwebState.ShootDynamicFull;
        }

        private void PerformShootDynamicFull()
        {
            bool isShootFinished = SpiderwebMotionLibrary.CalculateWireShootMotion(
                _startEndPositionsProvider.StartPosition,
                _startEndPositionsProvider.EndPosition,
                _lineRenderer,
                _shootDuration,
                _shootSineFrequency,
                _shootSineAmplitudeCurve,
                ref _localTime);
            
            if (isShootFinished)
                StartVibrateDynamicFull();
        }
        
        private void StartVibrateDynamicFull()
        {
            _localTime = 0;
            _spiderwebState = SpiderwebState.VibrateDynamicFull;
        }
        
        private void PerformVibrateDynamicFull()
        {
            bool isVibrateFinished = SpiderwebMotionLibrary.CalculateWireVibrateMotion(
                _startEndPositionsProvider.StartPosition,
                _startEndPositionsProvider.EndPosition,
                _lineRenderer,
                _vibrateDuration,
                _vibrateFrequencyCurve,
                _vibrateAmplitudeCurve,
                _vibrateAmplitude,
                ref _localTime);
            
            if (isVibrateFinished)
            {
                StartTautDynamic();
            }
        }

        private void StartTautDynamic()
        {
            _localTime = 0;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _startEndPositionsProvider.StartPosition);
            _lineRenderer.SetPosition(1, _startEndPositionsProvider.EndPosition);
            _lineRenderer.enabled = true;
            enabled = true;
            _spiderwebState = SpiderwebState.TautDynamic;
        }
        
        private void PerformTautDynamic()
        {
            _lineRenderer.SetPosition(0, _startEndPositionsProvider.StartPosition);
            _lineRenderer.SetPosition(1, _startEndPositionsProvider.EndPosition);
        }
        
        public void StartFallBreakDynamic()
        {
            _localTime = 0;
            _lineRenderer.positionCount = _shootPointCount;
            _startPosition = _startEndPositionsProvider.StartPosition;
            _endPosition = _startEndPositionsProvider.EndPosition;
            _lineRenderer.enabled = true;
            _spiderwebState = SpiderwebState.FallBreakStatic;
            enabled = true;
        }

        private void LateUpdate()
        {
            if (_spiderwebState == SpiderwebState.ShootStatic)
                PerformShootStatic();
            else if (_spiderwebState == SpiderwebState.VibrateStatic)
                PerformVibrateStatic();
            else if (_spiderwebState == SpiderwebState.FallBreakStatic)
                PerformFallBreakStatic();
            else if (_spiderwebState == SpiderwebState.Hang)
                PerformHang();
            else if (_spiderwebState == SpiderwebState.BreakHang)
                PerformBreakHang();
            else if (_spiderwebState == SpiderwebState.Tangle)
                PerformTangle();
            else if (_spiderwebState == SpiderwebState.Detangle)
                PerformDetangle();
            else if (_spiderwebState == SpiderwebState.ShootDynamic)
                PerformShootDynamic();
            else if (_spiderwebState == SpiderwebState.VibrateDynamic)
                PerformVibrateDynamic();
            else if (_spiderwebState == SpiderwebState.ShootDynamicFull)
                PerformShootDynamicFull();
            else if (_spiderwebState == SpiderwebState.VibrateDynamicFull)
                PerformVibrateDynamicFull();
            else if (_spiderwebState == SpiderwebState.TautDynamic)
                PerformTautDynamic();
        }
    }
}
