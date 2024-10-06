using System;
using UnityEngine;

public class DragonflySwarm : MonoBehaviour
{
    [SerializeField] private Vector3 _startPositionL;
    [SerializeField] private Vector3 _startPositionMid;
    [SerializeField] private Vector3 _startPositionR;
    [SerializeField] private DragonflyProjectileMoth[] _moths;
    [SerializeField] private float _timeInterval = 1f;
    private Vector3[] _startPositions = new Vector3[3];
    
    private float _localTime = 0f;
    private bool _isWaitingForAttack = false;
    private int _attackCount = 0;
    
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
        _moths[_attackCount].Initialize(_startPositions[_attackCount]);
        _moths[_attackCount].AttackStart();
        _isWaitingForAttack = true;
    }
    
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.Y))
        {
            PlayAttack(1);
        }
        
        if (_isWaitingForAttack)
        {
            if (_localTime >= _timeInterval)
            {
                _attackCount++;
                if (_attackCount < _moths.Length)
                {
                    _moths[_attackCount].Initialize(_startPositions[_attackCount]);
                    _moths[_attackCount].AttackStart();
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
}
