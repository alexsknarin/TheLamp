using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly
{
    public class DragonflySwarm : MonoBehaviour, IInitializable
    {
        [SerializeField] private Vector3 _startPositionL;
        [SerializeField] private Vector3 _startPositionMid;
        [SerializeField] private Vector3 _startPositionR;
        [SerializeField] private DragonflyProjectileMoth.DragonflyProjectileMoth[] _moths;
        [SerializeField] private float _timeInterval = 1f;
        private readonly Vector3[] _startPositions = new Vector3[3];
        private float _localTime = 0f;
        private bool _isWaitingForAttack = false;
        private int _attackCount = 0;

        public event Action<CollidableEnemy> MothAttackStarted;
        public event Action<Enemy, bool> MothDeactivated;
    
        public void Initialize()
        {
            for (int i = 0; i < _moths.Length; i++)
            {
                _moths[i].gameObject.SetActive(false);
                _moths[i].Initialize();
            }
            _localTime = 0f;
            _isWaitingForAttack = false;
            _attackCount = 0;
        }
        
        public void SetDuration(float duration)
        {
            _timeInterval = duration/3;
        }

        private void OnEnable()
        {
            for (int i = 0; i < _moths.Length; i++)
            {
                _moths[i].Deactivated += OnMothDeactivated;
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _moths.Length; i++)
            {
                _moths[i].Deactivated -= OnMothDeactivated;
            }
        }

        public void PlayAttack(int direction)
        {
            if (direction == 1)
            {
                _startPositions[0] = _startPositionR;
                _startPositions[1] = _startPositionMid;
                _startPositions[2] = _startPositionL;
            }
            else
            {
                _startPositions[0] = _startPositionL;
                _startPositions[1] = _startPositionMid;
                _startPositions[2] = _startPositionR;
            }
            _localTime = 0f;
            _moths[_attackCount].gameObject.SetActive(true);
            _moths[_attackCount].SetStartPosition(_startPositions[_attackCount]);
            _moths[_attackCount].Play();
            MothAttackStarted?.Invoke(_moths[_attackCount]);
            _isWaitingForAttack = true;
        }

        public void TriggerGameover()
        {
            _localTime = 0f;
            _isWaitingForAttack = false;
            for (int i = 0; i < _moths.Length; i++)
            {
                _moths[i].TriggerGameOver();
            }
        }

        private void Update()
        {
            if (_isWaitingForAttack)
            {
                if (_localTime >= _timeInterval)
                {
                    _attackCount++;
                    if (_attackCount < _moths.Length)
                    {
                        _moths[_attackCount].gameObject.SetActive(true);
                        _moths[_attackCount].SetStartPosition(_startPositions[_attackCount]);
                        _moths[_attackCount].Play();
                        MothAttackStarted?.Invoke(_moths[_attackCount]);
                        _localTime = 0f;
                    }
                    else
                    {
                        _isWaitingForAttack = false;
                        _attackCount = 0;
                    }
                }
                else
                {
                    _localTime += Time.deltaTime;
                }
            }
        }

        private void OnMothDeactivated(Enemy enemy, bool damaged)
        {
            MothDeactivated?.Invoke(enemy, damaged);
        }
    }
}
