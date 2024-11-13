using System;
using UnityEngine;
using Random = UnityEngine.Random;
public class DragonflyPatrolSpiderState : IState
{
    public event Action OnEndedEvent;
    private readonly float _minWaitTime = 0f;
    private readonly float _maxWaitTime = 1f;
    private float _localTime = 0f;
    private float _duration = 0f;
    
    public DragonflyPatrolSpiderState(float minWaitTime, float maxWaitTime)
    {
        _minWaitTime = minWaitTime;
        _maxWaitTime = maxWaitTime;
    }
    
    public void OnEnter()
    {
        _localTime = 0;
        _duration = Random.Range(_minWaitTime, _maxWaitTime);
    }

    public void Tick()
    {
        _localTime += Time.deltaTime;
        if (_localTime >= _duration)
        {
            OnEndedEvent?.Invoke();
        }
    }

    public void OnExit()
    {
    }
}
