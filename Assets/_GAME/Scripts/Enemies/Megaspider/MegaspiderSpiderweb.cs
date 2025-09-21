using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderSpiderweb : MonoBehaviour
    {
        private enum SpiderwebState
        {
            Idle,
            Shoot,
            Vibrate,
            Break,
            Hang,
            BreakHang
        }
        
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private AnimationCurve _shootSineAmplitudeCurve;
        [SerializeField] private AnimationCurve _vibrateAmplitudeCurve;
        [SerializeField] private AnimationCurve _vibrateFrequencyCurve;
        [SerializeField] private AnimationCurve _fallDeformationCurve;
        [SerializeField] private AnimationCurve _fallContractionCurve;
        private SpiderwebState _spiderwebState = SpiderwebState.Idle;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private IPositionProvider _endPositionProvider;
        private float _shootDuration = 0.25f;
        private float _shootSineFrequency = 13f;
        private int _shootPointCount = 20;
        private float _vibrateDuration = 0.45f;
        private float _fallDuration = 1f;
        private float _breakHangDuration = .65f;
        private float _breakHangNoiseOffset;
        private float _localTime;

        // TODO: Support moving end position
        public void StartShoot(Vector3 startPosition, Vector3 endPosition)
        {
            _localTime = 0;
            _startPosition = startPosition;
            _endPosition = endPosition;
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _shootPointCount;
            enabled = true;
            _spiderwebState = SpiderwebState.Shoot;
        }
        
        // TODO: Support moving end position
        private void PerformShoot()
        {
            Vector3 axis = (_endPosition - _startPosition).normalized;
            Vector3 perpendicular = Vector3.Cross(axis, Vector3.back);
            
            float phase = _localTime / _shootDuration;
            
            if (phase > 1)
            {
                StartVibrate();
                return;
            }
        
            Vector3 endPos = Vector3.Lerp(_startPosition, _endPosition, phase);

            for (int i = 0; i < _lineRenderer.positionCount; i++)
            {
                float resampledPhase = i / ((float) _lineRenderer.positionCount-1);
                Vector3 resampledPos = Vector3.Lerp(_startPosition, endPos, resampledPhase);
                float u = 1 - resampledPhase;
                float displace = Mathf.Sin(u * _shootSineFrequency * phase) 
                                 * _shootSineAmplitudeCurve.Evaluate(phase);
                resampledPos += perpendicular * (displace * resampledPhase);
            
                _lineRenderer.SetPosition(i, resampledPos);
            }
       
            _localTime += Time.deltaTime;
        }

        private void StartVibrate()
        {
            _localTime = 0;
            _spiderwebState = SpiderwebState.Vibrate;
        }
        
        // TODO: Support moving end position
        private void PerformVibrate()
        {
            Vector3 axis = (_endPosition - _startPosition).normalized;
            Vector3 perpendicular = Vector3.Cross(axis, Vector3.back);
        
            float phase = _localTime / _vibrateDuration;
        
            if (phase > 1)
            {
                phase = 1;
                enabled = false;
            }
        
            for (int i = 0; i < _lineRenderer.positionCount; i++)
            {
                float resampledPhase = i / ((float) _lineRenderer.positionCount-1);
                Vector3 resampledPos = Vector3.Lerp(_startPosition, _endPosition, resampledPhase);
                float u = 1 - resampledPhase;
                float displace = Mathf.Sin(u * Mathf.PI) 
                                 * Mathf.Cos(phase*_vibrateFrequencyCurve.Evaluate(phase))
                                 *_vibrateAmplitudeCurve.Evaluate(phase);
                resampledPos += perpendicular * displace;
            
                _lineRenderer.SetPosition(i, resampledPos);
            }
       
            _localTime += Time.deltaTime;
        }

        public void StartBreak()
        {
            _localTime = 0;
            _spiderwebState = SpiderwebState.Break;
            enabled = true;
        }

        private void PerformBreak()
        {
            Vector3 axis = (_endPosition - _startPosition).normalized;
            Vector3 perpendicular = Vector3.Cross(axis, Vector3.back);
        
            // Calculate once
            float fullAngle = Vector3.Angle(axis, Vector3.down);
        
            float phase = _localTime / _fallDuration;
            if (phase > 1)
            {
                enabled = false;
                _spiderwebState = SpiderwebState.Idle;
                _lineRenderer.enabled = false;
                return;
            }
        
            float currentAngle = Mathf.Lerp(0, fullAngle, Mathf.Pow(phase, 3f));
            float bendDirection = 1;
            if (_endPosition.x < _startPosition.x)
            {
                currentAngle *= -1;
                bendDirection *= -1;
            }
        
            Vector3 endPos = _endPosition - _startPosition;
            Quaternion endRotation = Quaternion.AngleAxis(currentAngle, Vector3.back);
            endPos = endRotation * endPos;
            endPos *= _fallContractionCurve.Evaluate(phase);
            perpendicular = endRotation * perpendicular;
            endPos += _startPosition;
        
        
            for (int i = 0; i < _lineRenderer.positionCount; i++)
            {
                float resampledPhase = i / ((float) _lineRenderer.positionCount-1);
                Vector3 resampledPos = Vector3.Lerp(_startPosition, endPos, resampledPhase);
                float u = 1 - resampledPhase;
                float displace = Mathf.Sin(u * 15f + phase * 10f) 
                                 * (1-u) 
                                 * _fallDeformationCurve.Evaluate(phase) * .5f
                                 - Mathf.Sin(u * Mathf.PI) * phase * bendDirection;

                resampledPos += perpendicular * displace;
            
                _lineRenderer.SetPosition(i, resampledPos);
            }
       
            _localTime += Time.deltaTime;
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
        }

        private void PerformHang()
        {
            _lineRenderer.SetPosition(1, _endPositionProvider.Position3D);
        }

        public void StopHang()
        {
            enabled = false;
            _spiderwebState = SpiderwebState.Idle;
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
            float phase = _localTime / _breakHangDuration;
            if (phase > 1)
            {
                enabled = false;
                _spiderwebState = SpiderwebState.Idle;
                _lineRenderer.enabled = false;
                return;
            }
            
            Vector3 endPos = Vector3.Lerp(_endPosition, _startPosition, phase);
                
            for(int i = 0; i < _lineRenderer.positionCount; i++)
            {
                float resampledPhase = i / ((float) _lineRenderer.positionCount-1);
                Vector3 resampledPos = Vector3.Lerp(_startPosition, endPos, resampledPhase);
                resampledPos.x += (Mathf.PerlinNoise1D(resampledPos.y + _breakHangNoiseOffset) - 0.5f) * phase * 2.8f;
                _lineRenderer.SetPosition(i, resampledPos);
            }

            _localTime += Time.deltaTime;
        }
        
        private void LateUpdate()
        {
            if (_spiderwebState == SpiderwebState.Shoot)
                PerformShoot();
            else if (_spiderwebState == SpiderwebState.Vibrate)
                PerformVibrate();
            else if (_spiderwebState == SpiderwebState.Break)
                PerformBreak();
            else if (_spiderwebState == SpiderwebState.Hang)
                PerformHang();
            else if (_spiderwebState == SpiderwebState.BreakHang)
                PerformBreakHang();
        }
    }
}
