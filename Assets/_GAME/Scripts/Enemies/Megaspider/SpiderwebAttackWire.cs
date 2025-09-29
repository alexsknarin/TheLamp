using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class SpiderwebAttackWire: IStartEndPositionsProvider
    {
        private const float LampRadius = 0.5f;

        private Vector3 _startPosition = Vector3.zero;
        private Vector3 _endPosition = Vector3.zero;
        private Vector3 _endPositionTransformed = Vector3.zero;
        private Color _color = Random.ColorHSV();
        private bool _isActive = false;
        private int _hitPoints = 6;
        private bool _isDamaged = false;
        private bool _isDestroyed = false;
        private bool _isMainAttackWire = false;

        public event Action Activated;
        public event Action Damaged;
        public event Action Destroyed;
        public event Action Deactivated;
        public Vector3 StartPosition => _startPosition;
        public Vector3 EndPosition => _endPositionTransformed;
        public bool IsActive => _isActive;
        public bool IsDestroyed => _isDestroyed;


        public void Initialize(Vector3 startPosition, Vector3 endPosition)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
            _endPositionTransformed = Vector3.zero;
            _isActive = false;
            _hitPoints = 3;
            _isDestroyed = false;
        }

        public void UpdateEndPosition(Transform lampTransform)
        {
            _endPositionTransformed = lampTransform.localToWorldMatrix.MultiplyPoint(_endPosition);
        
            Color color = _color;
            if (_isActive)
            {
                color = Color.green;
            }

            Debug.DrawLine(_startPosition, _endPositionTransformed, color);
        
            if (_isDamaged)
            {
                Debug.DrawLine(_startPosition, _endPositionTransformed, Color.red, 0.055f);
                _isDamaged = false;
            }
        }

        public void SetActive(bool isActive)
        {
            if (!_isDestroyed)
            {
                _isActive = isActive;
                if (_isActive)
                {
                    Activated?.Invoke();
                }
                else
                {
                    Deactivated?.Invoke();
                }
                
                _isMainAttackWire = false;
            }
        }

        public void ReceiveDamage(int damage)
        {
            _hitPoints -= damage;
            _isDamaged = true;
            if (_hitPoints <= 0 && !_isDestroyed)
            {
                _isActive = false;
                _isDestroyed = true;
                Destroyed?.Invoke();
            }
            else
            {
                Damaged?.Invoke();
            }
        }
    }
}
