using System;
using UnityEngine;
using Random = UnityEngine.Random;
public class DragonflyPatrolTailState : IState
{
    public event Action OnEnded;
    private float _minWaitTime = 0f;
    private float _maxWaitTime = 1f;
    private float _localTime = 0f;
    private float _duration = 0f;
    
    public DragonflyPatrolTailState(float minWaitTime, float maxWaitTime)
    {
        _minWaitTime = minWaitTime;
        _maxWaitTime = maxWaitTime;
    }
    
    public void OnEnter()
    {
        _localTime = 0;
        _duration = Random.Range(_minWaitTime, _maxWaitTime);
        Debug.Log("DragonflyPatrolTailState duration = " + _duration);
    }

    public void Tick()
    {
        _localTime += Time.deltaTime;
        if (_localTime >= _duration)
        {
            Debug.Log("DragonflyPatrolTailState OnEnded");
            OnEnded?.Invoke();
        }
    }

    public void OnExit()
    {
    }
}
